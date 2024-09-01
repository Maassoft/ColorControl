using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.NVIDIA;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json;
using novideo_srgb;
using nspector.Common;
using nspector.Common.Meta;
using NStandard;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces.Display;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using static ColorControl.Shared.Contracts.NVIDIA.NvHdrSettings;
using static ColorControl.Shared.Native.CCD;

namespace ColorControl.Services.NVIDIA
{
    class NvService : GraphicsService<NvPreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [DllImport(@"nvapi64", EntryPoint = @"nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl,
                    PreserveSig = true)]
        private static extern IntPtr NvAPI64_QueryInterface(uint interfaceId);

        [FunctionId(FunctionId.NvAPI_GPU_SetDitherControl)]
        public delegate int NvAPI_Disp_SetDitherControl(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint OutputId,
            [In] uint state,
            [In] uint bits,
            [In] uint mode
        );

        [FunctionId(FunctionId.NvAPI_GPU_GetDitherControl)]
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

        [FunctionId(FunctionId.NvAPI_Disp_SetOutputMode)]
        public delegate int NvAPI_Disp_SetOutputMode(
            [In] uint DisplayId,
            ref NV_DISPLAY_OUTPUT_MODE pDisplayMode
        );

        [FunctionId(FunctionId.NvAPI_Disp_GetOutputMode)]
        public delegate int NvAPI_Disp_GetOutputMode(
            [In] uint DisplayId,
            ref NV_DISPLAY_OUTPUT_MODE pDisplayMode
        );

        [FunctionId(FunctionId.NvAPI_RestartDisplayDriver)]
        public delegate void NvAPI_RestartDisplayDriver();

        // NvAPI_RestartDisplayDriver B4B26B65

        //public struct NV_HDR_METADATA_V1
        //{
        //    public uint version;                                          //!< Version of this structure

        //    public short displayPrimary_x0;                                //!< x coordinate of color primary 0 (e.g. Red) of mastering display ([0x0000-0xC350] = [0.0 - 1.0])
        //    public short displayPrimary_y0;                                //!< y coordinate of color primary 0 (e.g. Red) of mastering display ([0x0000-0xC350] = [0.0 - 1.0])

        //    public short displayPrimary_x1;                                //!< x coordinate of color primary 1 (e.g. Green) of mastering display ([0x0000-0xC350] = [0.0 - 1.0])
        //    public short displayPrimary_y1;                                //!< y coordinate of color primary 1 (e.g. Green) of mastering display ([0x0000-0xC350] = [0.0 - 1.0])

        //    public short displayPrimary_x2;                                //!< x coordinate of color primary 2 (e.g. Blue) of mastering display ([0x0000-0xC350] = [0.0 - 1.0])
        //    public short displayPrimary_y2;                                //!< y coordinate of color primary 2 (e.g. Blue) of mastering display ([0x0000-0xC350] = [0.0 - 1.0])

        //    public short displayWhitePoint_x;                              //!< x coordinate of white point of mastering display ([0x0000-0xC350] = [0.0 - 1.0])
        //    public short displayWhitePoint_y;                              //!< y coordinate of white point of mastering display ([0x0000-0xC350] = [0.0 - 1.0])

        //    public short max_display_mastering_luminance;                  //!< Maximum display mastering luminance ([0x0000-0xFFFF] = [0.0 - 65535.0] cd/m^2, in units of 1 cd/m^2)
        //    public short min_display_mastering_luminance;                  //!< Minimum display mastering luminance ([0x0000-0xFFFF] = [0.0 - 6.55350] cd/m^2, in units of 0.0001 cd/m^2)

        //    public short max_content_light_level;                          //!< Maximum Content Light level (MaxCLL) ([0x0000-0xFFFF] = [0.0 - 65535.0] cd/m^2, in units of 1 cd/m^2)
        //    public short max_frame_average_light_level;                    //!< Maximum Frame-Average Light Level (MaxFALL) ([0x0000-0xFFFF] = [0.0 - 65535.0] cd/m^2, in units of 1 cd/m^2)
        //}

        //public delegate int NvAPI_Disp_GetSourceHdrMetadata(
        //    [In] uint DisplayId,
        //    [MarshalAs(UnmanagedType.Struct)] ref NV_HDR_METADATA_V1 pMetadata,
        //    [In] long sourcePID
        //);

        public override string ServiceName => "NVIDIA";

        protected override string PresetsBaseFilename => "NvPresets.json";

        private Display _currentDisplay;
        private string _baseProfileName = "";
        private string _configFilename;
        public NvServiceConfig Config { get; private set; }
        private NV_DISPLAY_OUTPUT_MODE? Hdr10OutputModeForced { get; set; }

        private DrsSettingsService _drs;
        private DrsSettingsMetaService _meta;
        private List<SettingItem> _settings = new List<SettingItem>();

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _refreshSemaphore = new SemaphoreSlim(1, 1);

        public uint DriverVersion { get; private set; }
        public bool OutputModeAvailable => DriverVersion >= 52500;

        private static readonly List<uint> _driverSettingIds = new();
        private readonly WinApiService _winApiService;
        private readonly RpcClientService _rpcClientService;
        private readonly PowerEventDispatcher _powerEventDispatcher;
        private readonly ServiceManager _serviceManager;
        private static NvService ServiceInstance;

        public static readonly int SHORTCUTID_NVQA = -200;

        public NvService(GlobalContext globalContext, WinApiService winApiService, RpcClientService rpcClientService, PowerEventDispatcher powerEventDispatcher, ServiceManager serviceManager) : base(globalContext)
        {
            _winApiService = winApiService;
            _rpcClientService = rpcClientService;
            _powerEventDispatcher = powerEventDispatcher;
            _serviceManager = serviceManager;
            _rpcClientService.Name = nameof(NvService);
            NvPreset.GetDriverSettingsDescription += GetDriverSettingsDescription;

            AddJsonConverter(new ColorDataConverter());
            LoadConfig();
            LoadPresets();
        }

