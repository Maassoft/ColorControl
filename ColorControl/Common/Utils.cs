using ColorControl.Forms;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NWin32;
using NWin32.NativeTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;

namespace ColorControl.Common
{
    static class Utils
    {
        [Flags]
        public enum ModKeys : int
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        public const int WM_BRINGTOFRONT = NativeConstants.WM_USER + 1;

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int ENUM_REGISTRY_SETTINGS = -2;

        public static string PKEY_PNPX_IpAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12297";
        public static string PKEY_PNPX_PhysicalAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12294";
        public static Guid GUID_CONSOLE_DISPLAY_STATE = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");
        public static int PBT_POWERSETTINGCHANGE = 32787;

        public delegate void PCREATE_PROCESS_NOTIFY_ROUTINE(IntPtr ParentId, IntPtr ProcessId, bool Create);

        public static bool ConsoleOpened { get; private set; }

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
        private static extern bool AttachConsole(int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();

        [DllImport(@"User32", SetLastError = true)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr hWnd);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool WinKeyDown = false;
        private static Keys[] KeysWithoutModifiers = new[] { Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24 };

        public static Bitmap SubPixelShift(Bitmap bitmap)
        {
            Bitmap bitmap2 = (Bitmap)bitmap.Clone();

            int shift = -1;
            bool shiftRed = true;
            bool shiftGreen = true;
            bool shiftBlue = false;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);

                    if (shift > 0 && i < bitmap.Width - shift)
                    {
                        Color pixel2 = bitmap2.GetPixel(i + shift, j);

                        Color pixel3 = Color.FromArgb(shiftRed ? pixel.R : pixel2.R, shiftGreen ? pixel.G : pixel2.G, shiftBlue ? pixel.B : pixel2.B);

                        bitmap2.SetPixel(i + shift, j, pixel3);

                    }
                    else if (i + shift >= 0)
                    {
                        Color pixel2 = bitmap2.GetPixel(i + shift, j);

                        Color pixel3 = Color.FromArgb(shiftRed ? pixel.R : pixel2.R, shiftGreen ? pixel.G : pixel2.G, shiftBlue ? pixel.B : pixel2.B);

                        bitmap2.SetPixel(i + shift, j, pixel3);

                    }
                }
            }

