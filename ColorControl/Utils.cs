using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NStandard;
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

namespace ColorControl
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

        public enum UserNotificationState : int
        {
            QUNS_NOT_PRESENT = 1,
            QUNS_BUSY,
            QUNS_RUNNING_D3D_FULL_SCREEN,
            QUNS_PRESENTATION_MODE,
            QUNS_ACCEPTS_NOTIFICATIONS,
            QUNS_QUIET_TIME,
            QUNS_APP
        }

        public static UserNotificationState[] NotificationsDisabledStates = new[] {
            UserNotificationState.QUNS_BUSY,
            UserNotificationState.QUNS_RUNNING_D3D_FULL_SCREEN,
            UserNotificationState.QUNS_PRESENTATION_MODE,
        };

        public const int WM_BRINGTOFRONT = NativeConstants.WM_USER + 1;

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int ENUM_REGISTRY_SETTINGS = -2;

        public static string PKEY_PNPX_IpAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12297";
        public static string PKEY_PNPX_PhysicalAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12294";

        public delegate void PCREATE_PROCESS_NOTIFY_ROUTINE(IntPtr ParentId, IntPtr ProcessId, bool Create);

        public static bool ConsoleOpened { get; private set; }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public extern static bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("user32.dll")]
        public extern static bool ShutdownBlockReasonDestroy(IntPtr hWnd);

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

        [DllImport("shell32.dll")]
        public static extern int SHQueryUserNotificationState(out UserNotificationState pquns);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        //[DllImport("Wtsapi32.dll")]
        //public extern static bool WTSRegisterSessionNotification(IntPtr hWnd, uint dwFlags);

        //[DllImport("Wtsapi32.dll")]
        //public extern static bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool WinKeyDown = false;
        private static UserNotificationState LastNotificationState;
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
                if (/*dev.IsEnabled &&*/ dev.Name.Contains(deviceName))
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

        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            text = text.Substring(0, Math.Min(text.Length, 127));
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
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

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            return GetDescription(e.GetType(), e);
        }

        public static string GetDescription(Type enumType, IConvertible value)
        {
            if (enumType.BaseType == typeof(Enum))
            {
                var val = value.ToInt32(CultureInfo.InvariantCulture);

                var memInfo = enumType.GetMember(enumType.GetEnumName(val));
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
                MessageForms.ErrorOk("Could not create/delete task: " + e.Message);
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

        public static ToolStripMenuItem BuildDropDownMenuEx(ContextMenuStrip mnuParent, string name, Type enumType, EventHandler clickEvent, object tag = null, int min = 0, int max = 0)
        {
            var subMenuName = $"{mnuParent.Name}_{name}";
            var subMenuItems = mnuParent.Items.Find(subMenuName, false);

            if (subMenuItems.Length > 0)
            {
                return subMenuItems[0] as ToolStripMenuItem;
            }

            ToolStripMenuItem subMenuItem;
            subMenuItem = (ToolStripMenuItem)mnuParent.Items.Add(name);
            subMenuItem.Name = subMenuName;

            if (enumType != null)
            {
                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    var itemText = (enumValue as IConvertible).GetDescription() ?? enumValue.ToString();
                    var item = subMenuItem.DropDownItems.Add(itemText);
                    item.Tag = tag;
                    item.AccessibleName = enumValue.ToString();
                    item.Click += clickEvent;
                }
            }
            else if (min >= 0 && max > min)
            {
                for (var i = 0; i <= 10; i++)
                {
                    var value = i * (max / 10);

                    var subSubItemName = $"{subMenuName}_{value}";

                    var item = subMenuItem.DropDownItems.Add(value.ToString());
                    item.Name = subSubItemName;
                    item.Tag = tag;
                    item.Click += clickEvent;
                }
            }
            else
            {
                subMenuItem.Tag = tag;
                subMenuItem.Click += clickEvent;
            }

            return subMenuItem;
        }

        public static void BuildDropDownMenu(ToolStripDropDownItem mnuParent, string name, Type enumType, object colorData, string propertyName, EventHandler clickEvent)
        {
            PropertyInfo property = null;
            var subMenuItems = mnuParent.DropDownItems.Find("miColorSettings_" + name, false);
            ToolStripMenuItem subMenuItem;
            if (subMenuItems.Length == 0)
            {
                subMenuItem = (ToolStripMenuItem)mnuParent.DropDownItems.Add(name);
                subMenuItem.Name = "miColorSettings_" + name;

                if (colorData != null)
                {
                    property = colorData.GetType().GetDeclaredProperty(propertyName);
                    subMenuItem.Tag = property;
                }

                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    var item = subMenuItem.DropDownItems.Add(enumValue.ToString());
                    item.Tag = enumValue;
                    item.Click += clickEvent;
                }
            }
            else
            {
                subMenuItem = (ToolStripMenuItem)subMenuItems[0];
                property = (PropertyInfo)subMenuItem.Tag;
            }

            if (colorData == null)
            {
                return;
            }
            var value = property.GetValue(colorData);

            foreach (var item in subMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    var menuItem = (ToolStripMenuItem)item;
                    if (menuItem.Tag != null)
                    {
                        menuItem.Checked = menuItem.Tag.Equals(value);
                    }
                }
            }
        }

        public static void BuildComboBox<T>(ComboBox comboBox, params T[] skip) where T : IConvertible
        {
            if (comboBox.Items.Count == 0)
            {
                foreach (var enumValue in Enum.GetValues(typeof(T)))
                {
                    if (skip.Contains((T)enumValue))
                    {
                        continue;
                    }
                    var item = new EnumComboBoxItem 
                    {
                        Name = ((T)enumValue).GetDescription(),
                        Id = (int)enumValue
                    };
                    
                    comboBox.Items.Add(item);
                }
            }
        }

        public static int SetComboBoxEnumIndex(ComboBox comboBox, int value)
        {
            if (comboBox.Items.Count == 0)
            {
                return -1;
            }

            var index = 0;

            for (var i = 0; i < comboBox.Items.Count; i++)
            {
                var item = comboBox.Items[i] as EnumComboBoxItem;
                if (item.Id == value)
                {
                    index = i;
                    break;
                }
            }

            comboBox.SelectedIndex = index;

            return index;
        }

        public static T GetComboBoxEnumItem<T>(ComboBox comboBox) where T : IConvertible
        {
            if (comboBox.SelectedIndex == -1)
            {
                return (T)Enum.Parse(typeof(T), "None");
            }

            var item = comboBox.SelectedItem as EnumComboBoxItem;

            return (T)Enum.Parse(typeof(T), item.Id.ToString());
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

        public static void InitListView(ListView listView, IEnumerable<string> columns)
        {
            foreach (var name in columns)
            {
                var columnName = name;
                var parts = name.Split('|');

                var width = 120;
                if (parts.Length > 1)
                {
                    width = int.Parse(parts[1]);
                    columnName = parts[0];
                }

                var header = listView.Columns.Add(columnName);
                header.Width = width == 120 ? -2 : width;
            }
        }

        public static void AddOrUpdateListItem<T>(ListView listView, List<T> presets, Config config, T preset = null, ListViewItem specItem = null) where T : PresetBase
        {
            ListViewItem item = null;
            if (preset == null)
            {
                item = listView.SelectedItems[0];
                preset = (T)item.Tag;
            }
            else
            {
                item = specItem;
            }

            if (preset.id == 0)
            {
                preset.id = preset.GetHashCode();
            }

            var values = preset.GetDisplayValues(config);

            if (item == null)
            {
                item = listView.Items.Add(values[0]);
                item.Tag = preset;
                for (var i = 1; i < values.Count; i++)
                {
                    item.SubItems.Add(values[i]);
                }
                if (!presets.Any(x => x.id == preset.id))
                {
                    presets.Add(preset);
                }
            }
            else
            {
                item.Text = values[0];
                for (var i = 1; i < values.Count; i++)
                {
                    if (item.SubItems.Count == i)
                    {
                        item.SubItems.Add(values[i]);
                    }
                    else
                    {
                        item.SubItems[i].Text = values[i];
                    }
                }
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
            var currentProcessId = Process.GetCurrentProcess().Id;
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.Equals(name) && (!skipCurrent || process.Id != currentProcessId))
                {
                    return process;
                }
            }

            return null;
        }

        public static void StartProcess(string fileName, string arguments = null)
        {
            var process = Process.Start(fileName, arguments);
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

        public static bool IsForegroundFullScreenAndDisabledNotifications(Screen screen = null)
        {
            return IsNotificationDisabled() && IsForegroundFullScreen(screen);
        }

        public static bool IsNotificationDisabled()
        {
            var result = SHQueryUserNotificationState(out var state);

            if (result == NativeConstants.ERROR_SUCCESS && state != LastNotificationState)
            {
                Logger.Debug($"Detected NotificationState change from {LastNotificationState} to {state}");
                LastNotificationState = state;
            }

            return NotificationsDisabledStates.Contains(state);
            //return state != UserNotificationState.QUNS_ACCEPTS_NOTIFICATIONS && state != UserNotificationState.QUNS_QUIET_TIME;
        }

        public static bool IsForegroundFullScreen(Screen screen = null)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            var hWnd = NativeMethods.GetForegroundWindow();

            NativeMethods.GetWindowRect(hWnd, out var rect);

            /* in case you want the process name:
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);
            var proc = System.Diagnostics.Process.GetProcessById((int)procId);
            Console.WriteLine(proc.ProcessName);
            */

            return screen.Bounds.Width <= (rect.right - rect.left) && screen.Bounds.Height <= (rect.bottom - rect.top);
        }

        public static (uint, bool) GetForegroundProcessIdAndIfFullScreen(Screen screen = null)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            var hWnd = NativeMethods.GetForegroundWindow();

            if (hWnd == IntPtr.Zero)
            {
                return (0, false);
            }

            NativeMethods.GetWindowRect(hWnd, out var rect);

            GetWindowThreadProcessId(hWnd, out var processId);

            var isFullScreen = screen.Bounds.Width <= (rect.right - rect.left) && screen.Bounds.Height <= (rect.bottom - rect.top);

            return (processId, isFullScreen);
        }

        public static Process GetForegroundFullScreenProcess(Screen screen = null)
        {
            var (processId, isFullScreen) = GetForegroundProcessIdAndIfFullScreen(screen);

            if (!isFullScreen)
            {
                return null;
            }

            return Process.GetProcessById((int)processId);
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
    }

    public class EnumComboBoxItem
    {
        public string Name;
        public int Id;

        public override string ToString()
        {
            return Name;
        }
    }

    public static class ProcessExtensions
    {
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int)parentId.NextValue());
        }

        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }
    }
}
