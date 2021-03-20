using LgTv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ColorControl
{
    class LgService
    {
        public static string LgListAppsJson = "listApps.json";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //public string FriendlyScreenName { get; private set; }

        public List<PnpDev> Devices { get; private set; }
        public PnpDev SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                Config.PreferredMacAddress = _selectedDevice != null ? _selectedDevice.MacAddress : null;
            }
        }

        public LgServiceConfig Config { get; private set; }

        private List<LgPreset> _lgPresets;
        private string _lgPresetsFilename;

        private LgTvApi _lgTvApi;
        private string _dataDir;
        private string _configFilename;
        private bool _allowPowerOn;
        private bool _justWokeUp;
        private PnpDev _selectedDevice;
        private string _rcButtonsFilename;
        private List<LgPreset> _remoteControlButtons;
        private List<LgApp> _lgApps = new List<LgApp>();

        private JavaScriptSerializer _JsonSerializer = new JavaScriptSerializer();
        private JavaScriptSerializer _JsonDeserializer = new JavaScriptSerializer();

        private Dictionary<string, Func<bool>> _invokableActions = new Dictionary<string, Func<bool>>();

        public LgService(string dataDir, bool allowPowerOn)
        {
            _dataDir = dataDir;
            _allowPowerOn = allowPowerOn;

            _invokableActions.Add("WOL", new Func<bool>(WakeSelectedDevice));

            LgPreset.LgApps = _lgApps;

            LoadConfig();
            LoadPresets();
            LoadRemoteControlButtons();

            //foreach (var screen in Screen.AllScreens)
            //{
            //    var name = screen.DeviceFriendlyName();
            //    if (name.Contains("LG"))
            //    {
            //        FriendlyScreenName = name;
            //    }
            //}

            var _ = RefreshDevices(afterStartUp: true);
        }

        ~LgService()
        {
            GlobalSave();
        }

        public void GlobalSave()
        {
            SaveConfig();
            SavePresets();
            SaveRemoteControlButtons();
        }

        public List<LgPreset> GetPresets()
        {
            return _lgPresets;
        }

        public LgPreset CreateNewPreset()
        {
            var preset = new LgPreset();
            var name = "New preset";
            string fullname;
            var number = 1;
            do
            {
                fullname = $"{name} ({number})";
                number++;
            } while (_lgPresets.Any(x => x.name.Equals(fullname)));

            preset.name = fullname;

            return preset;
        }

        private void LoadPresets()
        {
            _lgPresetsFilename = Path.Combine(_dataDir, "LgPresets.json");
            var toCopy = Path.Combine(Directory.GetCurrentDirectory(), "LgPresets.json");
            if (!File.Exists(_lgPresetsFilename) && File.Exists(toCopy))
            {
                try
                {
                    File.Copy(toCopy, _lgPresetsFilename);
                }
                catch (Exception e)
                {
                    Logger.Error($"Error while copying {toCopy} to {_lgPresetsFilename}: {e.Message}");
                }
            }

            if (File.Exists(_lgPresetsFilename))
            {
                var json = File.ReadAllText(_lgPresetsFilename);

                _lgPresets = _JsonDeserializer.Deserialize<List<LgPreset>>(json);
            }
            else
            {
                _lgPresets = new List<LgPreset>();
            }
        }

        private void LoadConfig()
        {
            _configFilename = Path.Combine(_dataDir, "LgConfig.json");
            if (File.Exists(_configFilename))
            {
                Config = JsonConvert.DeserializeObject<LgServiceConfig>(File.ReadAllText(_configFilename));
            }
            else
            {
                Config = new LgServiceConfig();
            }
        }

        private void LoadRemoteControlButtons()
        {
            var defaultButtons = GenerateDefaultRemoteControlButtons();

            _rcButtonsFilename = Path.Combine(_dataDir, "LgRemoteControlButtons.json");
            //if (File.Exists(_rcButtonsFilename))
            //{
            //    var json = File.ReadAllText(_rcButtonsFilename);

            //    _remoteControlButtons = _JsonDeserializer.Deserialize<List<LgPreset>>(json);

            //    var missingButtons = defaultButtons.Where(b => !_remoteControlButtons.Any(x => x.name.Equals(b.name)));
            //    _remoteControlButtons.AddRange(missingButtons);
            //}
            //else
            {
                _remoteControlButtons = defaultButtons;
            }
        }

        public List<LgPreset> GetRemoteControlButtons()
        {
            return _remoteControlButtons;
        }

        public Dictionary<string, Func<bool>> GetInvokableActions()
        {
            return _invokableActions;
        }

        private List<LgPreset> GenerateDefaultRemoteControlButtons()
        {
            var list = new List<LgPreset>();

            for (var i = 1; i < 11; i++)
            {
                var button = i < 10 ? i : 0;
                list.Add(GeneratePreset(button.ToString()));
            }

            list.Add(GeneratePreset("Settings", appId: "com.palm.app.settings", key: Keys.S));

            list.Add(GeneratePreset("Vol +", step: "VOLUMEUP", key: Keys.VolumeUp));
            list.Add(GeneratePreset("Vol -", step: "VOLUMEDOWN", key: Keys.VolumeDown));
            list.Add(GeneratePreset("Mute", step: "MUTE", key: Keys.VolumeMute));

            list.Add(GeneratePreset("Up", step: "UP", key: Keys.Up));
            list.Add(GeneratePreset("Down", step: "DOWN", key: Keys.Down));
            list.Add(GeneratePreset("Left", step: "LEFT", key: Keys.Left));
            list.Add(GeneratePreset("Right", step: "RIGHT", key: Keys.Right));

            list.Add(GeneratePreset("Enter", step: "ENTER", key: Keys.Enter));
            list.Add(GeneratePreset("Back", step: "BACK", key: Keys.Back));
            list.Add(GeneratePreset("Exit", step: "EXIT", key: Keys.Escape));

            return list;
        }

        private LgPreset GeneratePreset(string name, string step = null, string appId = null, Keys key = Keys.None)
        {
            var preset = new LgPreset();

            var cvt = new KeysConverter();
            var shortcut = key != Keys.None ? (string)cvt.ConvertTo(key, typeof(string)) : null;

            preset.name = name;
            preset.appId = appId;
            preset.shortcut = shortcut;
            if (appId == null)
            {
                preset.steps.Add(step == null ? name : step);
            }

            return preset;
        }

        private void SavePresets()
        {
            try
            {
                var json = _JsonSerializer.Serialize(_lgPresets);
                File.WriteAllText(_lgPresetsFilename, json);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        private void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(Config);
            File.WriteAllText(_configFilename, json);
        }

        private void SaveRemoteControlButtons()
        {
            var json = JsonConvert.SerializeObject(_remoteControlButtons);
            File.WriteAllText(_rcButtonsFilename, json);
        }

        public async Task RefreshDevices(bool connect = true, bool afterStartUp = false)
        {
            var devices = await Utils.GetPnpDevices(Config.DeviceSearchKey);
            SetDevices(devices);

            if (afterStartUp && SelectedDevice == null && Config.PowerOnAfterStartup && !string.IsNullOrEmpty(Config.PreferredMacAddress))
            {
                Logger.Debug("No device has been found, trying to wake it first...");

                WakeSelectedDevice(Config.PreferredMacAddress);

                await Task.Delay(4000);
                await RefreshDevices();

                return;
            }

            if (connect && SelectedDevice != null)
            {
                if (Config.PowerOnAfterStartup && _allowPowerOn)
                {
                    var _ = WakeAndConnectToSelectedDeviceWithRetries();
                }
                else
                {
                    var _ = ConnectToSelectedDevice();
                }
            }
        }

        private void SetDevices(List<PnpDev> devices)
        {
            Devices = devices;
            if (Devices?.Count > 0)
            {
                var preferredDevice = Devices.FirstOrDefault(x => x.MacAddress != null && x.MacAddress.Equals(Config.PreferredMacAddress)) ?? Devices[0];

                SelectedDevice = preferredDevice;
            }
        }

        public async Task<bool> ConnectToSelectedDevice(int retries = 3)
        {
            try
            {
                DisposeConnection();
                _lgTvApi = await LgTvApi.CreateLgTvApi(SelectedDevice.IpAddress, retries);

                //_lgTvApi.GetServiceList();
                return _lgTvApi != null;
            }
            catch (Exception ex)
            {
                string logMessage = ex.ToLogString(Environment.StackTrace);
                Logger.Error($"Error while connecting to {SelectedDevice.IpAddress}: {logMessage}");
                return false;
            }
        }

        private async Task<bool> Connected(bool reconnect = false)
        {
            if (SelectedDevice == null)
            {
                Logger.Debug("Cannot apply LG-preset: no device has been selected");
                return false;
            }

            if (reconnect || _lgTvApi == null || _lgTvApi.ConnectionClosed || !string.Equals(_lgTvApi.GetIpAddress(), SelectedDevice.IpAddress))
            {
                if (!await ConnectToSelectedDevice())
                {
                    Logger.Debug("Cannot apply LG-preset: no connection could be made");
                    return false;
                }
            }
            return true;
        }


        public async Task<bool> ApplyPreset(LgPreset preset, bool reconnect = false)
        {
            var hasApp = !string.IsNullOrEmpty(preset.appId);

            var hasWOL = preset.steps.Any(s => s.Equals("WOL", StringComparison.OrdinalIgnoreCase));

            if (hasWOL)
            {
                var connected = await WakeAndConnectToSelectedDevice(0);
                if (!connected)
                {
                    return false;
                }
            }

            for (var tries = 0; tries <= 1; tries++)
            {
                if (!await Connected(reconnect || tries == 1))
                {
                    return false;
                }

                if (hasApp)
                {
                    try
                    {
                        await _lgTvApi.LaunchApp(preset.appId);
                    }
                    catch (Exception ex)
                    {
                        string logMessage = ex.ToLogString(Environment.StackTrace);
                        Logger.Error("Error while launching app: " + logMessage);

                        if (tries == 0)
                        {
                            continue;
                        }
                        return false;
                    }

                    if (_justWokeUp)
                    {
                        _justWokeUp = false;
                        await Task.Delay(1000);
                    }
                }

                if (preset.steps.Any())
                {
                    if (hasApp)
                    {
                        await Task.Delay(1500);
                    }
                    try
                    {
                        await ExecuteSteps(_lgTvApi, preset);
                    }
                    catch (Exception ex)
                    {
                        string logMessage = ex.ToLogString(Environment.StackTrace);
                        Logger.Error("Error while executing steps: " + logMessage);

                        if (tries == 0)
                        {
                            continue;
                        }
                        return false;
                    }
                }

                return true;
            }

            return true;
        }

        public async Task PowerOn()
        {
            var mouse = await _lgTvApi.GetMouse();
            mouse.SendButton(ButtonType.POWER);
        }

        private async Task ExecuteSteps(LgTvApi api, LgPreset preset)
        {
            var mouse = await api.GetMouse();
            foreach (var step in preset.steps)
            {
                var keySpec = step.Split(':');
                var key = keySpec[0].ToUpper();
                if (_invokableActions.ContainsKey(key))
                {
                    continue;
                }

                if (keySpec.Length == 2)
                {
                    SendKey(mouse, key, int.Parse(keySpec[1]));
                }
                else
                {
                    SendKey(mouse, key);
                }
            }
        }

        private void SendKey(LgWebOsMouseService mouse, string key, int delay = 180)
        {
            if (key.Length == 1 && int.TryParse(key, out _))
            {
                key = "_" + key;
            }
            var button = (ButtonType)Enum.Parse(typeof(ButtonType), key);
            mouse.SendButton(button);
            Thread.Sleep(delay);
        }

        public async Task<LgWebOsMouseService> GetMouseAsync()
        {
            return await _lgTvApi.GetMouse();
        }

        public async Task RefreshApps(bool force = false)
        {
            if (SelectedDevice == null)
            {
                Logger.Debug("Cannot refresh apps: no device has been selected");
            }

            if (!await Connected(force))
            {
                Logger.Debug("Cannot refresh apps: no connection could be made");
            }

            _lgApps.Clear();
            _lgApps.AddRange(await _lgTvApi.GetApps(force));
        }

        public List<LgApp> GetApps()
        {
            return _lgApps;
        }

        internal void DisposeConnection()
        {
            if (_lgTvApi != null)
            {
                _lgTvApi.Dispose();
                _lgTvApi = null;
            }
        }

        internal async Task<bool> PowerOff()
        {
            if (!await Connected(true))
            {
                return false;
            }

            await _lgTvApi.TurnOff();
            return true;
        }

        internal bool WakeSelectedDevice()
        {
            return WakeSelectedDevice(SelectedDevice?.MacAddress ?? Config.PreferredMacAddress);
        }

        internal bool WakeSelectedDevice(string macAddress)
        {
            var result = false;

            macAddress = string.IsNullOrEmpty(macAddress) ? SelectedDevice?.MacAddress : macAddress;
            if (macAddress != null)
            {
                result = WOL.WakeFunction(macAddress, Config.UseOldNpcapWol);
                _justWokeUp = true;
            }
            else
            {
                Logger.Debug("Cannot wake device: no device has been selected or there is no preferred MAC-address stored in the configuration");
            }

            return result;
        }

        internal void WakeAfterResume()
        {
            if (Config.PowerOnAfterResume)
            {
                var _ = WakeAndConnectToSelectedDeviceWithRetries();
            }
        }

        private async Task<bool> WakeAndConnectToSelectedDeviceWithRetries()
        {
            var wakeDelay = 0;
            var maxRetries = Config.PowerOnRetries <= 1 ? 5 : Config.PowerOnRetries;

            var result = false;
            for (var retry = 0; retry < maxRetries && !result; retry++)
            {
                Logger.Debug($"WakeAndConnectToSelectedDeviceWithRetries: attempt {retry + 1} of {maxRetries}...");

                var ms = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                result = await WakeAndConnectToSelectedDevice(retry == 0 ? wakeDelay : 0);
                ms = DateTimeOffset.Now.ToUnixTimeMilliseconds() - ms;
                if (!result)
                {
                    var delay = 2000 - ms;
                    if (delay > 0)
                    {
                        await Task.Delay((int)delay);
                    }
                }
            }

            return result;
        }

        private async Task<bool> WakeAndConnectToSelectedDevice(int wakeDelay = 5000, int connectDelay = 500)
        {
            try
            {
                //if (wakeDelay == 0)
                //{
                //    if (await ConnectToSelectedDevice())
                //    {
                //        Logger.Debug("Already connected, no wake needed?");
                //        return;
                //    };
                //}

                await Task.Delay(wakeDelay);
                var result = WakeSelectedDevice();
                if (!result)
                {
                    Logger.Debug("WOL failed");
                    return false;
                }
                Logger.Debug("WOL succeeded");
                await Task.Delay(connectDelay);
                result = await ConnectToSelectedDevice();
                Logger.Debug("Connect succeeded: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("WakeAndConnectToSelectedDevice: " + e.ToLogString());
                return false;
            }
        }
    }
}
