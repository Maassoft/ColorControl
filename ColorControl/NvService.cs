using NvAPIWrapper;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.GPU.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColorControl
{
    class NvService : GraphicsService
    {
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
            [In][Out] IntPtr ditherControl);

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct NV_GPU_DITHER_CONTROL_V1
        {
            public int version;
            public uint state;
            public uint bits;
            public uint mode;
        };

        private Display _currentDisplay;
        private bool _initialized = false;

        public NvService()
        {
        }

        public Display GetCurrentDisplay()
        {
            if (_currentDisplay == null)
            {
                _currentDisplay = Display.GetDisplays()[0];
            }

            return _currentDisplay;
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


        public bool ApplyPreset(NvPreset preset, Config config)
        {
            SetCurrentDisplay(preset);

            if (preset.applyHDR)
            {
                if (preset.toggleHDR || preset.HDREnabled != IsHDREnabled())
                {
                    ToggleHDR(config.DisplaySettingsDelay);
                }
            }

            if (preset.applyRefreshRate)
            {
                SetRefreshRate(preset.refreshRate);
            }

            if (preset.applyColorData)
            {
                var display = GetCurrentDisplay();
                //var data = new ColorData(ColorDataFormat.YUV444, ColorDataColorimetry.YCC709, ColorDataDynamicRange.Auto, ColorDataDepth.BPC8, preset.colorData.SelectionPolicy, ColorDataDesktopDepth.Default);
                display.DisplayDevice.SetColorData(preset.colorData);

                //var master = display.DisplayDevice.HDRColorData.MasteringDisplayData;
                //var newMaster = new MasteringDisplayColorData(master.FirstColorCoordinate, master.SecondColorCoordinate, master.ThirdColorCoordinate, new ColorDataColorCoordinate(0.25f, 0.25f), 50, 0, 50, 50);
                //var hdr = new HDRColorData(ColorDataHDRMode.UHDA, newMaster, ColorDataFormat.YUV444, ColorDataDynamicRange.Auto, ColorDataDepth.BPC8);
                //display.DisplayDevice.SetHDRColorData(hdr);
            }

            if (preset.applyDithering)
            {
                SetDithering(preset.ditheringEnabled);
            }

            return true;
        }

        public void SetDithering(bool enabled)
        {
            var ptr = NvAPI64_QueryInterface(0xDF0DFCDD);
            if (ptr != IntPtr.Zero)
            {
                var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_SetDitherControl)) as NvAPI_Disp_SetDitherControl;

                var displayDevice = GetCurrentDisplay().DisplayDevice;

                var gpuHandle = displayDevice.PhysicalGPU.Handle;
                var displayId = displayDevice.DisplayId;

                var result = delegateValue(gpuHandle, displayId, (uint)(enabled ? 1 : 2), 1, 4);
                if (result != 0)
                {
                    Logger.Error($"Could not set dithering because NvAPI_Disp_SetDitherControl returned a non-zero return code: {result}");
                }
            }
        }

        public bool GetDithering()
        {
            var ptr = NvAPI64_QueryInterface(0x932AC8FB);
            if (ptr != IntPtr.Zero)
            {
                var delegateValue = Marshal.GetDelegateForFunctionPointer(ptr, typeof(NvAPI_Disp_GetDitherControl)) as NvAPI_Disp_GetDitherControl;

                var displayDevice = GetCurrentDisplay().DisplayDevice;

                var gpuHandle = displayDevice.PhysicalGPU.Handle;
                var displayId = displayDevice.DisplayId;

                NV_GPU_DITHER_CONTROL_V1 info = new NV_GPU_DITHER_CONTROL_V1();
                info.version = 1;
                IntPtr bla = Marshal.AllocHGlobal(Marshal.SizeOf(info.GetType()));
                Marshal.StructureToPtr(info, bla, false);

                // Does not work yet...What is the exact interface of NvAPI_Disp_GetDitherControl?

                var result = delegateValue(gpuHandle, displayId, bla);
                if (result != 0)
                {
                    Logger.Error($"Could not get dithering because NvAPI_Disp_GetDitherControl returned a non-zero return code: {result}");
                }

                return info.state == 1;
            }

            return false;
        }

        public bool SetRefreshRate(uint refreshRate)
        {
            var display = GetCurrentDisplay();
            var portrait = new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);
            var timing = display.DisplayDevice.CurrentTiming;

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
