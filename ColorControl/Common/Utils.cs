using ColorControl.Forms;
using ColorControl.Native;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NWin32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using Task = System.Threading.Tasks.Task;

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

        public const int WM_BRINGTOFRONT = NativeConstants.WM_USER + 1;

        public static string PKEY_PNPX_IpAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12297";
        public static string PKEY_PNPX_PhysicalAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12294";
        public static Guid GUID_CONSOLE_DISPLAY_STATE = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");

        public static bool ConsoleOpened { get; private set; }
        public static bool UseDedicatedElevatedProcess = false;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool WinKeyDown = false;
        private static Keys[] KeysWithoutModifiers = new[] { Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24 };

        public static string SERVICE_NAME = "Color Control Service";

        public static string ELEVATION_MSG = @"Elevation is needed in some cases where ColorControl needs administrator rights.
Some operations like installing a service, changing the priority of a process or creating a temporary IP-route for improved WOL-functionality will not work without those rights.

These methods are available:

- None: no administrator operations can be executed (unless program already manually is started as admin)

- Run as admin: only available when starting automatically after login (configures scheduled task with highest privileges)

- Use Windows Service: this will install a Windows Service that handles the operations which need admin rights. This is the preferred method.

- Use dedicated elevated process: a second ColorControl-process will be spawned that is run as administrator.

