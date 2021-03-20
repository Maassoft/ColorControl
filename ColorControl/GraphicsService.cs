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
    class GraphicsService : ServiceBase
    {
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettingsA(string deviceName, int modeNum, out DEVMODEA devMode);

        const int ENUM_CURRENT_SETTINGS = -1;
        const int ENUM_REGISTRY_SETTINGS = -2;

        public GraphicsService(string dataPath) : base(dataPath)
        {
            Initialize();
        }

        ~GraphicsService()
        {
            Uninitialize();
        }

        protected void ToggleHDR(int delay = 1000)
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

        protected void OpenDisplaySettings(int delay = 1000)
        {
            Process.Start("ms-settings:display");
            Thread.Sleep(delay);

            var process = Process.GetProcessesByName("SystemSettings").FirstOrDefault();
            if (process != null)
            {
                System.Windows.Forms.SendKeys.SendWait("%{F4}");
            }
        }

        protected bool SetRefreshRateInternal(string displayName, uint refreshRate, bool portrait, int horizontal, int vertical)
        {
            uint i = 0;
            DEVMODEA devMode;
            while (NativeMethods.EnumDisplaySettingsA(displayName, i, out devMode))
            {
                // Also compare width with vertical and height with horizontal in case of portrait mode
                if (((!portrait && devMode.dmPelsWidth == horizontal && devMode.dmPelsHeight == vertical) ||
                    (portrait && devMode.dmPelsWidth == vertical && devMode.dmPelsHeight == horizontal))
                    && devMode.dmBitsPerPel == 32 && devMode.dmDisplayFrequency == refreshRate)
                {
                    IntPtr bla = Marshal.AllocHGlobal(Marshal.SizeOf(devMode));
                    Marshal.StructureToPtr(devMode, bla, false);
                    var result = NativeMethods.ChangeDisplaySettingsExA(displayName, bla, IntPtr.Zero, 0, IntPtr.Zero);
                    if (result != NativeConstants.DISP_CHANGE_SUCCESSFUL)
                    {
                        Logger.Error($"Could not set refreshrate {refreshRate} on display {displayName} because ChangeDisplaySettingsExA returned a non-zero return code: {result}");
                    }
                    return result == NativeConstants.DISP_CHANGE_SUCCESSFUL;
                }
                i++;
            }
            Logger.Info($"Could not set refreshrate {refreshRate} on display {displayName} because EnumDisplaySettings did not report it as a valid refreshrate");

            return false;
        }

        protected List<uint> GetAvailableRefreshRatesInternal(string displayName, bool portrait, int horizontal, int vertical)
        {
            var list = new List<uint>();

            uint i = 0;
            DEVMODEA devMode;
            while (NativeMethods.EnumDisplaySettingsA(displayName, i, out devMode))
            {
                if ((!portrait && devMode.dmPelsWidth == horizontal && devMode.dmPelsHeight == vertical) ||
                    (portrait && devMode.dmPelsWidth == vertical && devMode.dmPelsHeight == horizontal))
                {
                    list.Add(devMode.dmDisplayFrequency);
                }
                i++;
            }
            return list;
        }

        protected uint GetCurrentRefreshRate(string displayName)
        {
            DEVMODEA devMode;
            // NativeMethods defines modeNum as an 'uint' but we need to pass '-1'
            if (EnumDisplaySettingsA(displayName, ENUM_CURRENT_SETTINGS, out devMode))
            {
                return devMode.dmDisplayFrequency;
            }
            return 0;
        }
    }
}