        public override void InstallEventHandlers()
        {
            SetShortcuts(SHORTCUTID_NVQA, Config.QuickAccessShortcut);

            GetDisplayInfos(false);

            MainViewModel.ConfigPath = _globalContext.DataPath;
            if (Config.ApplyNovideoOnStartup)
            {
                MainWindow.CreateAndShow(false);
            }

            // TODO: implement later
            //_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_Suspend, PowerModeChanged);
            //_powerEventDispatcher.RegisterAsyncEventHandler(PowerEventDispatcher.Event_Resume, PowerModeResume);
            //_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_Shutdown, PowerModeChanged);
        }

        public override List<string> GetInfo()
        {
            return [$"{_presets.Count} presets"];
        }

        public static async Task<bool> ExecutePresetAsync(string idOrName)
        {
            try
            {
                var nvService = Program.ServiceProvider.GetRequiredService<NvService>();

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

        public Display GetCurrentDisplay(bool checkDisplays = true)
        {
            if (checkDisplays && !HasDisplaysAttached())
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

        private Display SetCurrentDisplay(NvPreset preset)
        {
            _currentDisplay = GetPresetDisplay(preset);

            return _currentDisplay;
        }

        private Display GetPresetDisplay(NvPreset preset)
        {
            if (preset.Display != null)
            {
                return preset.Display;
            }

            if (preset.primaryDisplay)
            {
                return GetPrimaryDisplay();
            }

            var displays = GetSimpleDisplayInfos();

            return displays.FirstOrDefault(d => d.DisplayId == preset.DisplayId)?.Display;
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

        public bool UpdatePreset(NvPreset specPreset)
        {
            var currentPreset = _presets.FirstOrDefault(p => p.id == specPreset.id);

            if (currentPreset != null)
            {
                currentPreset.name = specPreset.name;
                currentPreset.shortcut = specPreset.shortcut;
                currentPreset.Update(specPreset);

                SavePresets();

                return true;
            }

            var newPreset = new NvPreset(specPreset);
            newPreset.name = specPreset.name;
            newPreset.shortcut = specPreset.shortcut;

            _presets.Add(newPreset);

            SavePresets();

            return true;
        }

        private async Task<Display> WaitForDisplayAsync(NvPreset preset)
        {
            var attempt = 0;
            Display display = null;

            while (attempt < 20)
            {
                if (HasDisplaysAttached())
                {
                    display = SetCurrentDisplay(preset);
                }

                if (display != null)
                {
                    break;
                }

                if (preset.IsStartupPreset)
                {
                    await Task.Delay(1000);
                    attempt++;
                }
                else
                {
                    break;
                }
            }

            return display;
        }

        public override async Task<bool> ApplyPreset(NvPreset preset)
        {
            var result = true;

            var display = await WaitForDisplayAsync(preset);

            if (display == null)
            {
                Logger.Warn($"Cannot apply preset {preset.IdOrName} because the associated display is not active/found");

                return false;
            }

            var hdrEnabled = IsHDREnabled();

            var newHdrEnabled = preset.applyHDR && (preset.HDREnabled || (preset.toggleHDR && !hdrEnabled));
            var applyHdr = preset.applyHDR && (preset.toggleHDR || preset.HDREnabled != hdrEnabled);

            if (preset.applyColorData && (ColorDataDiffers(preset.colorData) || (!newHdrEnabled && preset.applyColorData && preset.colorData.Colorimetry != ColorDataColorimetry.Auto)))
            {
                if (preset.DisplayConfig.ApplyRefreshRate || preset.DisplayConfig.ApplyResolution)
                {
                    var timing = display.DisplayDevice.CurrentTiming;
                    if ((preset.DisplayConfig.ApplyRefreshRate && preset.DisplayConfig.RefreshRate.MilliValue < timing.Extra.FrequencyInMillihertz) ||
                        (preset.DisplayConfig.ApplyResolution && preset.DisplayConfig.Resolution.ActiveWidth < timing.HorizontalVisible))
                    {
                        SetMode(preset.DisplayConfig, true);
                    }
                }

                SetColorData(display, preset.colorData);
            }

            if (preset.applyHDR)
            {
                if (applyHdr)
                {
                    if (newHdrEnabled && !hdrEnabled)
                    {
                        MainWindow.BeforeDisplaySettingsChange();
                    }

                    SetHDRState(display, newHdrEnabled);
                }

                if (newHdrEnabled && preset.HdrSettings.OutputMode.HasValue)
                {
                    SetOutputMode(preset.HdrSettings.OutputMode.Value, display);
                }

                if (preset.SDRBrightness.HasValue)
                {
                    SetSDRBrightness(display.Name, preset.SDRBrightness.Value);
                }
            }

            if (preset.DisplayConfig.ApplyRefreshRate || preset.DisplayConfig.ApplyResolution)
            {
                if (!SetMode(preset.DisplayConfig, true))
                {
                    result = false;
                }
            }

            if (preset.applyOther)
            {
                if (preset.ColorProfileSettings.ProfileName != null)
                {
                    CCD.SetDisplayDefaultColorProfile(display.Name, preset.ColorProfileSettings.ProfileName, _globalContext.Config.SetMinTmlAndMaxTml);
                }

                if (preset.scaling.HasValue)
                {
                    SetScaling(display, preset.scaling.Value);
                }
            }

            if (preset.applyHdmiSettings)
            {
                SetHdmiSettings(display, preset.HdmiInfoFrameSettings);
            }

            if (preset.applyDithering)
            {
                if (!SetDithering(preset.DitherState, preset: preset))
                {
                    result = false;
                }
            }

            if (preset.ApplyColorEnhancements)
            {
                SetDigitalVibranceLevel(display, preset.ColorEnhancementSettings.DigitalVibranceLevel);
                SetHueAngle(display, preset.ColorEnhancementSettings.HueAngle);
            }

            if (preset.applyDriverSettings)
            {
                SetDriverSettings(preset.driverSettings);
            }

            if (preset.ApplyNovideoSettings && !newHdrEnabled)
            {
                SetNovideoSettings(preset.DisplayId, preset.NovideoSettings);
            }

            if (preset.applyOverclocking)
            {
                result = ApplyOverclocking(preset.ocSettings);
            }

            _lastAppliedPreset = preset;

            preset.IsStartupPreset = false;

            PresetApplied();
            GetDisplayInfos(false);

            return result;
        }

        private void SetNovideoSettings(string displayId, NovideoSettings novideoSettings)
        {
            MainWindow.ApplySettings(displayId, novideoSettings.ApplyClamp, (int)novideoSettings.ColorSpace);
        }

        public bool ApplyOverclocking(List<NvGpuOcSettings> settings)
        {
            if (!_winApiService.IsAdministrator())
            {
                var message = new SvcRpcSetNvOverclockingMessage
                {
                    MethodName = nameof(ApplyOverclocking),
                    OverclockingSettings = settings
                };

                var result = _rpcClientService.Call<object>(message);

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

        public void ApplyDriverSettings(string profileName, List<KeyValuePair<uint, string>> settings)
        {
            RefreshProfileSettings();

            _drs.StoreSettingsToProfile(profileName, settings);
        }

        public bool RestoreDriverSettings(string profileName, List<KeyValuePair<uint, string>> settings)
        {
            var result = false;

            foreach (var keyValuePair in settings)
            {
                var settingId = keyValuePair.Key;
                _drs.ResetValue(profileName, settingId, out result);
            }

            return result;
        }

        public bool SetDriverSettings(Dictionary<uint, uint> settings, string profileName = null)
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

            _drs.StoreSettingsToProfile(profileName ?? _baseProfileName, convertedSettings);

            return true;
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

            return currentColorData.IsDifferent(colorData);
        }

        public void SetHDRState(Display display, bool enabled)
        {
            if (!enabled && Hdr10OutputModeForced.HasValue)
            {
                SetOutputMode(NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_SDR, display);
                Hdr10OutputModeForced = null;
            }

            CCD.SetHDRState(enabled, display.Name);
        }

        public void SetDigitalVibranceLevel(Display display, int level)
        {
            var dvControl = display.DigitalVibranceControl;

            dvControl.CurrentLevel = level;
        }

        public int GetDigitalVibranceLevel(Display display)
        {
            return display.DigitalVibranceControl.CurrentLevel;
        }

        public void SetHueAngle(Display display, int angle)
        {
            display.HUEControl.CurrentAngle = angle;
        }

        public int GetHueAngle(Display display)
        {
            return display.HUEControl.CurrentAngle;
        }

        //public NV_HDR_METADATA_V1? GetHdrMetaData(Display display = null)
        //{
        //    var ptr = NvAPI64_QueryInterface(0x0D3F52DA);
        //    if (ptr != IntPtr.Zero)
        //    {
        //        var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_GetSourceHdrMetadata)) as NvAPI_Disp_GetSourceHdrMetadata;

        //        display ??= GetCurrentDisplay();

        //        var displayDevice = display.DisplayDevice;

        //        var displayId = displayDevice.DisplayId;

        //        var metadata = new NV_HDR_METADATA_V1();
        //        metadata.version = MAKE_NVAPI_VERSION<NV_HDR_METADATA_V1>(1);

        //        var processId = Process.GetCurrentProcess().Id;

        //        var resultValue = delegateValue(displayId, ref metadata, processId);

        //        return resultValue == 0 ? metadata : null;
        //    }

        //    return null;
        //}

        public bool SetOutputMode(NV_DISPLAY_OUTPUT_MODE outputMode, Display display = null)
        {
            var delegateValue = DelegateFactory.GetDelegate<NvAPI_Disp_SetOutputMode>();

            if (delegateValue == null)
            {
                return false;
            }

            display ??= GetCurrentDisplay();

            var displayDevice = display.DisplayDevice;

            var displayId = displayDevice.DisplayId;

            var newOutputMode = outputMode;
            var previousOutputMode = Hdr10OutputModeForced;

            var resultValue = delegateValue(displayId, ref outputMode);

            if (resultValue == 0)
            {
                Hdr10OutputModeForced = newOutputMode > NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_SDR ? newOutputMode : null;
            }

            if (previousOutputMode == NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING && newOutputMode == NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10)
            {
                // HDR10+ will not be disabled properly when setting output mode to HDR10, just disable and enable HDR again
                SetHDRState(display, false);
                SetHDRState(display, true);
            }

            return resultValue == 0;
        }

        public NV_DISPLAY_OUTPUT_MODE? GetOutputMode(Display display = null)
        {
            var delegateValue = DelegateFactory.GetDelegate<NvAPI_Disp_GetOutputMode>();

            if (delegateValue == null)
            {
                return null;
            }


            display ??= GetCurrentDisplay();

            var displayDevice = display.DisplayDevice;

            var displayId = displayDevice.DisplayId;

            var outputMode = NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_SDR;

            var resultValue = delegateValue(displayId, ref outputMode);

            return resultValue == 0 ? outputMode : null;
        }

        public bool SetDitherRegistryKey(string displayRegId, uint state, uint bits = 1, uint mode = 4)
        {
            if (!_winApiService.IsAdministrator())
            {
                var result = _rpcClientService.Call<bool>(nameof(SetDitherRegistryKey), displayRegId, state, bits, mode);

                return result;
            }

            const string DOMAIN_ALIAS_RID_ADMINS = "S-1-5-32-544";

            var registryPath = "SYSTEM\\CurrentControlSet\\Services\\nvlddmkm\\State\\DisplayDatabase";

            var displayDbKey = Registry.LocalMachine.OpenSubKey(registryPath);

            var subKeyNames = displayDbKey.GetSubKeyNames().Where(n => n.Contains(displayRegId)).ToList();

            foreach (var subKeyName in subKeyNames)
            {
                var keyPath = $"{registryPath}\\{subKeyName}";

                try
                {
                    var subKey = Registry.LocalMachine.OpenSubKey(keyPath);

                    var ditherValueName = subKey.GetValueNames().FirstOrDefault(n => n == "DitherRegistryKey");

                    if (ditherValueName == null)
                    {
                        continue;
                    }

                    subKey = Registry.LocalMachine.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions);

                    var accessControl = subKey.GetAccessControl();

                    var rules = accessControl.GetAccessRules(true, true, typeof(SecurityIdentifier));
                    var accessRules = new RegistryAccessRule[rules.Count];
                    rules.CopyTo(accessRules, 0);

                    if (!accessRules.Any(r => r.RegistryRights == RegistryRights.FullControl && r.IdentityReference.Value == DOMAIN_ALIAS_RID_ADMINS && r.AccessControlType == AccessControlType.Allow))
                    {
                        var ownerIdentity = accessControl.GetOwner(typeof(SecurityIdentifier));

                        var adminFullControlRule = new RegistryAccessRule(ownerIdentity, RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);

                        accessControl.SetAccessRule(adminFullControlRule);

                        subKey.SetAccessControl(accessControl);
                    }

                    subKey = Registry.LocalMachine.OpenSubKey(keyPath, true);

                    var value = subKey.GetValue(ditherValueName) as byte[];

                    if (value == null)
                    {
                        continue;
                    }

                    value[9] = (byte)state;
                    value[10] = (byte)bits;
                    value[11] = (byte)mode;

                    // Checksum
                    value[12] = (byte)value.Take(12).Sum(v => (uint)v);

                    subKey.SetValue(ditherValueName, value, RegistryValueKind.Binary);

                    break;
                }
                catch (Exception ex)
                {
                    Logger.Debug($"Cannot get access to registry key: {keyPath} because: {ex.Message}");
                }
            }

            return true;
        }

        public bool SetDithering(NvDitherState state, uint bits = 1, uint mode = 4, NvPreset preset = null, Display currentDisplay = null, bool setRegistryKey = false, bool restartDriver = false)
        {
            var result = true;

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

                setRegistryKey |= preset.SetDitherRegistryKey;
                restartDriver |= preset.RestartDriver;
            }

            if (setRegistryKey)
            {
                var info = CCD.GetDisplayInfo(display.Name);
                var regId = info?.DisplayId;

                if (regId.IsNullOrEmpty())
                {
                    return false;
                }

                result = SetDitherRegistryKey(regId, (uint)state, bits, mode);

                if (restartDriver)
                {
                    RestartDriver();
                }
            }
            else
            {
                var delegateValue = DelegateFactory.GetDelegate<NvAPI_Disp_SetDitherControl>();

                if (delegateValue == null)
                {
                    return false;
                }

                var resultValue = delegateValue(gpuHandle, displayId, (uint)state, bits, mode);
                if (resultValue != 0)
                {
                    Logger.Error($"Could not set dithering because NvAPI_Disp_SetDitherControl returned a non-zero return code: {resultValue}");
                    result = false;
                }
            }

            return result;
        }

        public bool RestartDriver()
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcClientService.Call<bool>(nameof(RestartDriver));
            }

            var restartDelegate = DelegateFactory.GetDelegate<NvAPI_RestartDisplayDriver>();

            if (restartDelegate != null)
            {
                restartDelegate();

                return true;
            }

            return false;
        }

        public NV_GPU_DITHER_CONTROL_V1 GetDithering(Display currentDisplay = null)
        {
            var version = MAKE_NVAPI_VERSION<NV_GPU_DITHER_CONTROL_V1>(1);
            var dither = new NV_GPU_DITHER_CONTROL_V1 { version = version };

            var delegateValue = DelegateFactory.GetDelegate<NvAPI_Disp_GetDitherControl>();

            if (delegateValue == null)
            {
                dither.state = -2;
                return dither;
            }

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

            return dither;
        }

        public bool SetMode(VirtualResolution resolution = null, Rational refreshRate = null, bool updateRegistry = false, NvPreset preset = null)
        {
            var display = preset == null ? GetCurrentDisplay() : GetPresetDisplay(preset);
            if (display == null)
            {
                return false;
            }

            return SetMode(display.Name, resolution, refreshRate, updateRegistry);
        }

        public bool SetMode(DisplayConfig displayConfig, bool updateRegistry = false, NvPreset preset = null)
        {
            var display = preset == null ? GetCurrentDisplay() : GetPresetDisplay(preset);
            if (display == null)
            {
                return false;
            }

            return SetMode(display.Name, displayConfig, updateRegistry);
        }

        public List<Rational> GetAvailableRefreshRatesV2(NvPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay(false);
            if (display == null)
            {
                return [];
            }

            var desktopRect = preset?.DisplayConfig.ApplyResolution == true && preset?.DisplayConfig.Resolution.ActiveWidth > 0 ?
                new Rectangle(0, 0, (int)preset.DisplayConfig.Resolution.ActiveWidth, (int)preset.DisplayConfig.Resolution.ActiveHeight) :
                GetDesktopRect(display);

            return GetAvailableRefreshRatesV2(display.Name, desktopRect.Width, desktopRect.Height);
        }

        public List<VirtualResolution> GetAvailableResolutionsV2(NvPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay(false);
            if (display == null)
            {
                return [];
            }

            return GetAvailableResolutionsInternalV2(display.Name);
        }

        private Rectangle GetDesktopRect(Display display)
        {
            try
            {
                return display.DisplayDevice.ScanOutInformation.SourceDesktopRectangle;
            }
            catch (Exception)
            {
                var timing = display.DisplayDevice.CurrentTiming;

                return new Rectangle(0, 0, timing.HorizontalActive, timing.VerticalActive);
            }
        }

        private bool IsDisplayInPortraitMode(Display display)
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

        public bool IsHDREnabled(Display currentDisplay = null)
        {
            var display = currentDisplay ?? GetCurrentDisplay();
            if (display == null)
            {
                return false;
            }

            var displayDevice = display.DisplayDevice;
            var hdr = Logger.Swallow(() => displayDevice.HDRColorData?.HDRMode == ColorDataHDRMode.UHDA);
            return hdr;
        }

        public void SetHdmiSettings(Display display, NvHdmiInfoFrameSettings settings)
        {
            var displayDevice = display.DisplayDevice;

            var info = displayDevice.HDMIVideoFrameOverrideInformation ?? displayDevice.HDMIVideoFrameCurrentInformation;

            if (!info.HasValue)
            {
                return;
            }

            var infoValue = info.Value;

            var newInfo = new InfoFrameVideo(
                infoValue.VideoIdentificationCode ?? 0xFF,
                InfoFrameVideoPixelRepetition.None,
                settings.ColorFormat ?? infoValue.ColorFormat,
                settings.Colorimetry ?? infoValue.Colorimetry,
                settings.ExtendedColorimetry ?? infoValue.ExtendedColorimetry ?? InfoFrameVideoExtendedColorimetry.Auto,
                settings.RGBQuantization ?? infoValue.RGBQuantization,
                settings.YCCQuantization ?? infoValue.YCCQuantization,
                settings.ContentMode ?? infoValue.ContentMode,
                settings.ContentType ?? infoValue.ContentType,
                InfoFrameVideoScanInfo.NoData,
                InfoFrameBoolean.Auto,
                InfoFrameVideoAspectRatioActivePortion.Auto,
                InfoFrameVideoAspectRatioCodedFrame.Auto,
                InfoFrameVideoNonUniformPictureScaling.NoData,
                InfoFrameVideoBarData.NotPresent,
                null,
                null,
                null,
                null
                );

            displayDevice.SetHDMIVideoFrameInformation(newInfo, true);
        }

        public void SetHDMIContentType(Display display, InfoFrameVideoContentType contentType)
        {
            var displayDevice = display.DisplayDevice;

            var info = displayDevice.HDMIVideoFrameCurrentInformation;

            if (info.HasValue)
            {
                var infoValue = info.Value;

                infoValue.SetContentType(contentType);

                displayDevice.SetHDMIVideoFrameInformation(infoValue, true);
            }
        }

        public InfoFrameVideoContentType GetHDMIContentType(Display display)
        {
            var info = GetInfoFrameVideo(display);

            return info.HasValue ? info.Value.ContentType : InfoFrameVideoContentType.Auto;
        }

        public NvHdmiInfoFrameSettings GetHdmiSettings(Display display)
        {
            var info = GetInfoFrameVideo(display);

            if (!info.HasValue)
            {
                return new NvHdmiInfoFrameSettings();
            }

            var infoValue = info.Value;

            return new NvHdmiInfoFrameSettings
            {
                ColorFormat = infoValue.ColorFormat,
                Colorimetry = infoValue.Colorimetry,
                ExtendedColorimetry = infoValue.ExtendedColorimetry,
                RGBQuantization = infoValue.RGBQuantization,
                YCCQuantization = infoValue.YCCQuantization,
                ContentMode = infoValue.ContentMode,
                ContentType = infoValue.ContentType
            };
        }

        private static InfoFrameVideo? GetInfoFrameVideo(Display display)
        {
            var displayDevice = display.DisplayDevice;

            if (displayDevice.ConnectionType != NvAPIWrapper.Native.GPU.MonitorConnectionType.HDMI)
            {
                return null;
            }

            return Logger.Swallow(() => displayDevice.HDMIVideoFrameOverrideInformation) ?? Logger.Swallow(() => displayDevice.HDMIVideoFrameCurrentInformation);
        }

        public Scaling GetScaling(Display display)
        {
            var pathTargetInfo = GetTargetInfoForDisplay(display);

            if (pathTargetInfo?.Details == null)
            {
                return Scaling.Default;
            }

            var details = pathTargetInfo.Details.Value;

            return details.Scaling;
        }

        public void SetScaling(Display display, Scaling scaling)
        {
            var pathInfos = DisplayApi.GetDisplayConfig().ToList();

            var pathInfo = pathInfos.FirstOrDefault(c => c.TargetsInfo.Any(ti => ti.DisplayId == display.DisplayDevice.DisplayId));
            var pathInfoIndex = pathInfos.IndexOf(pathInfo);

            var pathTargetInfo = pathInfo.TargetsInfo.FirstOrDefault(ti => ti.DisplayId == display.DisplayDevice.DisplayId);

            if (pathTargetInfo?.Details == null)
            {
                return;
            }

            var details = pathTargetInfo.Details.Value;

            if (details.Scaling == scaling)
            {
                return;
            }

            details.Scaling = scaling;

            var newPathTargetInfo = new PathTargetInfoV2(pathTargetInfo.DisplayId, details);

            var newPathInfo = new PathInfoV2(new[] { newPathTargetInfo }, pathInfo.SourceModeInfo);

            var newPathInfos = pathInfos.ToArray();
            newPathInfos[pathInfoIndex] = newPathInfo;

            DisplayApi.SetDisplayConfig(newPathInfos, DisplayConfigFlags.SaveToPersistence);
        }

        private IPathTargetInfo? GetTargetInfoForDisplay(Display display)
        {
            var pathInfos = DisplayApi.GetDisplayConfig();

            var pathInfo = pathInfos.FirstOrDefault(c => c.TargetsInfo.Any(ti => ti.DisplayId == display.DisplayDevice.DisplayId));

            if (pathInfo == null)
            {
                return null;
            }

            var pathTargetInfo = pathInfo.TargetsInfo.FirstOrDefault(ti => ti.DisplayId == display.DisplayDevice.DisplayId);

            return pathTargetInfo;
        }

        private IPathInfo? GetPathInfoForDisplay(Display display)
        {
            var pathInfos = DisplayApi.GetDisplayConfig();

            return pathInfos.FirstOrDefault(c => c.TargetsInfo.Any(ti => ti.DisplayId == display.DisplayDevice.DisplayId));
        }

        public void SetColorProfile(Display display, string name)
        {
            CCD.SetDisplayDefaultColorProfile(display.Name, name, _globalContext.Config.SetMinTmlAndMaxTml);
        }

        public void TestResolution()
        {
            //CCD.SetDisplayConfig();

            //var dxWrapper = new DXWrapper();

            //dxWrapper.Test();

            //var display = GetCurrentDisplay();

            //var configs = DisplayApi.GetDisplayConfig();

            //var displayDevice = display.DisplayDevice;

            //var output = displayDevice.Output;
            //var bytes = File.ReadAllBytes(@"d:\S95C_mod.bin");

            //displayDevice.PhysicalGPU.WriteEDIDData(output, bytes);

            //SetDithering(NvDitherState.Enabled, 1, 4, setRegistryKey: true);

            //GetHdrMetaData();

            //SetOutputMode(NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING);

            //var display = GetCurrentDisplay();

            //CCD.GetUsePerUserDisplayProfiles(display.Name);

            //CCD.GetDisplayColorProfiles(display.Name);

            //CCD.InstallColorProfile(@"H:\srgb2000.icm");

            //CCD.AddDisplayColorProfile(display.Name, "srgb2000.icm");

            //CCD.SetDisplayDefaultColorProfile(display.Name, "srgb2000.icm");

            //var displayDevice = display.DisplayDevice;

            //var config = DisplayApi.GetDisplayConfig();

            //var config1 = config[0];

            //var pathInfo1 = config1.TargetsInfo.First();

            //var details = pathInfo1.Details.Value;

            //var newDetails = new PathAdvancedTargetInfo(details.Rotation, Scaling.GPUScanOutToNative);

            //var newPathTargetInfo = new PathTargetInfoV2(pathInfo1.DisplayId, newDetails);

            //var newPathInfo = new PathInfoV2(new[] { newPathTargetInfo }, config1.SourceModeInfo);

            //DisplayApi.SetDisplayConfig(new IPathInfo[] { newPathInfo }, DisplayConfigFlags.SaveToPersistence);

            //var info = DisplayApi.GetScanOutConfiguration(displayDevice.DisplayId);

            //var vertices = new float[] {
            //    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
            //    0.0f, 2160.0f, 0.0f, 2160.0f, 0.0f, 0.0f,
            //    3840.0f, 0.0f, 3840.0f, 0.0f, 0.0f, 0.0f,
            //    3840.0f, 2160.0f, 3840.0f, 2160.0f, 0.0f, 0.0f,
            //};

            //var warp = new ScanOutWarpingV1(WarpingVerticeFormat.TriangleStripXYUVRQ, vertices, info.TargetViewPortRectangle);
            //var verticesCount = 4;
            //var isSticky = false;

            //DisplayApi.SetScanOutWarping(displayDevice.DisplayId, warp, ref verticesCount, out isSticky);

            //var timing = displayDevice.CurrentTiming;

            //var res = new CustomResolution(3840, 2160, ColorFormat.A8R8G8B8, timing);

            //displayDevice.TrialCustomResolution(res);

            //var hdrColorData = displayDevice.HDRColorData;
            //var mastering = hdrColorData.MasteringDisplayData;

            //var newMastering = new MasteringDisplayColorData(
            //    new ColorDataColorCoordinate(0.2f, 0.2f),
            //    mastering.SecondColorCoordinate,
            //    mastering.ThirdColorCoordinate,
            //    mastering.WhiteColorCoordinate,
            //    120,
            //    mastering.MinimumMasteringLuminance,
            //    mastering.MaximumContentLightLevel,
            //    mastering.MaximumFrameAverageLightLevel);

            //var newHdrColorData = new HDRColorData(hdrColorData.HDRMode, newMastering);

            //displayDevice.SetHDRColorData(newHdrColorData);

            //var info = displayDevice.HDMIVideoFrameOverrideInformation ?? displayDevice.HDMIVideoFrameCurrentInformation;

            //if (info.HasValue)
            //{
            //    var infoValue = info.Value;

            //    var newInfo = new InfoFrameVideo(
            //        infoValue.VideoIdentificationCode.Value,
            //        InfoFrameVideoPixelRepetition.None,
            //        InfoFrameVideoColorFormat.RGB,
            //        InfoFrameVideoColorimetry.UseExtendedColorimetry,
            //        InfoFrameVideoExtendedColorimetry.BT2020,
            //        infoValue.RGBQuantization,
            //        infoValue.YCCQuantization,
            //        InfoFrameVideoITC.ITContent,
            //        infoValue.ContentType,
            //        InfoFrameVideoScanInfo.NoData,
            //        InfoFrameBoolean.Auto,
            //        InfoFrameVideoAspectRatioActivePortion.Auto,
            //        InfoFrameVideoAspectRatioCodedFrame.Auto,
            //        InfoFrameVideoNonUniformPictureScaling.NoData,
            //        InfoFrameVideoBarData.NotPresent,
            //        null,
            //        null,
            //        null,
            //        null
            //        );

            //    displayDevice.SetHDMIVideoFrameInformation(newInfo, true);
            //}
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
                var displayInfo = new NvDisplayInfo(display, null, null);

                list.Add(displayInfo);
            }

            return list;
        }

