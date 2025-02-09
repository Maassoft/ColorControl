using ColorControl.Shared.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NStandard;
using NWin32;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace ColorControl.Shared.Common
{
    public static class Utils
    {
        public const int WM_BRINGTOFRONT = NativeConstants.WM_USER + 1;

        public static string PKEY_PNPX_IpAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12297";
        public static string PKEY_PNPX_PhysicalAddress = "{656a3bb3-ecc0-43fd-8477-4ae0404a96cd} 12294";
        public static string PKEY_DEVICE_CATEGORY = "{78c34fc8-104a-4aca-9ea4-524d52996e57} 90";
        public static string PKEY_DeviceContainer_Category = "DEVPKEY_DeviceContainer_Category";
        public static string DEV_CLASS_DISPLAY_TV_LCD = "Display.TV.LCD";
        public static Guid GUID_CONSOLE_DISPLAY_STATE = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");

        public static bool ConsoleOpened { get; private set; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

        public static string[] GetDeviceProperty(ManagementObject device, string propertyName)
        {
            string[] values = null;

            var args = new object[] { new string[] { propertyName }, null };
            device.InvokeMethod("GetDeviceProperties", args);

            // one mbo for each device property key
            var mbos = (ManagementBaseObject[])args[1];
            if (mbos.Length > 0)
            {
                // get value of property named "Data"
                // not all objects have that so we enum all props here
                var data = mbos[0].Properties.OfType<PropertyData>().FirstOrDefault(p => p.Name == "Data")?.Value;
                if (data != null)
                {
                    if (data is string[] strValues)
                    {
                        values = strValues.Where(s => !s.IsNullOrWhiteSpace()).ToArray();
                    }
                    else if (data is byte[])
                    {
                        values = [BitConverter.ToString((byte[])data)];
                    }
                    else
                    {
                        values = [data?.ToString()];
                    }
                }
            }

            return values;
        }

        public static async Task<List<PnpDev>> GetPnpDevices(string deviceName, string category = null)
        {
            var devices = new List<PnpDev>();

            var queryString = $"SELECT * FROM Win32_PnPEntity WHERE PNPClass='DigitalMediaDevices' AND NAME LIKE '%{deviceName}%'";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(queryString);

            foreach (var mo in mos.Get().OfType<ManagementObject>())
            {
                //foreach (var prop in mo.Properties)
                //{
                //    Debug.WriteLine($"{prop.Name}: {prop.Value}");
                //}

                if (category != null)
                {
                    var devCategory = GetDeviceProperty(mo, PKEY_DeviceContainer_Category);

                    if (devCategory != null && !devCategory.Contains(category))
                    {
                        continue;
                    }
                }

                var name = mo["Name"] as string;
                var ipAddress = GetDeviceProperty(mo, PKEY_PNPX_IpAddress)?.FirstOrDefault();
                var macAddress = GetDeviceProperty(mo, PKEY_PNPX_PhysicalAddress)?.FirstOrDefault();

                if (!devices.Any(x => x.Name.Equals(name) && x.IpAddress != null && x.IpAddress.Equals(ipAddress)))
                {
                    var device = new PnpDev(false, name, ipAddress, macAddress);
                    devices.Add(device);
                }
            }

            return devices;
        }

        public static Image GenerateGradientBitmap(int width, int height)
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

        public static Dictionary<T, string> EnumToDictionary<T>(IEnumerable<T> skipValues = null) where T : struct, Enum
        {
            return Enum.GetValues<T>().Where(v => skipValues == null || !skipValues.Contains(v)).ToDictionary(k => k, e => e.GetDescription());
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

        public static T GetEnumValueByDescription<T>(string value) where T : struct
        {
            var enumName = GetEnumNameByDescription(typeof(T), value);
            if (enumName != null)
            {
                value = enumName;
            }

            return Enum.Parse<T>(value);
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

        public static List<string> GetDescriptions(Type enumType, int value = -1, int fromValue = 0, bool replaceUnderscore = false)
        {
            var list = new List<string>();
            foreach (var enumValue in Enum.GetValues(enumType))
            {
                if (value > -1 || fromValue > 0)
                {
                    var enumIntValue = Convert.ToInt32(enumValue);

                    if (enumIntValue < fromValue || value >= 0 && (enumIntValue & value) == 0)
                    {
                        continue;
                    }
                }

                list.Add(GetDescription(enumType, enumValue as IConvertible) ?? (replaceUnderscore ? enumValue.ToString().Replace("_", "") : enumValue.ToString()));
            }
            return list;
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

        public static string GetResourceFile(string resourceName)
        {
            resourceName = "ColorControl.Resources." + resourceName;

            var assembly = Assembly.GetEntryAssembly();

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

        public static uint ParseUInt(string value, uint def = 0)
        {
            if (uint.TryParse(value, out var result))
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

        public static async Task<dynamic> GetRestJsonAsync(string url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<dynamic>(result);

                return data;
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

        public static async Task<bool> PingHost(string nameOrAddress)
        {
            try
            {
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(nameOrAddress, 2000);

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

        public static string ToKiloUnitString(this uint value, int div = 1000, string units = "MHz") => $"{value / div}{units}";
        public static string ToKiloUnitString(this int value, int div = 1000, string units = "MHz") => $"{value / div}{units}";
        public static string ToUnitString(this uint value, string units = "MHz") => $"{value}{units}";
        public static string ToSignedUnitString(this int value, string units = "MHz") => $"{(value >= 0 ? "+" + value : value)}{units}";
        public static string ToSignedUnitString(this uint value, string units = "MHz") => $"{(value >= 0 ? "+" + value : value)}{units}";
        public static string ToUnitString(string units = "°", params double?[] values)
        {
            var notNullValues = values.Where(v => v.HasValue).Select(v => $"{v}{units}");

            return string.Join("/", notNullValues);
        }

        public static System.Windows.Application EnsureApplication()
        {
            return System.Windows.Application.Current ?? new System.Windows.Application { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
        }
    }
}
