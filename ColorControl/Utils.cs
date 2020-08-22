using NWin32.NativeTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;

namespace ColorControl
{
    class Utils
    {
        [Flags]
        public enum ModKeys : int
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        public static string PKEY_PNPX_IpAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12297";

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

        public static async Task<string> GetDeviceProperty(string deviceName, string propertyName)
        {
            string value = null;
            string name = null;
            var list = await DeviceInformation.FindAllAsync("", new List<string>());

            var reqs = new List<string>();
            reqs.Add(propertyName);

            foreach (var dev in list)
            {
                name = dev.Name;
                if (dev.IsEnabled && dev.Name.Contains(deviceName))
                {
                    //Debug.WriteLine(dev.Name);
                    //foreach (var key in dev.Properties.Keys)
                    //{
                    //    Debug.WriteLine($"* {key}: {dev.Properties[key]}");
                    //}

                    var guid = dev.Properties["System.Devices.DeviceInstanceId"];

                    var aqs = "System.Devices.DeviceInstanceId:=\"" + guid + "\"";
                    var list2 = await PnpObject.FindAllAsync(PnpObjectType.Device, reqs, aqs);

                    foreach (var dev2 in list2)
                    {
                        foreach (var key in dev2.Properties.Keys)
                        {
                            var obj = dev2.Properties[key];
                            string newValue = null;
                            if (obj is string[])
                            {
                                var arr = obj as string[];
                                if (arr.Length > 0)
                                {
                                    newValue = arr[0];
                                }
                            }
                            else
                            {
                                newValue = dev2.Properties[key].ToString();
                            }
                            if (newValue != value)
                            {
                                value = newValue;
                                Logger.Debug($"LG TV with name {dev.Name} found at {value}");
                            }
                            //Debug.WriteLine($"* {key}: {value}");
                        }
                    }

                }
            }
            return value;
        }

        public static async Task<List<PnpDev>> GetPnpDevices(string deviceName, string propertyName)
        {
            var devices = new List<PnpDev>();

            var list = await DeviceInformation.FindAllAsync("", new List<string>());

            var reqs = new List<string>();
            reqs.Add(propertyName);

            foreach (var dev in list)
            {
                string name = dev.Name;
                if (dev.IsEnabled && dev.Name.Contains(deviceName))
                {
                    var guid = dev.Properties["System.Devices.DeviceInstanceId"];

                    var aqs = "System.Devices.DeviceInstanceId:=\"" + guid + "\"";
                    var list2 = await PnpObject.FindAllAsync(PnpObjectType.Device, reqs, aqs);

                    foreach (var dev2 in list2)
                    {
                        foreach (var key in dev2.Properties.Keys)
                        {
                            var obj = dev2.Properties[key];
                            string value = null;
                            if (obj is string[])
                            {
                                var arr = obj as string[];
                                if (arr.Length > 0)
                                {
                                    value = arr[0];
                                }
                            }
                            else
                            {
                                value = dev2.Properties[key].ToString();
                            }
                            if (!devices.Any(x => x.name.Equals(name) && x.ipAddress.Equals(value)))
                            {
                                var device = new PnpDev(name, value);
                                devices.Add(device);
                            }
                        }
                    }

                }
            }
            return devices;
        }
    }
}