        public List<NvDisplayInfo> GetDisplayInfos(bool refreshSettings = true)
        {
            try
            {
                var list = new List<NvDisplayInfo>();

                if (refreshSettings)
                {
                    RefreshProfileSettings();
                }

                var displayInfos = GetSimpleDisplayInfos();

                foreach (var displayInfo in displayInfos)
                {
                    var preset = GetPresetForDisplay(displayInfo, true, displayInfos.Count());
                    var values = preset.GetDisplayValues(_globalContext.Config);

                    displayInfo.Values = values;
                    displayInfo.InfoLine = preset.InfoLine;

                    list.Add(displayInfo);
                }

                var notifyIconManager = _globalContext.ServiceProvider.GetService<NotifyIconManager>();

                if (notifyIconManager != null)
                {
                    notifyIconManager.SetText(string.Join("\n", list.Select(i => i.InfoLine)));
                }

                return list;
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return new List<NvDisplayInfo>();
            }
        }

        public List<NvPreset> GetDisplayPresets()
        {
            try
            {
                var list = new List<NvPreset>();

                var displayInfos = GetSimpleDisplayInfos();

                foreach (var displayInfo in displayInfos)
                {
                    var preset = GetPresetForDisplay(displayInfo, true, displayInfos.Count());
                    preset.MonitorConnectionType = preset?.Display?.DisplayDevice?.ConnectionType ?? NvAPIWrapper.Native.GPU.MonitorConnectionType.Unknown;
                    list.Add(preset);
                }

                return list;
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return null;
            }
        }

