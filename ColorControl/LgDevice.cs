using LgTv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl
{
    class LgDevice
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; private set; }
        public string IpAddress { get; private set; }
        public string MacAddress { get; private set; }
        public bool IsCustom { get; private set; }
        [JsonIgnore]
        public bool IsDummy { get; private set; }

        public bool PowerOnAfterStartup { get; set; }
        public bool PowerOnAfterResume { get; set; }
        public bool PowerOffOnShutdown { get; set; }
        public bool PowerOffOnStandby { get; set; }
        public bool PowerSwitchOnScreenSaver { get; set; }

        [JsonIgnore]
        private LgTvApi _lgTvApi;
        [JsonIgnore]
        private bool _justWokeUp;

        [JsonIgnore]
        private Dictionary<string, Func<bool>> _invokableActions = new Dictionary<string, Func<bool>>();

        [JsonConstructor]
        public LgDevice(string name, string ipAddress, string macAddress, bool isCustom = true, bool isDummy = false)
        {
            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
            IsCustom = isCustom;
            IsDummy = isDummy;

            _invokableActions.Add("WOL", new Func<bool>(Wake));
        }

        public override string ToString()
        {
            return (IsDummy ? string.Empty : (IsCustom ? "Custom: " : "Auto detect: ")) + $"{Name}" + (!string.IsNullOrEmpty(IpAddress) ? $" ({IpAddress})" : string.Empty);
        }

        public async Task<bool> Connect(int retries = 3)
        {
            try
            {
                DisposeConnection();
                _lgTvApi = await LgTvApi.CreateLgTvApi(IpAddress, retries);

                //Test();
                //_lgTvApi.Test3();
                //_lgTvApi.SetSystemSettings();
                return _lgTvApi != null;
            }
            catch (Exception ex)
            {
                string logMessage = ex.ToLogString(Environment.StackTrace);
                Logger.Error($"Error while connecting to {IpAddress}: {logMessage}");
                return false;
            }
        }

        public bool IsConnected()
        {
            return !_lgTvApi?.ConnectionClosed ?? false;
        }

        internal void DisposeConnection()
        {
            if (_lgTvApi != null)
            {
                _lgTvApi.Dispose();
                _lgTvApi = null;
            }
        }
        public async Task<bool> ExecutePreset(LgPreset preset, bool reconnect = false)
        {
            var hasApp = !string.IsNullOrEmpty(preset.appId);

            var hasWOL = preset.steps.Any(s => s.Equals("WOL", StringComparison.OrdinalIgnoreCase));

            if (hasWOL)
            {
                var connected = await WakeAndConnect(0);
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

        public async Task<IEnumerable<LgApp>> GetApps(bool force = false)
        {
            if (!await Connected(force))
            {
                Logger.Debug("Cannot refresh apps: no connection could be made");
                return new List<LgApp>();
            }

            return await _lgTvApi.GetApps(force);
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

        internal async Task<bool> TestConnection(int retries = 1)
        {
            if (!await Connected(true, retries))
            {
                return false;
            }

            try
            {
                await _lgTvApi.IsMuted();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("TestConnection: " + ex.ToLogString());
                return false;
            }
        }

        internal async Task<bool> WakeAndConnect(int wakeDelay = 5000, int connectDelay = 500)
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
                var result = Wake();
                if (!result)
                {
                    Logger.Debug("WOL failed");
                    return false;
                }
                Logger.Debug("WOL succeeded");
                await Task.Delay(connectDelay);
                result = await Connect();
                Logger.Debug("Connect succeeded: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("WakeAndConnectToSelectedDevice: " + e.ToLogString());
                return false;
            }
        }

        internal bool Wake()
        {
            var result = false;

            if (MacAddress != null)
            {
                result = WOL.WakeFunction(MacAddress);
                _justWokeUp = true;
            }
            else
            {
                Logger.Debug("Cannot wake device: the device has no MAC-address");
            }

            return result;
        }

        internal async Task<bool> WakeAndConnectWithRetries(int retries = 5)
        {
            var wakeDelay = 0;
            var maxRetries = retries <= 1 ? 5 : retries;

            var result = false;
            for (var retry = 0; retry < maxRetries && !result; retry++)
            {
                Logger.Debug($"WakeAndConnectWithRetries: attempt {retry + 1} of {maxRetries}...");

                var ms = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                result = await WakeAndConnect(retry == 0 ? wakeDelay : 0);
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

            if (result)
            {
                var resumeScript = Path.Combine(Program.DataDir, "ResumeScript.bat");
                if (File.Exists(resumeScript))
                {
                    Utils.StartProcess(resumeScript);
                }
            }

            return result;
        }

        public Dictionary<string, Func<bool>> GetInvokableActions()
        {
            return _invokableActions;
        }

        public async Task PowerOn()
        {
            var mouse = await _lgTvApi.GetMouse();
            mouse.SendButton(ButtonType.POWER);
        }

        public void Test()
        {
            try
            {
                //_lgTvApi.GetServiceList();
                //_lgTvApi?.Test();
            }
            catch (Exception ex)
            {
                Logger.Error("TEST: " + ex.ToLogString());
            }
        }

        private async Task<bool> Connected(bool reconnect = false, int retries = 3)
        {
            if (reconnect || !IsConnected() || !string.Equals(_lgTvApi.GetIpAddress(), IpAddress))
            {
                if (!await Connect(retries))
                {
                    Logger.Debug("Cannot apply LG-preset: no connection could be made");
                    return false;
                }
            }
            return true;
        }

        internal void ConvertToCustom()
        {
            IsCustom = true;
        }

        internal async Task SetBacklight(int backlight)
        {
            await _lgTvApi.SetSystemSettings("backlight", backlight.ToString());
        }

        public async Task SetOLEDMotionPro(string mode)
        {
            await _lgTvApi.SetConfig("tv.model.motionProMode", mode);
        }

        internal async Task SetConfig(string key, string value)
        {
            await _lgTvApi.SetConfig(key, value);
        }
    }
}
