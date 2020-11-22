using LgTv;
using Newtonsoft.Json;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Data.Json;

namespace ColorControl
{
    class LgService
    {
        public static string LgListAppsJson = "listApps.json";

        public static string LgDeviceSearchKey = "[LG]";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string FriendlyScreenName { get; private set; }

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

        public LgService(string dataDir, bool allowPowerOn)
        {
            _dataDir = dataDir;
            _allowPowerOn = allowPowerOn;
            LoadConfig();

            foreach (var screen in Screen.AllScreens)
            {
                var name = screen.DeviceFriendlyName();
                if (name.Contains("LG"))
                {
                    FriendlyScreenName = name;
                }
            }

            RefreshDevices();
        }

        ~LgService()
        {
            SaveConfig();
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

        public void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(Config);
            File.WriteAllText(_configFilename, json);
        }

        public void RefreshDevices(Action callBack = null)
        {
            Utils.GetPnpDevices(LgDeviceSearchKey).ContinueWith((task) =>
            {
                SetDevicesTask(task);
                callBack?.Invoke();
                if (callBack == null && SelectedDevice != null)
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
            );
        }

        private void SetDevicesTask(Task<List<PnpDev>> task)
        {
            Devices = task.Result;
            if (Devices?.Count > 0)
            {
                var preferredDevice = Devices.FirstOrDefault(x => x.MacAddress.Equals(Config.PreferredMacAddress)) ?? Devices[0];

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

        internal void WakeSelectedDevice()
        {
            if (SelectedDevice != null)
            {
                WOL.WakeFunction(SelectedDevice.MacAddress);
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
