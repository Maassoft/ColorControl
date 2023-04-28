using NWin32.NativeTypes;
using System;
using System.Runtime.InteropServices;

namespace ColorControl.Native
{
    public static class WinApi
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData; // in C/C++ this is an UINT_PTR, not an UINT
            public int cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyData : IDisposable
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, ref CopyData target,
                                                    SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

            [Flags]
            enum SendMessageTimeoutFlags : uint
            {
                SMTO_NORMAL = 0x0,
                SMTO_BLOCK = 0x1,
                SMTO_ABORTIFHUNG = 0x2,
                SMTO_NOTIMEOUTIFNOTHUNG = 0x8
            }
            const uint WM_COPYDATA = 0x4A;

            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;

            public void Dispose()
            {
                if (lpData != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(lpData);
                    lpData = IntPtr.Zero;
                    cbData = 0;
                }
            }
            public string AsAnsiString
            {
                get { return Marshal.PtrToStringAnsi(lpData, cbData); }
            }
            public string AsUnicodeString
            {
                get { return Marshal.PtrToStringUni(lpData); }
            }
            public static CopyData CreateForString(int dwData, string value, bool Unicode = false)
            {
                var result = new CopyData();
                result.dwData = (IntPtr)dwData;
                result.lpData = Unicode ? Marshal.StringToCoTaskMemUni(value) : Marshal.StringToCoTaskMemAnsi(value);
                result.cbData = value.Length + 2;
                return result;
            }

            public static UIntPtr Send(IntPtr targetHandle, int dwData, string value, uint timeoutMs = 1000, bool Unicode = false)
            {
                var cds = CreateForString(dwData, value, Unicode);
                UIntPtr result;
                SendMessageTimeout(targetHandle, WM_COPYDATA, IntPtr.Zero, ref cds, SendMessageTimeoutFlags.SMTO_NORMAL, timeoutMs, out result);
                cds.Dispose();
                return result;
            }
        }

        public enum MessageFilterInfo : uint
        {
            None = 0, AlreadyAllowed = 1, AlreadyDisAllowed = 2, AllowedHigher = 3
        };

        public enum ChangeWindowMessageFilterExAction : uint
        {
            Reset = 0, Allow = 1, DisAllow = 2
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CHANGEFILTERSTRUCT
        {
            public uint size;
            public MessageFilterInfo info;
        }

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int ENUM_REGISTRY_SETTINGS = -2;

        public static int PBT_POWERSETTINGCHANGE = 32787;

        public delegate void PCREATE_PROCESS_NOTIFY_ROUTINE(IntPtr ParentId, IntPtr ProcessId, bool Create);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettingsA(string deviceName, int modeNum, out DEVMODEA devMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool AttachConsole(int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();

        [DllImport(@"User32", SetLastError = true)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, ChangeWindowMessageFilterExAction action, ref CHANGEFILTERSTRUCT changeInfo);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, EntryPoint = "#104")]
        public static extern int RefreshImmersiveColorPolicyState();

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, EntryPoint = "#133")]
        public static extern int AllowDarkModeForWindow(IntPtr hWnd, bool allow);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, EntryPoint = "#135")]
        public static extern int SetPreferredAppMode(int appMode);

        [DllImport("uxtheme.dll", EntryPoint = "#136")]
        public static extern void FlushMenuThemes();

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    }
}
