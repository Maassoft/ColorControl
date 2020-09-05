using NvAPIWrapper;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.GPU.Structures;
using NWin32;
using NWin32.NativeTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ColorControl
{
    class NvService
    {
        const UInt32 WM_KEYDOWN = 0x0100;
        const UInt32 WM_KEYUP = 0x0101;
        const int VK_TAB = 0x09;
        const int VK_SPACE = 0x20;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

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
            [Out] uint OutputId,
            [In] [Out] IntPtr ditherControl);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct NV_GPU_DITHER_CONTROL_V1
        {
            public int version;
            public uint state;
            public uint bits;
            public uint mode;
        };

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Display _currentDisplay;

        public NvService()
        {
            NVIDIA.Initialize();
        }

        ~NvService()
        {
            NVIDIA.Unload();
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

        public void ToggleHDR(int delay = 1000)
        {
            Process.Start("ms-settings:display");
            Thread.Sleep(delay);

            var process = Process.GetProcessesByName("SystemSettings").FirstOrDefault();
            if (process != null)
            {
                System.Windows.Forms.SendKeys.SendWait("{TAB}");
                System.Windows.Forms.SendKeys.SendWait("{TAB}");
                System.Windows.Forms.SendKeys.SendWait(" ");
                System.Windows.Forms.SendKeys.SendWait("%{F4}");
            }
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
                IntPtr bla = Marshal.AllocHGlobal(Marshal.SizeOf(info));
                Marshal.StructureToPtr(info, bla, false);

                // Does not work yet...What is the exact interface of NvAPI_Disp_GetDitherControl?

                //var result = delegateValue(gpuHandle, displayId, bla);
                //if (result != 0)
                //{
                //    Logger.Error($"Could not get dithering because NvAPI_Disp_GetDitherControl returned a non-zero return code: {result}");
                //}

                return info.state == 1;
            }

            return false;
        }

        public bool SetRefreshRate(uint refreshRate)
        {
            var display = GetCurrentDisplay();
            var portrait = new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);
            var timing = display.DisplayDevice.CurrentTiming;

            uint i = 0;
            DEVMODEA devMode;
            while (NativeMethods.EnumDisplaySettingsA(display.Name, i, out devMode))
            {
                // Also compare width with vertical and height with horizontal in case of portrait mode
                if (((!portrait && devMode.dmPelsWidth == timing.HorizontalVisible && devMode.dmPelsHeight == timing.VerticalVisible) ||
                    (portrait && devMode.dmPelsWidth == timing.VerticalVisible && devMode.dmPelsHeight == timing.HorizontalVisible))
                    && devMode.dmBitsPerPel == 32 && devMode.dmDisplayFrequency == refreshRate)
                {
                    IntPtr bla = Marshal.AllocHGlobal(Marshal.SizeOf(devMode));
                    Marshal.StructureToPtr(devMode, bla, false);
                    var result = NativeMethods.ChangeDisplaySettingsExA(display.Name, bla, IntPtr.Zero, 0, IntPtr.Zero);
                    if (result != NativeConstants.DISP_CHANGE_SUCCESSFUL)
                    {
                        Logger.Error($"Could not set refreshrate {refreshRate} on display {display.Name} because ChangeDisplaySettingsExA returned a non-zero return code: {result}");
                    }
                    return result == NativeConstants.DISP_CHANGE_SUCCESSFUL;
                }
                i++;
            }
            Logger.Info($"Could not set refreshrate {refreshRate} on display {display.Name} because EnumDisplaySettings did not report it as a valid refreshrate");
            return false;
        }

        public List<uint> GetAvailableRefreshRates(NvPreset preset = null)
        {
            var list = new List<uint>();

            if (preset != null)
            {
                SetCurrentDisplay(preset);
            }

            var display = GetCurrentDisplay();
            var portrait = new[] { Rotate.Degree90, Rotate.Degree270 }.Contains(display.DisplayDevice.ScanOutInformation.SourceToTargetRotation);
            var timing = display.DisplayDevice.CurrentTiming;

            uint i = 0;
            DEVMODEA devMode;
            while (NativeMethods.EnumDisplaySettingsA(display.Name, i, out devMode))
            {
                if ((!portrait && devMode.dmPelsWidth == timing.HorizontalVisible && devMode.dmPelsHeight == timing.VerticalVisible) ||
                    (portrait && devMode.dmPelsWidth == timing.VerticalVisible && devMode.dmPelsHeight == timing.HorizontalVisible))
                {
                    list.Add(devMode.dmDisplayFrequency);
                }
                i++;
            }
            return list;
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
    }
}
