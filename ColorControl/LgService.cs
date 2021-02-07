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

        private LgTvApi _lgTvApi;
        private string _dataDir;
        private string _configFilename;
        private bool _allowPowerOn;
        private bool _justWokeUp;
        private PnpDev _selectedDevice;
        private string _rcButtonsFilename;
        private List<LgPreset> _remoteControlButtons;
        private JavaScriptSerializer _JsonDeserializer = new JavaScriptSerializer();

        public LgService(string dataDir, bool allowPowerOn)
        {
            _dataDir = dataDir;
            _allowPowerOn = allowPowerOn;
            LoadConfig();
            LoadRemoteControlButtons();

            //foreach (var screen in Screen.AllScreens)
            //{
            //    var name = screen.DeviceFriendlyName();
            //    if (name.Contains("LG"))
            //    {
            //        FriendlyScreenName = name;
            //    }
            //}

            RefreshDevices(afterStartUp: true);
        }

        ~LgService()
        {
            SaveConfig();
            SaveRemoteControlButtons();
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

        public void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(Config);
            File.WriteAllText(_configFilename, json);
        }

        public void SaveRemoteControlButtons()
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
                    WakeAndConnectToSelectedDevice(0);
                }
                else
                {
                    ConnectToSelectedDevice();
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
                if (keySpec.Length == 2)
                {
                    SendKey(mouse, keySpec[0], int.Parse(keySpec[1]));
                }
                else
                {
                    SendKey(mouse, keySpec[0]);
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

        public async Task<List<LgApp>> RefreshApps(bool force = false)
        {
            if (SelectedDevice == null)
            {
                Logger.Debug("Cannot refresh apps: no device has been selected");
                return null;
            }

            if (!await Connected(force))
            {
                Logger.Debug("Cannot refresh apps: no connection could be made");
                return null;
            }

            var list = await _lgTvApi.GetApps(force);

            return list.ToList();
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

        internal void WakeSelectedDevice(string macAddress = null)
        {
            macAddress = macAddress == null ? SelectedDevice?.MacAddress : macAddress;
            if (macAddress != null)
            {
                WOL.WakeFunction(macAddress, !Config.UseAlternateWol);
                _justWokeUp = true;
            }
            else
            {
                Logger.Debug("Cannot wake device: no device has been selected");
            }
        }

        internal void WakeAfterResume()
        {
            if (Config.PowerOnAfterResume)
            {
                WakeAndConnectToSelectedDevice(Config.PowerOnDelayAfterResume);
            }
        }

        private void WakeAndConnectToSelectedDevice(int wakeDelay = 5000, int connectDelay = 1000)
        {
            Task.Run(async () =>
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
                    WakeSelectedDevice();
                    await Task.Delay(connectDelay);
                    await ConnectToSelectedDevice();
                }
                catch (Exception e)
                {
                    Logger.Error("WakeAndConnectToSelectedDevice: " + e.ToLogString());
                }
            });
        }
    }
}