        public NvPreset GetPresetForDisplay(string name, bool driverSettings = false)
        {
            var displays = GetSimpleDisplayInfos();
            var display = displays.FirstOrDefault(d => name.StartsWith(d.Name));

            return GetPresetForDisplay(display, driverSettings, displays.Count());
        }

        public NvPreset GetPresetForDisplay(NvDisplayInfo displayInfo, bool driverSettings = false, int displayCount = 1)
        {
            if (displayInfo == null)
            {
                return null;
            }

            var display = displayInfo.Display;

            var isPrimaryDisplay = displayCount == 1 || display.DisplayDevice.DisplayId == DisplayDevice.GetGDIPrimaryDisplayDevice()?.DisplayId;
            var preset = new NvPreset { Display = display, IsDisplayPreset = true, applyColorData = false, primaryDisplay = isPrimaryDisplay };

            preset.HDREnabled = IsHDREnabled(display);
            preset.HdrSettings.OutputMode = preset.HDREnabled && OutputModeAvailable ? GetOutputMode(display) : null;

            preset.DisplayId = displayInfo.DisplayId;
            preset.displayName = displayInfo.Name;
            preset.colorData = GetCurrentColorData(display);
            preset.ColorEnhancementSettings = GetCurrentColorEnhancements(display);
            preset.HdmiInfoFrameSettings = GetHdmiSettings(display);
            preset.SDRBrightness = GetSDRBrightness(display.Name);
            preset.scaling = GetScaling(display);
            preset.ColorProfileSettings.ProfileName = CCD.GetDisplayDefaultColorProfile(display.Name, preset.HDREnabled ? COLORPROFILESUBTYPE.CPST_EXTENDED_DISPLAY_COLOR_MODE : COLORPROFILESUBTYPE.CPST_STANDARD_DISPLAY_COLOR_MODE);

            preset.DisplayConfig = CCD.GetDisplayConfig(display.Name);

            var ditherInfo = GetDithering(display);

            if (ditherInfo.state == -1)
            {
                preset.ditheringEnabled = true;
                preset.ditheringBits = (uint)NvDitherBits.Bits8;
                preset.ditheringMode = (uint)NvDitherMode.Temporal;
            }
            else
            {
                preset.ditheringEnabled = ditherInfo.state == 0 ? null : ditherInfo.state != (int)NvDitherState.Disabled;
                preset.ditheringBits = (uint)ditherInfo.bits;
                preset.ditheringMode = (uint)ditherInfo.mode;
            }

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

            if (driverSettings)
            {
                var settings = GetVisibleSettings();

                foreach (var setting in settings)
                {
                    var settingMeta = GetSettingMeta(setting.SettingId);

                    if (settingMeta == null)
                    {
                        continue;
                    }

                    var (presetSetting, isDefault, _) = settingMeta.ToIntValue(setting.ValueText);

                    if (isDefault)
                    {
                        continue;
                    }

                    preset.driverSettings.Add(setting.SettingId, presetSetting);
                }
            }

            return preset;
        }

