using NvAPIWrapper;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.GPU.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColorControl
{
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

        public delegate long NvAPI_Disp_SetDitherControl(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint OutputId,
            [In] uint state,
            [In] uint bits,
            [In] uint mode
        );

        public delegate long NvAPI_Disp_GetDitherControl(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint OutputId,
            [In] IntPtr ditherControl);

        [StructLayout(LayoutKind.Sequential)]
        public struct NV_GPU_DITHER_CONTROL_V1
        {
            public int version;
            public uint state;
            public uint bits;
            public uint mode;
        };

        private Display _currentDisplay;

        public NvService(string dataPath) : base(dataPath)
        {
            var converter = new ColorDataConverter();
            _JsonDeserializer.RegisterConverters(new[] { converter });

            LoadPresets();
        }

        public static bool ExecutePresetAsync(string idOrName)
        {
            try
            {
                var nvService = new NvService(Program.DataDir);

                var result = nvService.ApplyPreset(idOrName);

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
            if (_currentDisplay == null)
            {
                _currentDisplay = Display.GetDisplays()[0];
            }

            return _currentDisplay;
        }

        public override bool HasDisplaysAttached()
        {
            try
            {
                return DisplayDevice.GetGDIPrimaryDisplayDevice() != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void GlobalSave()
        {
            SavePresets();
        }

        private void LoadPresets()
        {
            _presetsFilename = Path.Combine(_dataPath, "NvPresets.json");

            if (File.Exists(_presetsFilename))
            {
                var json = File.ReadAllText(_presetsFilename);

                _presets = _JsonDeserializer.Deserialize<List<NvPreset>>(json);
            }
            else
            {
                _presets = NvPreset.GetDefaultPresets();
            }
        }

        private void SavePresets()
        {
            try
            {
                var json = _JsonSerializer.Serialize(_presets);
                File.WriteAllText(_presetsFilename, json);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        private void SetCurrentDisplay(NvPreset preset)
        {
            if (preset.primaryDisplay)
            {
                var displayId = DisplayDevice.GetGDIPrimaryDisplayDevice().DisplayId;
                _currentDisplay = Display.GetDisplays().FirstOrDefault(x => x.DisplayDevice.DisplayId == displayId);
            }
            else
            {
                _currentDisplay = Display.GetDisplays().FirstOrDefault(x => x.Name.Equals(preset.displayName));
            }
        }


        public bool ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return ApplyPreset(preset, Program.AppContext);
            }
            else
            {
                return false;
            }
        }

        public bool ApplyPreset(NvPreset preset, AppContext appContext)
        {
            var result = true;

            if (!HasDisplaysAttached())
            {
                return false;
            }

            SetCurrentDisplay(preset);

            var hdrEnabled = IsHDREnabled();

            var newHdrEnabled = preset.applyHDR && (preset.HDREnabled || (preset.toggleHDR && !hdrEnabled));
            var applyHdr = preset.applyHDR && (preset.toggleHDR || preset.HDREnabled != hdrEnabled);

            if (preset.applyColorData && (ColorDataDiffers(preset.colorData) || (!newHdrEnabled && preset.applyColorData && preset.colorData.Colorimetry != ColorDataColorimetry.Auto)))
            {
                var display = GetCurrentDisplay();
                if (hdrEnabled)
                {
                    SetHDRState(display, false);

                    applyHdr = false;
                }

                try
                {
                    display.DisplayDevice.SetColorData(preset.colorData);
                }
                catch (Exception ex)
                {
                    Logger.Error($"SetColorData threw an exception: {ex.Message}");
                    result = false;
                }

                if (hdrEnabled && newHdrEnabled)
                {
                    SetHDRState(display, true, useSwitch: appContext.StartUpParams.NoGui);

                    applyHdr = false;
                }
            }

            if (applyHdr)
            {
                var display = GetCurrentDisplay();

                var colorData = preset.applyColorData ? preset.colorData : display.DisplayDevice.CurrentColorData;

                SetHDRState(display, newHdrEnabled, colorData, appContext.StartUpParams.NoGui);
            }

            if (preset.applyRefreshRate)
            {
                if (!SetRefreshRate(preset.refreshRate))
                {
                    result = false;
                }
            }

            if (preset.applyDithering)
            {
                if (!SetDithering(preset.ditheringEnabled, preset: preset))
                {
                    result = false;
                }
            }

            _lastAppliedPreset = preset;

            PresetApplied();

            return result;
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

        private void SetHDRState(Display display, bool enabled, ColorData colorData = null, bool useSwitch = false)
        {
            if (useSwitch)
            {
                if (enabled != IsHDREnabled())
                {
                    ToggleHDR();
                }
                return;
            }

            var newMaster = new MasteringDisplayColorData();
            var hdr = new HDRColorData(enabled ? ColorDataHDRMode.UHDA : ColorDataHDRMode.Off, newMaster, colorData?.ColorFormat, colorData?.DynamicRange, colorData?.ColorDepth);

            display.DisplayDevice.SetHDRColorData(hdr);

            // HDR will not always be disabled this way, then we can only disable it through the display settings
            if (!enabled && IsHDREnabled())
            {
                ToggleHDR();
            }
            else if (enabled)
            {
                // Currectly there seems to be a bug that after enabling HDR via NVAPI, some settings are only applied upon opening the Display Settings...
                ////OpenDisplaySettings();
            }
        }

        public bool SetDithering(bool enabled, uint bits = 1, uint mode = 4, NvPreset preset = null)
        {
            var result = true;

            var ptr = NvAPI64_QueryInterface(0xDF0DFCDD);
            if (ptr != IntPtr.Zero)
            {
                var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_SetDitherControl)) as NvAPI_Disp_SetDitherControl;

                var displayDevice = GetCurrentDisplay().DisplayDevice;

                var gpuHandle = displayDevice.PhysicalGPU.Handle;
                var displayId = displayDevice.DisplayId;

                if (preset != null)
                {
                    bits = preset.ditheringBits;
                    mode = preset.ditheringMode;
                }

                var resultValue = delegateValue(gpuHandle, displayId, (uint)(enabled ? 1 : 2), bits, mode);
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

        public bool GetDithering()
        {
            // Requires elevation, too bad :(
            //Utils.GetRegistryKeyValue(@"SYSTEM\CurrentControlSet\Services\nvlddmkm\State\DisplayDatabase", "DitherRegistryKey");

            /*
            var ptr = NvAPI64_QueryInterface(0x932AC8FB);
            if (ptr != IntPtr.Zero)
            {
                var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_GetDitherControl)) as NvAPI_Disp_GetDitherControl;

                var displayDevice = GetCurrentDisplay().DisplayDevice;

                var gpuHandle = displayDevice.PhysicalGPU.Handle;
                var displayId = displayDevice.DisplayId;

                NV_GPU_DITHER_CONTROL_V1 info = new NV_GPU_DITHER_CONTROL_V1();
                info.version = 1;
                var size = Marshal.SizeOf(info.GetType());
                IntPtr bla = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(info, bla, false);

                // Does not work yet...What is the exact interface of NvAPI_Disp_GetDitherControl?

                var result = delegateValue(gpuHandle, displayId, bla);
                if (result != 0)
                {
                    Logger.Error($"Could not get dithering because NvAPI_Disp_GetDitherControl returned a non-zero return code: {result}");
                }

                return info.state == 1;
            }
            */
            return false;
        }

        public bool SetRefreshRate(uint refreshRate)
        {
            var display = GetCurrentDisplay();
            var timing = display.DisplayDevice.CurrentTiming;

            if (timing.Extra.RefreshRate == refreshRate)
            {
                return true;
            }

            var portrait = new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);

            return SetRefreshRateInternal(display.Name, refreshRate, portrait, timing.HorizontalVisible, timing.VerticalVisible);
        }

        public List<uint> GetAvailableRefreshRates(NvPreset preset = null)
        {
            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay();
            var portrait = new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);
            var timing = display.DisplayDevice.CurrentTiming;

            return GetAvailableRefreshRatesInternal(display.Name, portrait, timing.HorizontalVisible, timing.VerticalVisible);
        }

        public bool IsHDREnabled()
        {
            var displayDevice = GetCurrentDisplay().DisplayDevice;
            var hdr = displayDevice.HDRColorData;
            return hdr?.HDRMode == ColorDataHDRMode.UHDA;
        }

        public Display[] GetDisplays()
        {
            return Display.GetDisplays();
        }

        public List<NvDisplayInfo> GetDisplayInfos()
        {
            Display[] displays;
            try
            {
                displays = GetDisplays();
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return null;
            }

            var list = new List<NvDisplayInfo>();

            foreach (var display in displays)
            {
                var values = new List<string>();

                values.Add("Current settings");

                var name = display.Name;

                var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(name));
                if (screen != null)
                {
                    name += " (" + screen.DeviceFriendlyName() + ")";
                }

                values.Add(name);

                var colorData = GetCurrentColorData(display);
                
                var colorSettings = string.Format("{0}, {1}, {2}, {3}", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry);

                values.Add(colorSettings);

                var refreshRate = display.DisplayDevice.CurrentTiming.Extra.RefreshRate;

                values.Add($"{refreshRate}Hz");

                var lastPreset = GetLastAppliedPreset();
                if (lastPreset != null)
                {
                    values.Add(lastPreset.GetDitheringDescription());
                }
                else
                {
                    values.Add(string.Empty);
                }

                var hdrEnabled = IsHDREnabled();
                values.Add(hdrEnabled ? "Yes" : "No");

                var infoLine = string.Format("{0}: {1}, {2}Hz, HDR: {3}", name, colorSettings, refreshRate, hdrEnabled ? "Yes" : "No");

                var displayInfo = new NvDisplayInfo(display, values, infoLine);

                list.Add(displayInfo);
            }

            return list;
        }

        public NvPreset GetLastAppliedPreset()
        {
            return _lastAppliedPreset;
        }

        protected override void Initialize()
        {
            NVIDIA.Initialize();
            _initialized = true;
        }

        protected override void Uninitialize()
        {
            if (_initialized)
            {
                NVIDIA.Unload();
            }
        }
    }
}