            return bitmap2;
            //bitmap2.Save("d:\\ss_shifted.bmp");
        }

        public static string GetDataPath(string subDir = "")
        {
            var path = Directory.GetParent(Application.UserAppDataPath).FullName;
            if (!string.IsNullOrEmpty(subDir))
            {
                path = Path.Combine(path, subDir);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            return path;
        }

        public static (int mods, int key) ParseShortcut(string shortcut)
        {
            int mods = 0;
            var key = 0;

            shortcut = shortcut.Replace(",", "");
            shortcut = shortcut.Replace("+", "");
            shortcut = shortcut.Replace("  ", " ");
            var parts = shortcut.Split(' ');

            if (parts.Length > 0)
            {
                for (int i = 0; i + 1 < parts.Length; i++)
                {
                    switch (parts[i])
                    {
                        case "Alt":
                            mods += (int)ModKeys.Alt;
                            break;
                        case "Control":
                            mods += (int)ModKeys.Control;
                            break;
                        case "Shift":
                            mods += (int)ModKeys.Shift;
                            break;
                        case "Win":
                            mods += (int)ModKeys.Win;
                            break;
                    }
                }

                var keyName = parts[parts.Length - 1];

                key = (int)Enum.Parse(typeof(Keys), keyName);
            }

            return (mods, key);
        }

        public static Keys ShortcutToKeys(string shortcut)
        {
            var (mods, key) = ParseShortcut(shortcut);

            var keys = Keys.None;

            if (((uint)mods & (uint)ModKeys.Alt) > 0)
            {
                keys |= Keys.Alt;
            }
            if (((uint)mods & (uint)ModKeys.Control) > 0)
            {
                keys |= Keys.Control;
            }
            if (((uint)mods & (uint)ModKeys.Shift) > 0)
            {
                keys |= Keys.Shift;
            }
            if (((uint)mods & (uint)ModKeys.Win) > 0)
            {
                keys |= Keys.Control;
            }

            keys |= (Keys)key;

            return keys;
        }

        internal static bool ExecuteElevated(string args)
        {
            var info = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName, args)
            {
                Verb = "runas", // indicates to elevate privileges
                UseShellExecute = true,
            };

            var process = new Process
            {
                EnableRaisingEvents = true, // enable WaitForExit()
                StartInfo = info
            };
            try
            {
                process.Start();
                process.WaitForExit(); // sleep calling process thread until evoked process exit
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("ExecuteElevated: " + e.Message);
            }
            return false;
        }

        internal static bool IsChromeFixInstalled()
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command");
            return key != null && key.GetValue(null).ToString().Contains("--disable-lcd-text");
        }

        internal static bool InstallChromeFix(bool install)
        {
            var argument = "--disable-lcd-text";

            var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command", true);
            if (key != null)
            {
                var value = key.GetValue(null).ToString();
                value = value.Replace("chrome.exe\" " + argument + " --", "chrome.exe\" --");
                if (install)
                {
                    value = value.Replace("chrome.exe\" --", "chrome.exe\" " + argument + " --");
                }
                key.SetValue(null, value);
            }

            var roamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            UpdateShortcut(Path.Combine(roamingFolder, @"Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\Google Chrome.lnk"), argument, !install);
            UpdateShortcut(Path.Combine(roamingFolder, @"Microsoft\Internet Explorer\Quick Launch\Google Chrome.lnk"), argument, !install);

            var allUsersStartMenuFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            UpdateShortcut(Path.Combine(allUsersStartMenuFolder, @"Programs\Google Chrome.lnk"), argument, !install);

            return true;
        }

        internal static bool IsChromeInstalled()
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command");
            return key != null;
        }

        internal static void GetRegistryKeyValue(string keyname, string valueName, bool deepSearch = false)
        {
            var key = Registry.LocalMachine.OpenSubKey(keyname);

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                var subKey = key.OpenSubKey(subKeyName);
                if (subKey.GetValueNames().Contains(valueName))
                {
                    Thread.Sleep(0);
                }
            }

        }

        internal static bool UpdateShortcut(string path, string arguments, bool removeArguments = false)
        {
            if (File.Exists(path))
            {
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);

                shortcut.Arguments = shortcut.Arguments.Replace(arguments, "");
                if (!removeArguments)
                {
                    if (!string.IsNullOrEmpty(shortcut.Arguments))
                    {
                        shortcut.Arguments = shortcut.Arguments + " ";
                    }
                    shortcut.Arguments = shortcut.Arguments + arguments;
                }

                // save it / create
                shortcut.Save();

                return true;
            }
            return false;
        }

        public static string GetDeviceProperty(PnpObject device, string propertyName)
        {
            object objValue;
            string value = null;

            //foreach (var key in device.Properties.Keys)
            //{
            //    Debug.WriteLine(key);
            //}

            if (device.Properties.TryGetValue(propertyName, out objValue))
            {
                if (objValue is string[])
                {
                    var arr = objValue as string[];
                    if (arr.Length > 0)
                    {
                        value = arr[0];
                    }
                }
                else if (objValue is byte[])
                {
                    value = BitConverter.ToString((byte[])objValue);
                }
                else
                {
                    value = objValue?.ToString();
                }
            }

            return value;
        }

        public static async Task<List<PnpDev>> GetPnpDevices(string deviceName)
        {
            var devices = new List<PnpDev>();

            var list = await DeviceInformation.FindAllAsync("", new List<string>());

            var reqs = new List<string>
            {
                PKEY_PNPX_IpAddress,
                PKEY_PNPX_PhysicalAddress
            };

            foreach (var dev in list)
            {
                string name = dev.Name;
                if (/*dev.IsEnabled &&*/ dev.Name.Contains(deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    var guid = dev.Properties["System.Devices.DeviceInstanceId"];

                    var aqs = "System.Devices.DeviceInstanceId:=\"" + guid + "\"";
                    var list2 = await PnpObject.FindAllAsync(PnpObjectType.Device, reqs, aqs);

                    foreach (var dev2 in list2)
                    {
                        var ipAddress = GetDeviceProperty(dev2, "System.Devices.IpAddress");
                        var macAddress = GetDeviceProperty(dev2, PKEY_PNPX_PhysicalAddress);

                        if (!devices.Any(x => x.Name.Equals(name) && x.IpAddress != null && x.IpAddress.Equals(ipAddress)))
                        {
                            var device = new PnpDev(dev, dev2, name, ipAddress, macAddress);
                            devices.Add(device);
                        }
                    }

                }
            }
            return devices;
        }

        public static T WaitForTask<T>(Task<T> task)
        {
            while (task != null && (task.Status < TaskStatus.WaitingForChildrenToComplete))
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
            return task.Result;
        }

        public static void WaitForTask(System.Threading.Tasks.Task task)
        {
            while (task != null && (task.Status < TaskStatus.WaitingForChildrenToComplete))
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }

        internal static Image GenerateGradientBitmap(int width, int height)
        {
            var bitmap = new Bitmap(512, height);

            for (var i = 0; i < 256; i++)
            {
                var pixel = Color.FromArgb(i, i, i);

                for (var h = 0; h < height; h++)
                {
                    bitmap.SetPixel(i * 2, h, pixel);
                    bitmap.SetPixel(i * 2 + 1, h, pixel);
                }
            }

            return bitmap;
        }

        public static string FirstCharUpperCase(string name)
        {
            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }

        public static string GetDescriptionByEnumName<T>(string value) where T : IConvertible
        {
            string description = null;

            if (Enum.TryParse(typeof(T), value, out var result))
            {
                description = (result as IConvertible)?.GetDescription();
            }

            description = description ?? FirstCharUpperCase(value);

            return description;
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            return GetDescription(e.GetType(), e);
        }

        public static string GetDescription(Type enumType, IConvertible value)
        {
            if (enumType.BaseType == typeof(Enum))
            {
                var val = value.ToInt32(CultureInfo.InvariantCulture);

                var enumName = enumType.GetEnumName(val);
                if (enumName == null)
                {
                    return string.Empty;
                }

                var memInfo = enumType.GetMember(enumName);
                var descriptionAttribute = memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() as DescriptionAttribute;

                if (descriptionAttribute != null)
                {
                    return descriptionAttribute.Description;
                }
            }

            return null; // could also return string.Empty
        }

        public static string GetEnumNameByDescription(Type enumType, string description)
        {
            if (enumType.BaseType == typeof(Enum))
            {
                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    if (description.Equals(GetDescription(enumType, enumValue as IConvertible), StringComparison.Ordinal))
                    {
                        return Enum.GetName(enumType, enumValue);
                    }
                }
            }

            return null; // could also return string.Empty
        }

        public static List<string> GetDescriptions<T>(int value = -1, int fromValue = 0) where T : IConvertible
        {
            return GetDescriptions(typeof(T), value, fromValue);
        }

        public static List<string> GetDescriptions(Type enumType, int value = -1, int fromValue = 0)
        {
            var list = new List<string>();
            foreach (var enumValue in Enum.GetValues(enumType))
            {
                if ((int)enumValue < fromValue || value >= 0 && ((int)enumValue & value) == 0)
                {
                    continue;
                }
                list.Add(GetDescription(enumType, enumValue as IConvertible));
            }
            return list;
        }

        public static void RegisterTask(string taskName, bool enabled)
        {
            var file = Process.GetCurrentProcess().MainModule.FileName;
            var directory = Path.GetDirectoryName(file);

            try
            {
                using (TaskService ts = new TaskService())
                {
                    if (enabled)
                    {
                        TaskDefinition td = ts.NewTask();
                        td.RegistrationInfo.Description = "Start ColorControl";

                        td.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name });

                        td.Actions.Add(new ExecAction(file, StartUpParams.RunningFromScheduledTaskParam, directory));

                        ts.RootFolder.RegisterTaskDefinition(taskName, td);
                    }
                    else
                    {
                        ts.RootFolder.DeleteTask(taskName, false);
                    }
                }
            }
            catch (Exception e)
            {
                //MessageForms.ErrorOk("Could not create/delete task: " + e.Message);
            }
        }

        public static bool TaskExists(string taskName, bool update)
        {
            var file = Assembly.GetExecutingAssembly().Location;
            using (TaskService ts = new TaskService())
            {
                var task = ts.RootFolder.Tasks.FirstOrDefault(x => x.Name.Equals(taskName));

                if (task != null)
                {
                    //var action = task.Definition.Actions.FirstOrDefault(x => x.ActionType == TaskActionType.Execute);
                    //if (action != null)
                    //{
                    //    var execAction = action as ExecAction;
                    //    if (!execAction.Path.Equals(file))
                    //    {
                    //        RegisterTask(taskName, false);
                    //        RegisterTask(taskName, true);
                    //    }
                    //}
                    return true;
                }
                return false;
            }
        }

        public static bool RegisterShortcut(IntPtr handle, int id, string shortcut, bool clear = false)
        {
            if (clear)
            {
                UnregisterHotKey(handle, id);
            }

            if (!string.IsNullOrEmpty(shortcut))
            {
                var (mods, key) = ParseShortcut(shortcut);
                var result = RegisterHotKey(handle, id, mods, key);
                if (!result)
                {
                    var errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                    Logger.Error($"Could not register shortcut {shortcut}: {errorMessage}");
                }

                return result;
            }

            return true;
        }

        public static string FormatKeyboardShortcut(KeyEventArgs keyEvent)
        {
            var pressedModifiers = keyEvent.Modifiers;

            //Debug.WriteLine("KD: " + e.Modifiers + ", " + e.KeyCode);

            var shortcutString = (pressedModifiers > 0 ? pressedModifiers.ToString() : "");
            if (keyEvent.KeyCode == Keys.LWin || WinKeyDown)
            {
                WinKeyDown = true;
                if (!string.IsNullOrEmpty(shortcutString))
                {
                    shortcutString += ", ";
                }
                shortcutString += "Win";
            }

            var empty = string.IsNullOrEmpty(shortcutString);
            if ((!empty || KeysWithoutModifiers.Contains(keyEvent.KeyCode)) && keyEvent.KeyCode != Keys.ControlKey && keyEvent.KeyCode != Keys.ShiftKey && keyEvent.KeyCode != Keys.Menu && keyEvent.KeyCode != Keys.LWin)
            {
                if (!empty)
                {
                    shortcutString += " + ";
                }
                shortcutString += keyEvent.KeyCode.ToString();
            }

            if (string.IsNullOrEmpty(shortcutString))
            {
                keyEvent.SuppressKeyPress = true;
            }

            return shortcutString;
        }

        public static bool ValidateShortcut(string shortcut)
        {
            var valid = string.IsNullOrWhiteSpace(shortcut) || shortcut.Contains("+") || ShortcutIsAllowedWithoutModifier(shortcut);

            if (!valid)
            {
                MessageForms.WarningOk("Invalid shortcut. The shortcut should have modifiers and a normal key or be F13-F24.");
            }

            return valid;
        }

        public static bool ShortcutIsAllowedWithoutModifier(string shortcut)
        {
            return Enum.TryParse<Keys>(shortcut, out var key) && KeysWithoutModifiers.Contains(key);
        }

        public static void HandleKeyboardShortcutUp(KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.LWin)
            {
                WinKeyDown = false;
            }
        }

        public static bool OpenConsole()
        {
            if (ConsoleOpened)
            {
                return true;
            }

            if (!AttachConsole(-1))
            {
                AllocConsole();
                return true;
            }
            ConsoleOpened = true;

            return false;
        }

        public static bool CloseConsole()
        {
            if (!ConsoleOpened)
            {
                return false;
            }

            SendKeys.SendWait("{ENTER}");
            var result = FreeConsole();

            return result;
        }

        public static Process GetProcessByName(string name, bool skipCurrent = true)
        {
            var currentProcessId = Environment.ProcessId;
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.Equals(name) && (!skipCurrent || process.Id != currentProcessId))
                {
                    return process;
                }
            }

            return null;
        }

        public static void StartProcess(string fileName, string arguments = null, bool hidden = false, bool wait = false, bool setWorkingDir = false, bool elevate = false)
        {
            var process = Process.Start(new ProcessStartInfo(fileName, arguments)
            {
                Verb = elevate ? "runas" : string.Empty, // indicates to elevate privileges
                UseShellExecute = true,
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                WorkingDirectory = setWorkingDir ? Path.GetDirectoryName(fileName) : null
            });
            if (wait)
            {
                process.WaitForExit();
            }
        }

        public static string GetResourceFile(string resourceName)
        {
            resourceName = "ColorControl.Resources." + resourceName;

            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();

                return result;
            }
        }

        public static void ParseWords(List<string> target, string text)
        {
            target.Clear();

            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            var words = text.Split(',');
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                if (word.IndexOf("(") > -1)
                {
                    words[i] = word.Trim();
                }
                else
                {
                    words[i] = word.Replace(" ", string.Empty);
                }
            }
            target.AddRange(words);
        }

        public static int ParseInt(string value, int def = 0)
        {
            if (int.TryParse(value, out var result))
            {
                return result;
            }

            return def;
        }

        public static int ParseDynamicAsInt(dynamic value, int def = 0)
        {
            if (value.Type == JTokenType.Integer)
            {
                return (int)value.Value;
            }
            if (value.Type == JTokenType.String)
            {
                return ParseInt(value.Value, def);
            }
            return def;
        }

        public static async Task<dynamic> GetRestJsonAsync(string url, Action<dynamic> callBack = null)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<dynamic>(result);

                if (callBack != null)
                {
                    callBack(data);
                }

                return data;
            }

            if (callBack != null)
            {
                callBack(null);
            }

            return null;
        }

        public static FileInfo SelectFile(string ext = "*.exe", string filter = "Application EXE Name|*.exe|Application Absolute Path|*.exe")
        {
            var openDialog = new OpenFileDialog();
            openDialog.DefaultExt = ext;
            openDialog.Filter = filter;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                return new FileInfo(openDialog.FileName);
                //if (openDialog.FilterIndex == 2)
                //    applicationName = openDialog.FileName;

                //return applicationName;
            }

            return null;
        }

        public static bool NormEquals(this string str1, string str2)
        {
            return str1.Trim().Replace(" ", string.Empty).Equals(str2.Trim().Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        }
    }
}