        private NvColorEnhancementSettings GetCurrentColorEnhancements(Display display)
        {
            return new NvColorEnhancementSettings
            {
                DigitalVibranceLevel = GetDigitalVibranceLevel(display),
                HueAngle = GetHueAngle(display)
            };
        }

        public List<SettingItem> GetVisibleSettings()
        {
            return GetSettings().Where(s => _driverSettingIds.Contains(s.SettingId)).ToList();
        }

        public SettingMeta GetSettingMeta(uint settingId)
        {
            return _meta.GetSettingMeta(settingId);
        }

        public List<NvSettingItemDto> GetDriverSettings(string profileName = null)
        {
            if (profileName == null)
            {
                _drs.GetProfileNames(ref profileName, false);
            }

            var applications = new Dictionary<string, string>();
            var settings = profileName == null ? GetVisibleSettings() : _drs.GetSettingsForProfile(profileName, SettingViewMode.Normal, ref applications, true);

            return settings.Where(s => s.GroupName != null).Select(s =>
            {
                var settingMeta = _meta.GetSettingMeta(s.SettingId);
                var intValue = settingMeta.ToIntValue(s.ValueText);
                var type = settingMeta.SettingType == nspector.Native.NVAPI2.NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE ? NvSettingType.BinaryType : NvSettingType.DwordType;
                var values = type == NvSettingType.BinaryType ?
                    settingMeta.BinaryValues?.Select(bv => new NvSettingItemValue { Value = BitConverter.ToUInt32(bv.Value), ValueName = bv.ValueName }) :
                    settingMeta.DwordValues?.Select(bv => new NvSettingItemValue { Value = bv.Value, ValueName = bv.ValueName });

                return new NvSettingItemDto
                {
                    GroupName = s.GroupName,
                    IsApiExposed = s.IsApiExposed,
                    IsSettingHidden = s.IsSettingHidden,
                    IsStringValue = s.IsStringValue,
                    SettingId = s.SettingId,
                    SettingText = s.SettingText,
                    State = (int)s.State,
                    ValueRaw = s.ValueRaw,
                    ValueText = s.ValueText,
                    Value = intValue.intValue,
                    DefaultValue = intValue.defaultValue,
                    SettingType = type,
                    Values = values?.ToList()
                };
            }).ToList();
        }

