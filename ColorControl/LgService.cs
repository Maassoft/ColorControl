using LgTv;
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
        public static string LgControllerExe = "LgController.exe";
        public static string LgListAppsJson = "listApps.json";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string FriendlyScreenName { get; private set; }

        public List<PnpDev> Devices { get; private set; }
        public PnpDev SelectedDevice { get; set; }

        private LgTvApi _lgTvApi;

        public LgService()
        {
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
        }

        public void RefreshDevices(Action callBack = null)
        {
            Utils.GetPnpDevices("[LG]", Utils.PKEY_PNPX_IpAddress).ContinueWith((task) =>
            {
                SetDevicesTask(task);
                callBack?.Invoke();
                if (callBack == null && SelectedDevice != null)
                {
                    ConnectToSelectedDevice();
                }
            }
            );
        }

        private void SetDevicesTask(Task<List<PnpDev>> task)
        {
            Devices = task.Result;
            if (Devices?.Count > 0)
            {
                SelectedDevice = Devices[0];
            }
        }

        public async Task<bool> ConnectToSelectedDevice(int retries = 1)
        {
            try
            {
                DisposeConnection();
                _lgTvApi = await LgTvApi.CreateLgTvApi(SelectedDevice.ipAddress, retries);
                return true;
            }
            catch (Exception ex)
            {
                string logMessage = ex.ToLogString(Environment.StackTrace);
                Logger.Error($"Error while connecting to {SelectedDevice.ipAddress}: {logMessage}");
                return false;
            }
        }

        public async Task<bool> ApplyPreset(LgPreset preset, bool reconnect = false)
        {
            if (SelectedDevice == null)
            {
                Logger.Debug("Cannot apply LG-preset: no device has been selected");
                return false;
            }

            if (reconnect || _lgTvApi == null || !string.Equals(_lgTvApi.GetIpAddress(), SelectedDevice.ipAddress))
            {
                if (!await ConnectToSelectedDevice())
                {
                    Logger.Debug("Cannot apply LG-preset: no connection could be made");
                    return false;
                }
            }

            var hasApp = !string.IsNullOrEmpty(preset.appId);

            if (hasApp)
            {
                await _lgTvApi.LaunchApp(preset.appId);
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
                    return false;
                }
            }

            return true;
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

            var api = await LgTvApi.CreateLgTvApi(SelectedDevice.ipAddress);
            var list = await api.GetApps(force);

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
    }
}
