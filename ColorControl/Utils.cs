using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static string PKEY_PNPX_IpAddress        = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12297";
        public static string PKEY_PNPX_PhysicalAddress  = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12294";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public static Bitmap SubPixelShift(Bitmap bitmap)
        {
            Bitmap bitmap2 = (Bitmap) bitmap.Clone();

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
            var info = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, args)
            {
                Verb = "runas", // indicates to elevate privileges
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

            var reqs = new List<string>();
            reqs.Add(PKEY_PNPX_IpAddress);
            reqs.Add(PKEY_PNPX_PhysicalAddress);

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

        public static void WaitForTask(Task task)
        {
            while (task != null && (task.Status < TaskStatus.WaitingForChildrenToComplete))
            {
                Thread.Sleep(100);
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
            if (e is Enum)
            {
                var type = e.GetType();
                var values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }

            return null; // could also return string.Empty
        }

        public static List<string> GetDescriptions<T>() where T : IConvertible
        {
            var list = new List<string>();
            foreach (var enumValue in Enum.GetValues(typeof(T)))
            {
                list.Add(((T)enumValue).GetDescription());
            }
            return list;
        }
    }
}