        public List<string> GetDriverProfileNames(bool onlyFoundPrograms = false)
        {
            if (onlyFoundPrograms)
            {
                var profileNames = new List<string>();

                // First check game service for programs
                profileNames.AddRange(GetProfileNamesOfGamePresets());

                // Other ways: check MRU in registry:
                // - Computer\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\exe
                // - Computer\HKEY_CLASSES_ROOT\Local Settings\Software\Microsoft\Windows\Shell\MuiCache
                profileNames.AddRange(GetProfileNamesOfMru());

                profileNames = profileNames.Distinct().ToList();
                profileNames.Sort();

                profileNames.Insert(0, _baseProfileName);

                return profileNames;
            }

            var baseProfileName = "";

            var allProfiles = DrsServiceLocator.SettingService.GetProfileNames(ref baseProfileName).Where(s => !s.StartsWith("0x")).OrderBy(s => s).ToList();

            return allProfiles;
        }

        private List<string> GetProfileNamesOfGamePresets()
        {
            if (_serviceManager.GameService == null)
            {
                return new List<string>();
            }

            var gamePresets = _serviceManager.GameService.GetPresets();
            var appNames = gamePresets.Where(p => p.Path != null && Path.Exists(p.Path)).Select(p => Path.GetFileName(p.Path)).ToList();

            var profiles = DrsServiceLocator.SettingService.GetProfileNamesByApps(appNames);

            return profiles;
        }

