using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Services.Common;
using ColorControl.Svc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using nspector.Common;
using nspector.Common.Meta;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.GPU.Structures;
using NWin32.NativeTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Services.NVIDIA
{
    enum NvDitherState
    {
        [Description("Auto")]
        Auto = 0,
        [Description("Enabled")]
        Enabled = 1,
        [Description("Disabled")]
        Disabled = 2
    }

    enum NvDitherBits
    {
        [Description("6-bit")]
        Bits6 = 0,
        [Description("8-bit")]
        Bits8 = 1,
        [Description("10-bit")]
        Bits10 = 2
    }

    enum NvDitherMode
    {
        [Description("Spatial Dynamic")]
        SpatialDynamic = 0,
        [Description("Spatial Static")]
        SpatialStatic = 1,
        [Description("Spatial Dynamic 2x2")]
        SpatialDynamic2x2 = 2,
        [Description("Spatial Static 2x2")]
        SpatialStatic2x2 = 3,
        [Description("Temporal")]
        Temporal = 4
    }

    class NvService : GraphicsService<NvPreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [DllImport(@"nvapi64", EntryPoint = @"nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl,
                    PreserveSig = true)]
        private static extern IntPtr NvAPI64_QueryInterface(uint interfaceId);

        public delegate int NvAPI_Disp_SetDitherControl(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint OutputId,
            [In] uint state,
            [In] uint bits,
            [In] uint mode
        );

        public delegate int NvAPI_Disp_GetDitherControl(
            [In] uint DisplayId,
            [MarshalAs(UnmanagedType.Struct)] ref NV_GPU_DITHER_CONTROL_V1 ditherControl);

        [StructLayout(LayoutKind.Sequential)]
        public struct NV_GPU_DITHER_CONTROL_V1
        {
            public uint version;
            public int state;
            public int bits;
            public int mode;
            public uint bitsCaps;
            public uint modeCaps;
        };

        public override string ServiceName => "NVIDIA";

        protected override string PresetsBaseFilename => "NvPresets.json";

        private Display _currentDisplay;
        private string _baseProfileName = "";
        private string _configFilename;
        public NvServiceConfig Config { get; private set; }

        private DrsSettingsService _drs;
        private DrsSettingsMetaService _meta;
        private List<SettingItem> _settings = new List<SettingItem>();

        private Semaphore _semaphore = new Semaphore(1, 1);

        public const uint DRS_GSYNC_APPLICATION_MODE = 294973784;
        public const uint DRS_VSYNC_CONTROL = 11041231;
        //public const uint DRS_ULTRA_LOW_LATENCY      = 0x10835000;
        public const uint DRS_PRERENDERED_FRAMES = 8102046;
        public const uint DRS_FRAME_RATE_LIMITER_V3 = 277041154;

        public const uint DRS_ANISOTROPIC_FILTERING_SETTING = 0x101E61A9;
        public const uint DRS_ANISOTROPIC_FILTER_OPTIMIZATION = 0x0084CD70;
        public const uint DRS_ANISOTROPIC_FILTER_SAMPLE_OPTIMIZATION = 0x00E73211;
        public const uint DRS_TEXTURE_FILTERING_QUALITY = 0x00CE2691;
        public const uint DRS_TEXTURE_FILTERING_NEGATIVE_LOD_BIAS = 0x0019BB68;

        public const uint DRS_RBAR_FEATURE = 0X000F00BA;
        public const uint DRS_RBAR_OPTIONS = 0X000F00BB;
        public const uint DRS_RBAR_SIZE_LIMIT = 0X000F00FF;

        private static readonly List<uint> _driverSettingIds = new()
        {
            DRS_GSYNC_APPLICATION_MODE,
            DRS_VSYNC_CONTROL,
            DRS_PRERENDERED_FRAMES,
            DRS_FRAME_RATE_LIMITER_V3,
            DRS_ANISOTROPIC_FILTERING_SETTING,
            DRS_ANISOTROPIC_FILTER_OPTIMIZATION,
            DRS_TEXTURE_FILTERING_QUALITY,
            DRS_TEXTURE_FILTERING_NEGATIVE_LOD_BIAS,
            DRS_ANISOTROPIC_FILTER_SAMPLE_OPTIMIZATION,
            DRS_RBAR_FEATURE,
            DRS_RBAR_OPTIONS,
            DRS_RBAR_SIZE_LIMIT
        };

        private static NvService ServiceInstance;

        public NvService(AppContextProvider appContextProvider) : base(appContextProvider)
        {
            NvPreset.NvService = this;

            AddJsonConverter(new ColorDataConverter());
            LoadConfig();
            LoadPresets();
        }

        public static async Task<bool> ExecutePresetAsync(string idOrName)
        {
            try
            {
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();

                var nvService = new NvService(appContextProvider);

                var result = await nvService.ApplyPreset(idOrName);

                if (!result)
                {
                    Console.WriteLine("Preset not found or error while executing.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing preset: " + ex.ToLogString());
                return false;
            }
        }

        public Display GetCurrentDisplay()
        {
            if (!HasDisplaysAttached())
            {
                _currentDisplay = null;
                return null;
            }

            if (_currentDisplay == null)
            {
                _currentDisplay = Display.GetDisplays()[0];
            }

            return _currentDisplay;
        }

        public override bool HasDisplaysAttached(bool reinitialize = false)
        {
            if (reinitialize)
            {
                try
                {
                    Initialize();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Could not reinitialize NVIDIA API: {ex.Message}");
                    return false;
                }
            }

            try
            {
                return DisplayDevice.GetGDIPrimaryDisplayDevice() != null;
            }
            catch (Exception)
            {
                return reinitialize ? false : HasDisplaysAttached(true);
            }
        }

        public void GlobalSave()
        {
            SavePresets();
            SaveConfig();
        }

        protected override List<NvPreset> GetDefaultPresets()
        {
            return NvPreset.GetDefaultPresets();
        }

        private void SavePresets()
        {
            Utils.WriteObject(_presetsFilename, _presets);
        }

        private void SetCurrentDisplay(NvPreset preset)
        {
            if (preset.primaryDisplay)
            {
                _currentDisplay = GetPrimaryDisplay();
            }
            else
            {
                _currentDisplay = Display.GetDisplays().FirstOrDefault(x => x.Name.Equals(preset.displayName));
            }
        }

        public Display GetPrimaryDisplay()
        {
            try
            {
                var displayId = DisplayDevice.GetGDIPrimaryDisplayDevice().DisplayId;
                return Display.GetDisplays().FirstOrDefault(x => x.DisplayDevice.DisplayId == displayId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while getting primary display device");

                return null;
            }
        }

        public async Task<bool> ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return await ApplyPreset(preset);
            }
            else
            {
                return false;
            }
        }

        public override async Task<bool> ApplyPreset(NvPreset preset)
        {
            var result = true;

            if (!HasDisplaysAttached())
            {
                return false;
            }

            SetCurrentDisplay(preset);

            var display = GetCurrentDisplay();

            if (display == null)
            {
                return false;
            }

            var hdrEnabled = IsHDREnabled();

            var newHdrEnabled = preset.applyHDR && (preset.HDREnabled || (preset.toggleHDR && !hdrEnabled));
            var applyHdr = preset.applyHDR && (preset.toggleHDR || preset.HDREnabled != hdrEnabled);

            if (preset.applyColorData && (ColorDataDiffers(preset.colorData) || (!newHdrEnabled && preset.applyColorData && preset.colorData.Colorimetry != ColorDataColorimetry.Auto)))
            {
                if (preset.applyRefreshRate || preset.applyResolution)
                {
                    var timing = display.DisplayDevice.CurrentTiming;
                    if ((preset.applyRefreshRate && preset.refreshRate < timing.Extra.RefreshRate) || (preset.applyResolution && preset.resolutionWidth < timing.HorizontalVisible))
                    {
                        SetMode(preset.applyResolution ? preset.resolutionWidth : 0, preset.applyResolution ? preset.resolutionHeight : 0, preset.applyRefreshRate ? preset.refreshRate : 0, true);
                    }
                }

                SetColorData(display, preset.colorData);
            }

            if (applyHdr)
            {
                SetHDRState(display, newHdrEnabled);
            }

            if (hdrEnabled)
            {
                //CCD.SetSDRWhiteLevel(1000, display.Name);
            }

            if (preset.applyRefreshRate || preset.applyResolution)
            {
                if (!SetMode(preset.applyResolution ? preset.resolutionWidth : 0, preset.applyResolution ? preset.resolutionHeight : 0, preset.applyRefreshRate ? preset.refreshRate : 0, true))
                {
                    result = false;
                }
            }

            if (preset.applyDithering)
            {
                if (!SetDithering(preset.ditheringEnabled ? NvDitherState.Enabled : NvDitherState.Disabled, preset: preset))
                {
                    result = false;
                }
            }

            if (preset.applyDriverSettings)
            {
                SetDriverSettings(preset.driverSettings);
            }

            if (preset.applyOverclocking)
            {
                result = ApplyOverclocking(preset.ocSettings);
            }

            _lastAppliedPreset = preset;

            PresetApplied();

            return result;
        }

        public bool ApplyOverclocking(NvGpuOcSettings setting)
        {
            return ApplyOverclocking(new List<NvGpuOcSettings> { setting });
        }

        public bool ApplyOverclocking(List<NvGpuOcSettings> settings)
        {
            if (!Utils.IsAdministrator())
            {
                var result = PipeUtils.SendMessage(new SvcNvOverclockingMessage
                {
                    OverclockingSettings = settings
                });

                if (result == null)
                {
                    return false;
                }

                return true;
            }

            var result2 = true;

            foreach (var ocSetting in settings)
            {
                var gpuInfo = NvGpuInfo.GetGpuInfo(ocSetting.PCIIdentifier);

                if (gpuInfo == null)
                {
                    continue;
                }

                result2 = gpuInfo.ApplyOcSettings(ocSetting);
            }

            return result2;
        }

        public bool SetColorData(Display display, ColorData colorData)
        {
            try
            {
                display.DisplayDevice.SetColorData(colorData);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"SetColorData threw an exception: {ex.Message}");
                return false;
            }
        }

        public static void ApplyDriverSettings(string profileName, List<KeyValuePair<uint, string>> settings)
        {
            CheckServiceInstance();

            ServiceInstance._drs.StoreSettingsToProfile(profileName, settings);
        }

        public static bool RestoreDriverSettings(string profileName, List<KeyValuePair<uint, string>> settings)
        {
            CheckServiceInstance();

            var result = false;

            foreach (var keyValuePair in settings)
            {
                var settingId = keyValuePair.Key;
                ServiceInstance._drs.ResetValue(profileName, settingId, out result);
            }

            return result;
        }

        public static NvService CheckServiceInstance()
        {
            if (ServiceInstance == null)
            {
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();

                ServiceInstance = new NvService(appContextProvider);
                ServiceInstance.Initialize();
                ServiceInstance.RefreshProfileSettings();
            }

            return ServiceInstance;
        }

        private void SetDriverSettings(Dictionary<uint, uint> settings)
        {
            var convertedSettings = new List<KeyValuePair<uint, string>>();

            foreach (var keyValue in settings)
            {
                var setting = ConvertDriverSetting(keyValue.Key, keyValue.Value);

                if (setting.Key == keyValue.Key)
                {
                    convertedSettings.Add(setting);
                }
            }

            _drs.StoreSettingsToProfile(_baseProfileName, convertedSettings);
        }

        public void SetDriverSetting(uint settingId, uint value)
        {
            SetDriverSettings(new Dictionary<uint, uint> { { settingId, value } });
        }

        private KeyValuePair<uint, string> ConvertDriverSetting(uint settingId, uint settingValue)
        {
            var convertedSetting = new KeyValuePair<uint, string>(0, string.Empty);

            var settingMeta = _meta.GetSettingMeta(settingId);

            if (settingMeta == null)
            {
                return convertedSetting;
            }

            if (settingMeta.SettingType == nspector.Native.NVAPI2.NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
            {
                var value = settingMeta.DwordValues?.FirstOrDefault(v => v.Value == settingValue);

                if (value != null)
                {
                    return new KeyValuePair<uint, string>(settingId, value.ValueName);
                }
            }
            else if (settingMeta.SettingType == nspector.Native.NVAPI2.NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
            {
                var value = "0x" + settingValue.ToString("x16");

                return new KeyValuePair<uint, string>(settingId, value);
            }

            return convertedSetting;
        }

        private ColorData GetCurrentColorData(Display display)
        {
            try
            {
                return display.DisplayDevice.CurrentColorData;
            }
            catch (Exception e)
            {
                Logger.Error("Error while reading current color data: " + e.Message);
                return new ColorData();
            }
        }

        private bool ColorDataDiffers(ColorData colorData)
        {
            var display = GetCurrentDisplay();

            var currentColorData = GetCurrentColorData(display);

            var settingRange = colorData.DynamicRange == ColorDataDynamicRange.Auto ? colorData.ColorFormat == ColorDataFormat.RGB ? ColorDataDynamicRange.VESA : ColorDataDynamicRange.CEA : colorData.DynamicRange;

            var settingSpace = colorData.Colorimetry;

            return colorData.ColorFormat != currentColorData.ColorFormat || colorData.ColorDepth != currentColorData.ColorDepth || settingRange != currentColorData.DynamicRange || settingSpace != currentColorData.Colorimetry;
        }

        public void SetHDRState(Display display, bool enabled)
        {
            CCD.SetHDRState(enabled, display.Name);
        }

        public bool SetDithering(NvDitherState state, uint bits = 1, uint mode = 4, NvPreset preset = null, Display currentDisplay = null)
        {
            var result = true;

            var ptr = NvAPI64_QueryInterface(0xDF0DFCDD);
            if (ptr != IntPtr.Zero)
            {
                var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_SetDitherControl)) as NvAPI_Disp_SetDitherControl;

                var display = currentDisplay ?? GetCurrentDisplay();
                if (display == null)
                {
                    return false;
                }

                var displayDevice = display.DisplayDevice;

                var gpuHandle = displayDevice.PhysicalGPU.Handle;
                var displayId = displayDevice.DisplayId;

                if (state == NvDitherState.Auto)
                {
                    // These seem to be ignored in Auto-state
                    bits = 0;
                    mode = 0;
                }
                else if (preset != null)
                {
                    bits = preset.ditheringBits;
                    mode = preset.ditheringMode;
                }

                var resultValue = delegateValue(gpuHandle, displayId, (uint)state, bits, mode);
                if (resultValue != 0)
                {
                    Logger.Error($"Could not set dithering because NvAPI_Disp_SetDitherControl returned a non-zero return code: {resultValue}");
                    result = false;
                }
            }
            else
            {
                Logger.Error($"Could not set dithering because the function NvAPI_Disp_SetDitherControl could not be found");
                result = false;
            }

            return result;
        }

        public NV_GPU_DITHER_CONTROL_V1 GetDithering(Display currentDisplay = null)
        {
            var dither = new NV_GPU_DITHER_CONTROL_V1 { version = 0x10018 };

            var ptr = NvAPI64_QueryInterface(0x932AC8FB);
            if (ptr != IntPtr.Zero)
            {
                var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_GetDitherControl)) as NvAPI_Disp_GetDitherControl;

                var display = currentDisplay ?? GetCurrentDisplay();
                if (display == null)
                {
                    dither.state = -1;
                    return dither;
                }

                var displayDevice = display.DisplayDevice;
                var displayId = displayDevice.DisplayId;

                var result = delegateValue(displayId, ref dither);
                if (result != 0)
                {
                    Logger.Error($"Could not get dithering because NvAPI_Disp_GetDitherControl returned a non-zero return code: {result}");
                    dither.state = -1;
                }
            }
            else
            {
                dither.state = -2;
            }

            return dither;
        }

        public bool SetMode(uint resolutionWidth = 0, uint resolutionHeight = 0, uint refreshRate = 0, bool updateRegistry = false)
        {
            var display = GetCurrentDisplay();
            if (display == null)
            {
                return false;
            }

            var desktopRect = display.DisplayDevice.ScanOutInformation.SourceDesktopRectangle;

            if (refreshRate == 0)
            {
                var timing = display.DisplayDevice.CurrentTiming;
                refreshRate = (uint)timing.Extra.RefreshRate;
            }

            if (resolutionWidth == 0)
            {
                resolutionWidth = (uint)desktopRect.Width;
                resolutionHeight = (uint)desktopRect.Height;
            }

            var portrait = IsDisplayInPortraitMode(display);

            return SetRefreshRateInternal(display.Name, (int)refreshRate, portrait, (int)resolutionWidth, (int)resolutionHeight, updateRegistry);
        }

        public List<uint> GetAvailableRefreshRates(NvPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay();
            if (display == null)
            {
                return new List<uint>();
            }

            var portrait = IsDisplayInPortraitMode(display);
            var desktopRect = display.DisplayDevice.ScanOutInformation.SourceDesktopRectangle;

            return GetAvailableRefreshRatesInternal(display.Name, portrait, desktopRect.Width, desktopRect.Height);
        }

        private bool IsDisplayInPortraitMode(NvAPIWrapper.Display.Display display)
        {
            try
            {
                return new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<DEVMODEA> GetAvailableResolutions(NvPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay();
            if (display == null)
            {
                return new List<DEVMODEA>();
            }

            var portrait = new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);
            var timing = display.DisplayDevice.CurrentTiming;

            return GetAvailableResolutionsInternal(display.Name, portrait, (uint)(timing.Extra.RefreshRate));
        }

        public bool IsHDREnabled(Display currentDisplay = null)
        {
            var display = currentDisplay ?? GetCurrentDisplay();
            if (display == null)
            {
                return false;
            }

            var displayDevice = display.DisplayDevice;
            var hdr = displayDevice.HDRColorData;
            return hdr?.HDRMode == ColorDataHDRMode.UHDA;
        }

        public Display[] GetDisplays()
        {
            return Display.GetDisplays();
        }

        public List<PhysicalGPU> GetGPUs()
        {
            var gpuHandles = GPUApi.EnumPhysicalGPUs();

            var gpus = gpuHandles.Select(h => new PhysicalGPU(h)).ToList();

            return gpus;
        }

        public List<NvDisplayInfo> GetSimpleDisplayInfos()
        {
            var displays = GetDisplays();
            var list = new List<NvDisplayInfo>();

            foreach (var display in displays)
            {
                var name = FormUtils.ExtendedDisplayName(display.Name);

                var displayInfo = new NvDisplayInfo(display, null, null, name);

                list.Add(displayInfo);
            }

            return list;
        }

        public List<NvDisplayInfo> GetDisplayInfos()
        {
            try
            {
                var displays = GetDisplays();
                var list = new List<NvDisplayInfo>();

                RefreshProfileSettings();
                var settings = GetVisibleSettings();

                foreach (var display in displays)
                {
                    var values = new List<string>
                    {
                        "Current settings"
                    };

                    var name = FormUtils.ExtendedDisplayName(display.Name);
                    values.Add(name);

                    var colorData = GetCurrentColorData(display);
                    var colorSettings = string.Format("{0}, {1}, {2}, {3}", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry);
                    values.Add(colorSettings);

                    var refreshRate = display.DisplayDevice.CurrentTiming.Extra.RefreshRate;
                    values.Add($"{refreshRate}Hz");

                    var mode = GetCurrentMode(display.Name);
                    values.Add($"{mode.dmPelsWidth}x{mode.dmPelsHeight}");

                    var ditherInfo = GetDithering(display);
                    string dithering;
                    if (ditherInfo.state == -1)
                    {
                        dithering = "Error";
                    }
                    else
                    {
                        var state = (NvDitherState)ditherInfo.state;
                        dithering = state switch { NvDitherState.Disabled => "Disabled", NvDitherState.Auto => "Auto: ", _ => string.Empty };
                        if (state is NvDitherState.Enabled or NvDitherState.Auto)
                        {
                            var ditherBitsDescription = ((NvDitherBits)ditherInfo.bits).GetDescription();
                            var ditherModeDescription = ((NvDitherMode)ditherInfo.mode).GetDescription();
                            dithering = string.Format("{0}{1} {2}", dithering, ditherBitsDescription, ditherModeDescription);
                        }
                    }

                    values.Add(dithering);

                    var hdrEnabled = IsHDREnabled(display);
                    values.Add(hdrEnabled ? "Yes" : "No");

                    var drsValues = new List<string>();

                    foreach (var setting in settings)
                    {
                        var settingMeta = GetSettingMeta(setting.SettingId);

                        if (settingMeta == null)
                        {
                            continue;
                        }

                        var value = settingMeta.ToFriendlyName(setting.ValueText);

                        if (value == null)
                        {
                            continue;
                        }

                        drsValues.Add($"{settingMeta.SettingName}: {value}");
                    }

                    values.Add(string.Join(", ", drsValues));

                    var infoLine = string.Format("{0}: {1}, {2}Hz, HDR: {3}", name, colorSettings, refreshRate, hdrEnabled ? "Yes" : "No");

                    var displayInfo = new NvDisplayInfo(display, values, infoLine, name);

                    list.Add(displayInfo);
                }

                return list;
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return null;
            }
        }

        public NvPreset GetPresetForDisplay(string displayName)
        {
            var displays = GetDisplays();

            var display = displays.FirstOrDefault(d => displayName.StartsWith(d.Name));

            if (display == null)
            {
                return null;
            }

            var isPrimaryDisplay = displays.Count() == 1 || display.DisplayDevice.DisplayId == DisplayDevice.GetGDIPrimaryDisplayDevice()?.DisplayId;
            var preset = new NvPreset { Display = display, applyColorData = false, primaryDisplay = isPrimaryDisplay };

            preset.displayName = FormUtils.ExtendedDisplayName(display.Name);
            preset.colorData = GetCurrentColorData(display);

            var mode = GetCurrentMode(display.Name);

            preset.refreshRate = mode.dmDisplayFrequency;
            preset.resolutionHeight = mode.dmPelsHeight;
            preset.resolutionWidth = mode.dmPelsWidth;

            var ditherInfo = GetDithering(display);

            if (ditherInfo.state == -1)
            {
                preset.ditheringEnabled = true;
                preset.ditheringBits = (uint)NvDitherBits.Bits8;
                preset.ditheringMode = (uint)NvDitherMode.Temporal;
            }
            else
            {
                preset.ditheringEnabled = ditherInfo.state != (int)NvDitherState.Disabled;
                preset.ditheringBits = (uint)ditherInfo.bits;
                preset.ditheringMode = (uint)ditherInfo.mode;
            }

            preset.HDREnabled = IsHDREnabled(display);

            if (Config.ShowOverclocking)
            {
                var gpus = GetGPUs();

                foreach (var gpu in gpus)
                {
                    var gpuInfo = NvGpuInfo.GetGpuInfo(gpu);

                    var settings = gpuInfo.GetOverclockSettings();

                    preset.ocSettings.Add(settings);
                }
            }

            return preset;
        }

        public List<SettingItem> GetVisibleSettings()
        {
            return GetSettings().Where(s => _driverSettingIds.Contains(s.SettingId)).ToList();
        }

        public SettingMeta GetSettingMeta(uint settingId)
        {
            return _meta.GetSettingMeta(settingId);
        }

        public bool IsGsyncEnabled()
        {
            var setting = GetSettings().FirstOrDefault(s => s.SettingId == DRS_GSYNC_APPLICATION_MODE);
            if (setting == null)
            {
                return false;
            }

            var settingMeta = GetSettingMeta(setting.SettingId);
            var settingValue = settingMeta.DwordValues?.FirstOrDefault(s => s.ValueName == setting.ValueText);

            return settingValue.Value >= 1;
        }

        protected List<SettingItem> GetSettings()
        {
            _semaphore.WaitOne();
            try
            {
                if (!_settings.Any())
                {
                    RefreshProfileSettings();
                }

                return _settings;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected override void Initialize()
        {
            NvAPIWrapper.NVIDIA.Initialize();
            _initialized = true;

            _drs = DrsServiceLocator.SettingService;
            _meta = DrsServiceLocator.MetaService;

            _drs.GetProfileNames(ref _baseProfileName, false);

            if (!Utils.IsAdministrator())
            {
                _drs.ApplySettings += HandleDrsApplySettings;
                _drs.RestoreSetting += HandleDrsRestoreSetting;
            }

            //SetGpuClocks();

            //Task.Run(() =>
            //{
            //    RefreshProfileSettings();
            //});
        }

        public void RefreshProfileSettings()
        {
            _semaphore.WaitOne();
            try
            {
                var firstTime = _settings?.Any() != true;

                DrsSessionScope.DestroyGlobalSession();

                var applications = default(Dictionary<string, string>);
                var normalSettings = _drs.GetSettingsForProfile(_baseProfileName, SettingViewMode.Normal, ref applications, true);

                _settings = new List<SettingItem>();
                _settings.AddRange(normalSettings);

                _driverSettingIds.Clear();
                _driverSettingIds.AddRange(_settings.Where(s => !s.IsStringValue && !s.SettingText.Contains("SLI")).Select(s => s.SettingId));

                if (firstTime)
                {
                    DrsServiceLocator.ScannerService.ScanProfileSettings(false, null);
                    _meta.ResetMetaCache();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private bool HasPrivilegesForUnexposedApis()
        {
            return Utils.IsAdministrator() || _appContextProvider.GetAppContext()?.Config.ElevationMethod != ElevationMethod.UseService || _appContextProvider.GetAppContext()?.IsServiceRunning() != true;
        }


        private void HandleDrsApplySettings(object sender, DrsEvent drsEvent)
        {
            if (HasPrivilegesForUnexposedApis())
            {
                return;
            }

            PipeUtils.SendMessage(new SvcNvDriverSettingsMessage { ProfileName = drsEvent.ProfileName, DriverSettings = drsEvent.Settings });

            drsEvent.Handled = true;
        }

        private void HandleDrsRestoreSetting(object sender, DrsEvent drsEvent)
        {
            if (HasPrivilegesForUnexposedApis())
            {
                return;
            }

            PipeUtils.SendMessage(new SvcNvDriverSettingsMessage(SvcMessageType.RestoreNvidiaDriverSetting) { ProfileName = drsEvent.ProfileName, DriverSettings = drsEvent.Settings });

            drsEvent.Handled = true;
        }

        protected override void Uninitialize()
        {
            if (_initialized)
            {
                NvAPIWrapper.NVIDIA.Unload();
            }
        }

        private void LoadConfig()
        {
            _configFilename = Path.Combine(_dataPath, "NvConfig.json");
            try
            {
                if (File.Exists(_configFilename))
                {
                    Config = JsonConvert.DeserializeObject<NvServiceConfig>(File.ReadAllText(_configFilename));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"LoadConfig: {ex.Message}");
            }
            Config ??= new NvServiceConfig();
        }

        private void SaveConfig()
        {
            Utils.WriteObject(_configFilename, Config);
        }
    }
}