The best and suggested method to provide this is via a Windows Service. Only when installing the service a User Account Control window may popup.";

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

        private static bool? _IsAdministrator;

        public static bool IsAdministrator()
        {
            if (_IsAdministrator.HasValue)
            {
                return _IsAdministrator.Value;
            }

            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            _IsAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);

            return _IsAdministrator.Value;
        }

        internal static int ExecuteElevated(string args, bool wait = true, bool skipDedicated = false, bool skipService = false)
        {
            if (!skipService && IsServiceRunning())
            {
                var result = PipeUtils.SendRpcMessage(args);

                if (result.HasValue)
                {
                    return 0;
                }
            }

            if (UseDedicatedElevatedProcess && !skipDedicated)
            {
                CheckElevatedProcess();

                WinApi.CopyData.Send(ElevatedProcessWindowHandle, 1, args);

                return 0;
            }

            var fileName = Process.GetCurrentProcess().MainModule.FileName;

            var info = new ProcessStartInfo(fileName, args)
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

                if (wait)
                {
                    process.WaitForExit(); // sleep calling process thread until evoked process exit
                }

                return process.Id;
            }
            catch (Exception e)
            {
                Logger.Error("ExecuteElevated: " + e.Message);
            }
            return 0;
        }

        private static IntPtr ElevatedProcessWindowHandle = IntPtr.Zero;

        internal static void CheckElevatedProcess()
        {
            ElevatedProcessWindowHandle = NativeMethods.FindWindowW(null, "ElevatedForm");

            if (ElevatedProcessWindowHandle == IntPtr.Zero)
            {
                ExecuteElevated(StartUpParams.StartElevatedParam, false, true, true);

                var delay = 1000;

                while (delay >= 0)
                {
                    Thread.Sleep(100);

                    ElevatedProcessWindowHandle = NativeMethods.FindWindowW(null, "ElevatedForm");

                    if (ElevatedProcessWindowHandle != IntPtr.Zero)
                    {
                        Thread.Sleep(500);
                        break;
                    }

                    delay -= 100;
                }
            }
        }

        internal static bool IsChromeFixInstalled()
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command");
            return key != null && key.GetValue(null).ToString().Contains("--disable-lcd-text");
        }

        internal static bool InstallChromeFix(bool install, string applicationDataFolder)
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

            var roamingFolder = applicationDataFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

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

        public static void SetBrightness(IntPtr handle)
        {
            var displays = Windows.Graphics.Display.DisplayServices.FindAll();

            var folderPicker = new Windows.Storage.Pickers.FolderPicker();

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, handle);

            folderPicker.FileTypeFilter.Add("*");
            var folder = folderPicker.PickSingleFolderAsync().GetAwaiter();

            //var info = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            //var colorInfo = info.GetAdvancedColorInfo();
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

                return enumName;
            }

            return null; // could also return string.Empty
        }

        public static string RemoveFirstUnderscore(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text[0] == '_')
            {
                return text.Substring(1);
            }

            return text;
        }

        public static string GetEnumNameByDescription(Type enumType, string description)
        {
            if (enumType.BaseType == typeof(Enum))
            {
                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    if (description.Equals(GetDescription(enumType, enumValue as IConvertible), StringComparison.Ordinal))
                    {
                        return Enum.GetName(enumType, enumValue).RemoveFirstUnderscore();
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

        public static void RegisterTask(string taskName, bool enabled, TaskRunLevel runLevel = TaskRunLevel.LUA)
        {
            if (!IsAdministrator())
            {
                ExecuteElevated(enabled ? $"{StartUpParams.EnableAutoStartParam} {(int)runLevel}" : StartUpParams.DisableAutoStartParam, skipService: true);
                return;
            }

            var file = Process.GetCurrentProcess().MainModule.FileName;
            var directory = Path.GetDirectoryName(file);

            try
            {
                using (TaskService ts = new TaskService())
                {
                    if (enabled)
                    {
                        var td = ts.NewTask();

                        td.RegistrationInfo.Description = "Start ColorControl";
                        td.Principal.RunLevel = runLevel;

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
                Logger.Error($"Could not create/delete task: {e.Message}");
            }
        }

        public static bool TaskExists(string taskName)
        {
            var file = Assembly.GetExecutingAssembly().Location;
            using (TaskService ts = new TaskService())
            {
                var task = ts.RootFolder.Tasks.FirstOrDefault(x => x.Name.Equals(taskName));

                return task != null;
            }
        }

        public static bool RegisterShortcut(IntPtr handle, int id, string shortcut, bool clear = false)
        {
            if (clear)
            {
                WinApi.UnregisterHotKey(handle, id);
            }

            if (!string.IsNullOrEmpty(shortcut))
            {
                var (mods, key) = ParseShortcut(shortcut);
                var result = WinApi.RegisterHotKey(handle, id, mods, key);
                if (!result)
                {
                    var errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                    Logger.Error($"Could not register shortcut {shortcut}: {errorMessage}");
                }

                return result;
            }

            return true;
        }

        public static void CheckWin32Error(string message)
        {
            var errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;

            throw new Exception($"{message}: {errorMessage}");
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

            if (!WinApi.AttachConsole(-1))
            {
                WinApi.AllocConsole();

                SetConsoleWriter();

                return true;
            }
            ConsoleOpened = true;

            SetConsoleWriter();

            return false;
        }

        private static void SetConsoleWriter()
        {
            var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(writer);
        }

        public static bool CloseConsole()
        {
            if (!ConsoleOpened)
            {
                return false;
            }

            SendKeys.SendWait("{ENTER}");
            var result = WinApi.FreeConsole();

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

        public static Process StartProcess(string fileName, string arguments = null, bool hidden = false, bool wait = false, bool setWorkingDir = false, bool elevate = false, uint affinityMask = 0, uint priorityClass = 0)
        {
            var process = Process.Start(new ProcessStartInfo(fileName, arguments)
            {
                Verb = elevate ? "runas" : string.Empty, // indicates to elevate privileges
                UseShellExecute = true,
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                WorkingDirectory = setWorkingDir ? Path.GetDirectoryName(fileName) : null
            });

            if (affinityMask > 0)
            {
                SetProcessAffinity(process.Id, affinityMask);
            }

            if (priorityClass > 0 && priorityClass != NativeConstants.NORMAL_PRIORITY_CLASS)
            {
                SetProcessPriority(process.Id, priorityClass);
            }

            if (wait)
            {
                process.WaitForExit();
            }

            return process;
        }

        internal static void SetProcessAffinity(int processId, uint affinityMask)
        {
            if (!IsAdministrator())
            {
                ExecuteElevated($"{StartUpParams.SetProcessAffinityParam} {processId} {affinityMask}");

                return;
            }

            var hProcess = NativeMethods.OpenProcess(NativeConstants.PROCESS_ALL_ACCESS, false, (uint)processId);

            if (hProcess == IntPtr.Zero)
            {
                CheckWin32Error($"Unable to set processor affinity on process {processId}");
            }

            NativeMethods.SetProcessAffinityMask(hProcess, affinityMask);

            //var AffinityMask = (long)process.ProcessorAffinity;
            //AffinityMask &= 0x000F; // use only any of the first 4 available processors
            //process.ProcessorAffinity = (IntPtr)AffinityMask;
        }

        internal static void SetProcessPriority(int processId, uint priorityClass)
        {
            if (!IsAdministrator())
            {
                ExecuteElevated($"{StartUpParams.SetProcessPriorityParam} {processId} {priorityClass}");

                return;
            }

            var hProcess = NativeMethods.OpenProcess(NativeConstants.PROCESS_ALL_ACCESS, false, (uint)processId);

            if (hProcess == IntPtr.Zero)
            {
                CheckWin32Error($"Unable to set process priority on process {processId}");
            }

            NativeMethods.SetPriorityClass(hProcess, priorityClass);
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
            var openDialog = new System.Windows.Forms.OpenFileDialog();
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

        public static void StartService()
        {
            if (!IsAdministrator())
            {
                ExecuteElevated(StartUpParams.StartServiceParam);
                return;
            }

            var controller = new ServiceController(SERVICE_NAME);

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                controller.Start();
            }
        }

        public static void StopService()
        {
            if (!IsAdministrator())
            {
                ExecuteElevated(StartUpParams.StopServiceParam);
                return;
            }

            var controller = new ServiceController(SERVICE_NAME);

            if (controller.Status == ServiceControllerStatus.Running)
            {
                controller.Stop();
            }
        }

        public static bool IsServiceInstalled()
        {
            return GetServiceStatus() != 0;
        }

        public static bool IsServiceRunning()
        {
            return GetServiceStatus() == ServiceControllerStatus.Running;
        }

        public static ServiceControllerStatus GetServiceStatus()
        {
            var controller = new ServiceController(SERVICE_NAME);

            try
            {
                return controller.Status;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        internal static void InstallService()
        {
            if (!IsAdministrator())
            {
                ExecuteElevated(StartUpParams.InstallServiceParam, skipService: true);

                return;
            }


            if (!IsServiceInstalled())
            {
                StartProcess("sc.exe", @$"create ""{SERVICE_NAME}"" binpath=""{Process.GetCurrentProcess().MainModule.FileName}"" start=auto", wait: true);
                StartProcess("sc.exe", @$"description ""{SERVICE_NAME}"" ""Executes tasks for Color Control that require elevation. If this service is stopped, functions like WOL might be impacted.""", wait: true);
            }

            StartService();
        }

        internal static void UninstallService()
        {
            if (!IsAdministrator())
            {
                ExecuteElevated(StartUpParams.UninstallServiceParam, skipService: true);

                return;
            }

            if (!IsServiceInstalled())
            {
                return;
            }

            StopService();

            StartProcess("sc.exe", @$"delete ""{SERVICE_NAME}""", wait: true);
        }

        public static bool PingHost(string nameOrAddress)
        {
            try
            {
                using var pinger = new Ping();
                var reply = pinger.Send(nameOrAddress);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
                return false;
            }
        }

        public static bool WriteText(string fileName, string data)
        {
            var doWrite = !File.Exists(fileName);

            if (!doWrite)
            {
                var writtenData = File.ReadAllText(fileName);

                doWrite = !writtenData.Equals(data);
            }

            if (doWrite)
            {
                try
                {
                    File.WriteAllText(fileName, data);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToLogString());
                }
            }

            return doWrite;
        }

        public static string ReadText(string fileName, bool reverse = false)
        {
            var exists = File.Exists(fileName);

            if (!exists)
            {
                return null;
            }

            try
            {
                var lines = new StringBuilder();

                using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using var reader = new StreamReader(fs);

                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (reverse)
                    {
                        lines.Insert(0, line + "\r\n");
                        continue;
                    }

                    lines.AppendLine(line);
                }

                return lines.ToString();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }

            return null;
        }

        public static List<string> ReadLines(string fileName, bool reverse = false)
        {
            var exists = File.Exists(fileName);

            if (!exists)
            {
                return null;
            }

            try
            {
                var lines = new List<string>();

                using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using var reader = new StreamReader(fs);

                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (reverse)
                    {
                        lines.Insert(0, line + "\r\n");
                        continue;
                    }

                    lines.Add(line);
                }

                return lines;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }

            return null;
        }

        public static bool WriteObject(string fileName, object value)
        {
            var data = JsonConvert.SerializeObject(value);

            return WriteText(fileName, data);
        }

        public static T DeserializeJson<T>(string fileName)
        {
            try
            {
                var json = File.ReadAllText(fileName);
                var data = JsonConvert.DeserializeObject<T>(json);

                return data;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());

                return default;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static async Task DownloadFileAsync(string url, string filePath)
        {
            var httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync(url);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var fileStream = new FileStream(filePath, FileMode.CreateNew);

            await stream.CopyToAsync(fileStream);
        }

        public static void UnZipFile(string zipFile, string filePath)
        {
            ZipFile.ExtractToDirectory(zipFile, filePath);
        }

        public static void UpdateFiles(string clientPath, string updatePath)
        {
            var infos = new List<FileSystemInfo>();

            GetFileSystemInfos(updatePath, infos);

            foreach (var info in infos)
            {
                var subPath = info.FullName.Replace(updatePath, "");

                if (subPath[0] == '\\')
                {
                    subPath = subPath.Substring(1);
                }

                var targetPath = Path.Combine(clientPath, subPath);

                //Logger.Debug($"Path: {updatePath}, info: {info.FullName}, target: {targetPath}");

                if (info is DirectoryInfo)
                {
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    continue;
                }

                var fileInfo = info as FileInfo;

                var oldPath = targetPath + ".old";

                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }

                if (File.Exists(targetPath))
                {
                    if (CompareFiles(fileInfo.FullName, targetPath))
                    {
                        continue;
                    }

                    File.Move(targetPath, targetPath + ".old");
                }

                fileInfo.CopyTo(targetPath);
            }
        }

        public static void GetFileSystemInfos(string path, List<FileSystemInfo> infos)
        {
            var directory = new DirectoryInfo(path);

            foreach (var dir in directory.GetDirectories())
            {
                infos.Add(dir);
                GetFileSystemInfos(dir.FullName, infos);
            }

            foreach (var file in directory.GetFiles())
            {
                infos.Add(file);
            }
        }

        public static string SHA256CheckSum(string filePath)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                    return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
            }
        }

        public static bool CompareFiles(string filePath1, string filePath2)
        {
            return SHA256CheckSum(filePath1) == SHA256CheckSum(filePath2);
        }

        public static string ToUnitString(this uint value, int div = 1000, string units = "MHz") => $"{value / div}{units}";
        public static string ToUnitString(this int value, int div = 1000, string units = "MHz") => $"{value / div}{units}";
    }
}