        private List<string> GetProfileNamesOfMru()
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"Local Settings\Software\Microsoft\Windows\Shell\MuiCache");

            if (key == null)
            {
                return new List<string>();
            }

            var names = key.GetValueNames();
            var appNames = new List<string>();
            var winFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            foreach (var name in names)
            {
                var parts = name.Split('.');

                var path = name.Replace("." + parts.Last(), "");

                if (!path.StartsWith(winFolder) && Path.Exists(path))
                {
                    appNames.Add(Path.GetFileName(path));
                }
            }

            appNames = appNames.Distinct().ToList();

            var profiles = DrsServiceLocator.SettingService.GetProfileNamesByApps(appNames);

            return profiles;
        }

        public bool IsGsyncEnabled()
        {
            var setting = GetSettings().FirstOrDefault(s => s.SettingId == NvSettingConstants.DRS_GSYNC_APPLICATION_MODE);
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
            _semaphore.Wait();
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

            var version = default(string);
            DriverVersion = GeneralApi.GetDriverAndBranchVersion(out version);

            _drs = DrsServiceLocator.SettingService;
            _meta = DrsServiceLocator.MetaService;

            _drs.GetProfileNames(ref _baseProfileName, false);

            //if (!WinApiService.IsAdministratorStatic())
            //{
            _drs.ApplySettings += HandleDrsApplySettings;
            _drs.RestoreSetting += HandleDrsRestoreSetting;
            //}

            //SetGpuClocks();

            //Task.Run(() =>
            //{
            //    RefreshProfileSettings();
            //});
        }

        public void RefreshProfileSettings()
        {
            _refreshSemaphore.Wait();
            try
            {
                var firstTime = _settings?.Any() != true;

                DrsSessionScope.DestroyGlobalSession();

                var applications = default(Dictionary<string, string>);
                var normalSettings = _drs.GetSettingsForProfile(_baseProfileName, SettingViewMode.Normal, ref applications, true);

                _settings = new List<SettingItem>(normalSettings);

                _driverSettingIds.Clear();
                _driverSettingIds.AddRange(_settings.Where(s => !s.IsStringValue && !s.SettingText.Contains("SLI") && s.GroupName != null && s.GroupName != "Unknown").Select(s => s.SettingId));

                if (firstTime)
                {
                    DrsServiceLocator.ScannerService.ScanProfileSettings(false, null);
                    _meta.ResetMetaCache();
                }
            }
            finally
            {
                _refreshSemaphore.Release();
            }
        }

        private bool HasPrivilegesForUnexposedApis()
        {
            return _winApiService.IsAdministrator() || !_winApiService.IsServiceRunning();
        }

        private bool IsAnySettingUnexposed(List<uint> settings)
        {
            return settings.Any(s => !_meta.GetSettingMeta(s).IsApiExposed);
        }

        private void HandleDrsApplySettings(object sender, DrsEvent drsEvent)
        {
            if (HasPrivilegesForUnexposedApis() || !IsAnySettingUnexposed(drsEvent.Settings.Select(s => s.Key).ToList()))
            {
                return;
            }

            var message = new SvcRpcSetNvDriverSettingsMessage
            {
                MethodName = nameof(ApplyDriverSettings),
                ProfileName = drsEvent.ProfileName,
                DriverSettings = drsEvent.Settings
            };

            _rpcClientService.Call<object>(message);

            RefreshProfileSettings();

            drsEvent.Handled = true;
        }

        private void HandleDrsRestoreSetting(object sender, DrsEvent drsEvent)
        {
            if (HasPrivilegesForUnexposedApis() || !IsAnySettingUnexposed(drsEvent.Settings.Select(s => s.Key).ToList()))
            {
                return;
            }

            var message = new SvcRpcSetNvDriverSettingsMessage
            {
                MethodName = nameof(RestoreDriverSettings),
                ProfileName = drsEvent.ProfileName,
                DriverSettings = drsEvent.Settings
            };

            _rpcClientService.Call<object>(message);

            RefreshProfileSettings();

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

        public NvServiceConfig GetConfig()
        {
            return Config;
        }

        public bool UpdateConfig(NvServiceConfig config)
        {
            Config.Update(config);

            SetShortcuts(SHORTCUTID_NVQA, Config.QuickAccessShortcut);

            SaveConfig();

            return true;
        }

        internal Display ResolveDisplay(NvPreset preset)
        {
            if (preset.Display == null)
            {
                preset.Display = GetPresetDisplay(preset);
            }

            return preset.Display;
        }

        private async Task PowerModeResume(object sender, PowerStateChangedEventArgs e, CancellationToken _)
        {
            Logger.Debug($"PowerModeChanged: {e.State}");

            // Wait
            await Task.Delay(30000);

            await ExecutePresetsForEvent(PresetTriggerType.Resume);
        }

        private void PowerModeChanged(object sender, PowerStateChangedEventArgs e)
        {
            Logger.Debug($"PowerModeChanged: {e.State}");

            switch (e.State)
            {
                case PowerOnOffState.StandBy:
                    {
                        _ = ExecutePresetsForEvent(PresetTriggerType.Standby);
                        break;
                    }
                case PowerOnOffState.ShutDown:
                    {
                        _ = ExecutePresetsForEvent(PresetTriggerType.Shutdown);
                        break;
                    }
            }
        }

        private async Task ExecutePresetsForEvent(PresetTriggerType triggerType)
        {
            Logger.Debug($"Executing presets for event {triggerType}");

            var presets = _presets.Where(p => p.Triggers.Any(t => t.Trigger == triggerType)).ToList();

            if (!presets.Any())
            {
                return;
            }

            await ExecuteEventPresets(_serviceManager, new[] { triggerType }).ConfigureAwait(true);
        }

        private static uint MAKE_NVAPI_VERSION<T>(int version)
        {
            return (uint)((Marshal.SizeOf(typeof(T))) | version << 16);
        }

        private string GetDriverSettingsDescription(Dictionary<uint, uint> driverSettings, bool useNewLines = false)
        {
            if (driverSettings.Count == 0)
            {
                return "None";
            }

            var values = new List<string>();

            foreach (var driverSetting in driverSettings)
            {
                var settingMeta = GetSettingMeta(driverSetting.Key);

                var value = settingMeta.ToFriendlyName(intValue: driverSetting.Value, displayDefault: true);

                if (value == null)
                {
                    continue;
                }

                values.Add($"{settingMeta.SettingName}: {value}");
            }

            return string.Join(useNewLines ? "\r\n" : ", ", values);
        }

    }
}
