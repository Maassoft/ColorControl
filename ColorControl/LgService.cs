﻿using LgTv;
using Microsoft.Win32;
using Newtonsoft.Json;
using NWin32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    class LgService : ServiceBase<LgPreset>
    {
        internal enum PowerOnOffState
        {
            StartUp = 1,
            Resume = 2,
            ScreenSaver = 3,
            ShutDown = 4,
            StandBy = 5
        }
    
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string LgListAppsJson = "listApps.json";

        public List<LgDevice> Devices { get; private set; }
        public LgDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                Config.PreferredMacAddress = _selectedDevice != null ? _selectedDevice.MacAddress : null;
            }
        }

        public LgServiceConfig Config { get; private set; }

        private string _configFilename;
        private bool _allowPowerOn;
        private LgDevice _selectedDevice;
        private string _rcButtonsFilename;
        private List<LgPreset> _remoteControlButtons;
        private List<LgApp> _lgApps = new List<LgApp>();

        private bool _poweredOffByScreenSaver;
        private int _poweredOffByScreenSaverProcessId;
        private Task _monitorTask;
        private int _monitorTaskCounter;

        public LgService(string dataPath, bool allowPowerOn) : base(dataPath)
        {
            _allowPowerOn = allowPowerOn;

            LgPreset.LgApps = _lgApps;

            LoadConfig();
            LoadPresets();
            LoadRemoteControlButtons();
        }

        ~LgService()
        {
            GlobalSave();
        }

        public static async Task<bool> ExecutePresetAsync(string presetName)
        {
            try
            {
                var lgService = new LgService(Program.DataDir, true);
                await lgService.RefreshDevices(afterStartUp: true);

                var result = await lgService.ApplyPreset(presetName);

                if (!result)
                {
                    Console.WriteLine("Preset not found or error while executing.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing preset: " + ex.ToLogString());
                return false;
            }
        }

        public void GlobalSave()
        {
            SaveConfig();
            SavePresets();
            SaveRemoteControlButtons();
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
            } while (_presets.Any(x => x.name.Equals(fullname)));

            preset.name = fullname;

            return preset;
        }

        private void LoadPresets()
        {
            _presetsFilename = Path.Combine(_dataPath, "LgPresets.json");
            var toCopy = Path.Combine(Directory.GetCurrentDirectory(), "LgPresets.json");
            if (!File.Exists(_presetsFilename) && File.Exists(toCopy))
            {
                try
                {
                    File.Copy(toCopy, _presetsFilename);
                }
                catch (Exception e)
                {
                    Logger.Error($"Error while copying {toCopy} to {_presetsFilename}: {e.Message}");
                }
            }

            if (File.Exists(_presetsFilename))
            {
                var json = File.ReadAllText(_presetsFilename);

                _presets = _JsonSerializer.Deserialize<List<LgPreset>>(json);
            }
            else
            {
                _presets = new List<LgPreset>();
            }
        }

        private void LoadConfig()
        {
            _configFilename = Path.Combine(_dataPath, "LgConfig.json");
            if (File.Exists(_configFilename))
            {
                Config = JsonConvert.DeserializeObject<LgServiceConfig>(File.ReadAllText(_configFilename));
            }
            else
            {
                Config = new LgServiceConfig();
            }

            LgPreset.LgDevices = Config.Devices;
        }

        private void LoadRemoteControlButtons()
        {
            var defaultButtons = GenerateDefaultRemoteControlButtons();

            _rcButtonsFilename = Path.Combine(_dataPath, "LgRemoteControlButtons.json");
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

            list.Add(GeneratePreset("Power", step: "POWER"));
            list.Add(GeneratePreset("Home", appId: "com.webos.app.home", key: Keys.H));
            list.Add(GeneratePreset("Settings", appId: "com.palm.app.settings", key: Keys.S));
            list.Add(GeneratePreset("TV Guide", appId: "com.webos.app.livemenu"));
            list.Add(GeneratePreset("List", step: "LIST"));
            list.Add(GeneratePreset("SAP", step: "SAP"));

            list.Add(GeneratePreset("Vol +", step: "VOLUMEUP", key: Keys.VolumeUp));
            list.Add(GeneratePreset("Vol -", step: "VOLUMEDOWN", key: Keys.VolumeDown));
            list.Add(GeneratePreset("Mute", step: "MUTE", key: Keys.VolumeMute));
            list.Add(GeneratePreset("Channel +", step: "CHANNELUP", key: Keys.MediaNextTrack));
            list.Add(GeneratePreset("Channel -", step: "CHANNELDOWN", key: Keys.MediaPreviousTrack));

            list.Add(GeneratePreset("Up", step: "UP", key: Keys.Up));
            list.Add(GeneratePreset("Down", step: "DOWN", key: Keys.Down));
            list.Add(GeneratePreset("Left", step: "LEFT", key: Keys.Left));
            list.Add(GeneratePreset("Right", step: "RIGHT", key: Keys.Right));

            list.Add(GeneratePreset("Enter", step: "ENTER", key: Keys.Enter));
            list.Add(GeneratePreset("Back", step: "BACK", key: Keys.Back));
            list.Add(GeneratePreset("Exit", step: "EXIT", key: Keys.Escape));

            list.Add(GeneratePreset("Netflix", appId: "netflix"));
            list.Add(GeneratePreset("Inputs", appId: "com.webos.app.homeconnect"));
            list.Add(GeneratePreset("Amazon Prime", appId: "amazon"));

            list.Add(GeneratePreset("Red", step: "RED"));
            list.Add(GeneratePreset("Green", step: "GREEN"));
            list.Add(GeneratePreset("Yellow", step: "YELLOW"));
            list.Add(GeneratePreset("Blue", step: "BLUE"));

            list.Add(GeneratePreset("Rakuten TV", appId: "ui30"));
            list.Add(GeneratePreset("Play", step: "PLAY"));
            list.Add(GeneratePreset("Pause", step: "PAUSE"));

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
                var json = _JsonSerializer.Serialize(_presets);
                File.WriteAllText(_presetsFilename, json);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        private void SaveConfig()
        {
            Config.PowerOnAfterStartup = Devices.Any(d => d.PowerOnAfterStartup);

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
            Devices = Config.Devices;
            var customIpAddresses = Devices.Where(d => d.IsCustom).Select(d => d.IpAddress);

            var pnpDevices = await Utils.GetPnpDevices(Config.DeviceSearchKey);

            var autoDevices = pnpDevices.Where(p => !customIpAddresses.Contains(p.IpAddress)).Select(d => new LgDevice(d.Name, d.IpAddress, d.MacAddress, false)).ToList();
            var autoIpAddresses = pnpDevices.Select(d => d.IpAddress);

            Devices.RemoveAll(d => !d.IsCustom && !autoIpAddresses.Contains(d.IpAddress));

            var newAutoDevices = autoDevices.Where(ad => !Devices.Any(d => d.IpAddress.Equals(ad.IpAddress)));
            Devices.AddRange(newAutoDevices);

            if (Devices.Any())
            {
                var preferredDevice = Devices.FirstOrDefault(x => x.MacAddress != null && x.MacAddress.Equals(Config.PreferredMacAddress)) ?? Devices[0];

                SelectedDevice = preferredDevice;
            }
            else
            {
                SelectedDevice = null;
            }

            if (afterStartUp && SelectedDevice == null && Config.PowerOnAfterStartup && !string.IsNullOrEmpty(Config.PreferredMacAddress))
            {
                Logger.Debug("No device has been found, trying to wake it first...");

                var tempDevice = new LgDevice("Test", string.Empty, Config.PreferredMacAddress);
                tempDevice.Wake();

                await Task.Delay(4000);
                await RefreshDevices();

                return;
            }

            if (connect && SelectedDevice != null)
            {
                if (_allowPowerOn)
                {
                    WakeAfterStartupOrResume();
                }
                else
                {
                    var _ = SelectedDevice.Connect();
                }
            }
        }

        public async Task<bool> ApplyPreset(string presetName)
        {
            var preset = _presets.FirstOrDefault(p => p.name.Equals(presetName, StringComparison.OrdinalIgnoreCase));
            if (preset != null)
            {
                return await ApplyPreset(preset);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ApplyPreset(LgPreset preset, bool reconnect = false)
        {
            var device = GetPresetDevice(preset);

            if (device == null)
            {
                Logger.Debug("Cannot apply preset: no device has been selected");
                return false;
            }

            return await device.ExecutePreset(preset, reconnect);
        }

        public LgDevice GetPresetDevice(LgPreset preset)
        {
            var device = string.IsNullOrEmpty(preset.DeviceMacAddress)
                ? SelectedDevice
                : Devices.FirstOrDefault(d => d.MacAddress?.Equals(preset.DeviceMacAddress, StringComparison.OrdinalIgnoreCase) ?? false);
            return device;
        }

        public async Task<LgWebOsMouseService> GetMouseAsync()
        {
            return await SelectedDevice?.GetMouseAsync();
        }

        public async Task RefreshApps(bool force = false)
        {
            if (SelectedDevice == null)
            {
                Logger.Debug("Cannot refresh apps: no device has been selected");
                return;
            }

            var apps = await SelectedDevice.GetApps();

            if (apps.Any())
            {
                _lgApps.Clear();
                _lgApps.AddRange(apps);
            }
        }

        public List<LgApp> GetApps()
        {
            return _lgApps;
        }

        internal async Task<bool> PowerOff()
        {
            return await SelectedDevice?.PowerOff();
        }

        internal Task PowerOffOnShutdownOrResume(PowerOnOffState state = PowerOnOffState.ShutDown)
        {
            var devices = Devices.Where(d => state == PowerOnOffState.ShutDown && d.PowerOffOnShutdown ||
                                             state == PowerOnOffState.StandBy && d.PowerOffOnStandby);

            var tasks = new List<Task>();
            foreach (var device in devices)
            {
                var task = device.PowerOff();

                tasks.Add(task);
            }

            return Task.WhenAll(tasks.ToArray());
        }

        internal bool WakeSelectedDevice()
        {
            return SelectedDevice?.Wake() ?? false;
        }

        internal void WakeAfterStartupOrResume(PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
        {
            if (checkUserSession && !UserSessionInfo.UserLocalSession)
            {
                Logger.Debug($"WakeAfterStartupOrResume: not waking because session info indicates no local session");
                return;
            }

            var wakeDevices = Devices.Where(d => state == PowerOnOffState.StartUp     && d.PowerOnAfterStartup ||
                                                 state == PowerOnOffState.Resume      && d.PowerOnAfterResume ||
                                                 state == PowerOnOffState.ScreenSaver && d.PowerSwitchOnScreenSaver);
            foreach (var device in wakeDevices)
            {
                if (device.PoweredOffBy == LgDevice.PowerOffSource.Manually)
                {
                    Logger.Debug($"[{device.Name}]: device was manually powered off by user, not powering on");
                    continue;
                }

                device.DisposeConnection();
                var _ = device.WakeAndConnectWithRetries(Config.PowerOnRetries);
            }
        }

        public void InstallEventHandlers()
        {
            SystemEvents.PowerModeChanged -= PowerModeChanged;
            SystemEvents.PowerModeChanged += PowerModeChanged;
            //SystemEvents.SessionEnded -= SessionEnded;
            //SystemEvents.SessionEnded += SessionEnded;

            UserSessionInfo.UserSessionSwitch -= SessionSwitched;
            UserSessionInfo.UserSessionSwitch += SessionSwitched;

            MonitorProcesses();
        }

        private void SessionSwitched(bool toLocal)
        {
            if (toLocal)
            {
                if (_poweredOffByScreenSaver)
                {
                    Logger.Debug("User switched to local and screen was powered off due to screensaver: waking up");
                    _poweredOffByScreenSaver = false;
                    _poweredOffByScreenSaverProcessId = 0;
                    WakeAfterStartupOrResume();
                }
                else
                {
                    Logger.Debug("User switched to local but screen was not powered off by screensaver: NOT waking up");
                }
            }
        }

        private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            var powerOn = e.Mode == PowerModes.Resume;

            Logger.Debug($"PowerModeChanged: {e.Mode}");

            if (powerOn)
            {
                WakeAfterStartupOrResume(PowerOnOffState.Resume);
                return;
            }

            if (Devices.Any(d => d.PowerOffOnStandby))
            {
                NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS | NativeConstants.ES_SYSTEM_REQUIRED | NativeConstants.ES_AWAYMODE_REQUIRED);
                try
                {
                    var standByScript = Path.Combine(Program.DataDir, "StandByScript.bat");
                    if (File.Exists(standByScript))
                    {
                        Utils.StartProcess(standByScript);
                    }

                    Logger.Debug("Powering off tv...");
                    var task = PowerOffOnShutdownOrResume(PowerOnOffState.StandBy);
                    Utils.WaitForTask(task);
                    Logger.Debug("Done powering off tv");
                }
                finally
                {
                    NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS);
                }
            }
        }

        private void SessionEnded(object sender, SessionEndedEventArgs e)
        {
            //if (e.Reason == SessionEndReasons.SystemShutdown)
            //{
            //    Logger.Debug($"SessionEnded: {e.Reason}");

            //    if (Config.PowerOffOnShutdown)
            //    {
            //        PowerOff();
            //    }
            //}
        }

        private void MonitorProcesses()
        {
            var enableMonitoring = Devices.Any(d => d.PowerSwitchOnScreenSaver);
            if (enableMonitoring && _monitorTask == null)
            {
                _monitorTask = CheckProcesses();
            }
            else if (!enableMonitoring && _monitorTask != null)
            {
                _monitorTask = null;
            }
        }

        public async Task CheckProcesses()
        {
            var wasConnected = Devices.Any(d => d.PowerSwitchOnScreenSaver && d.IsConnected());
            _monitorTaskCounter++;
            var validCounter = _monitorTaskCounter;
            var lastProcessId = 0;
            //const int minBacklight = 30;
            //const int maxBacklight = 50;
            //var lastBacklight = minBacklight;

            while (Devices.Any(d => d.PowerSwitchOnScreenSaver) && validCounter == _monitorTaskCounter)
            {
                await Task.Delay(500);

                try
                {
                    if (_poweredOffByScreenSaver && !UserSessionInfo.UserLocalSession)
                    {
                        continue;
                    }

                    var processes = Process.GetProcesses();

                    if (_poweredOffByScreenSaver)
                    {
                        if (processes.Any(p => p.Id == _poweredOffByScreenSaverProcessId))
                        {
                            continue;
                        }

                        Logger.Debug("Screensaver stopped, waking");
                        _poweredOffByScreenSaver = false;
                        _poweredOffByScreenSaverProcessId = 0;
                        lastProcessId = 0;
                        WakeAfterStartupOrResume(PowerOnOffState.ScreenSaver);

                        continue;
                    }

                    var devices = Devices.Where(d => d.PowerSwitchOnScreenSaver && d.IsConnected()).ToList();

                    if (!devices.Any())
                    {
                        if (wasConnected)
                        {
                            Logger.Debug("Screensaver check: TV(s) where connected, but not any longer");
                            wasConnected = false;
                        }
                        continue;
                    }

                    if (!wasConnected)
                    {
                        Logger.Debug("Screensaver check: TV(s) where not connected, but connection has now been established");
                        wasConnected = true;
                    }

                    //if (Utils.IsForegroundFullScreen())
                    //{
                    //    if (lastBacklight != maxBacklight)
                    //    {
                    //        SelectedDevice?.SetBacklight(maxBacklight);
                    //        lastBacklight = maxBacklight;
                    //    }
                    //}
                    //else if (lastBacklight != minBacklight)
                    //{
                    //    SelectedDevice?.SetBacklight(minBacklight);
                    //    lastBacklight = minBacklight;
                    //}

                    var process = processes.FirstOrDefault(p => p.ProcessName.ToLowerInvariant().EndsWith(".scr"));

                    if (process == null)
                    {
                        continue;
                    }

                    var parent = process.Parent();

                    if (parent?.ProcessName.Contains("winlogon") ?? false)
                    {
                        Logger.Debug($"Screensaver started: {process.ProcessName}, parent: {parent.ProcessName}");
                        try
                        {
                            foreach (var device in devices)
                            {
                                Logger.Debug($"Screensaver check: test connection with {device.Name}...");
                                var test = await device.TestConnection();
                                Logger.Debug("Screensaver check: test connection result: " + test);
                                if (!test)
                                {
                                    continue;
                                }

                                Logger.Debug($"Screensaver check: powering off tv {device.Name} because of screensaver");
                                _poweredOffByScreenSaver = await device.PowerOff();
                            }

                            _poweredOffByScreenSaverProcessId = process.Id;
                            lastProcessId = 0;
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Screensaver check: can't power off: " + e.ToLogString());
                        }
                    }
                    else if (process.Id != lastProcessId)
                    {
                        Logger.Debug($"Screensaver started: {process.ProcessName}, but invalid parent: {parent?.ProcessName ?? "no parent"}");
                        lastProcessId = process.Id;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("CheckProcesses: " + ex.ToLogString());
                }
            }
        }

        internal void AddCustomDevice(LgDevice device)
        {
            Devices.Add(device);
        }

        internal void RemoveCustomDevice(LgDevice device)
        {
            Devices.Remove(device);

            if (!string.IsNullOrEmpty(device.MacAddress))
            {
                var presets = _presets.Where(p => !string.IsNullOrEmpty(p.DeviceMacAddress) && p.DeviceMacAddress.Equals(device.MacAddress)).ToList();
                presets.ForEach(preset =>
                {
                    preset.DeviceMacAddress = null;
                });
            }

            if (SelectedDevice == device)
            {
                SelectedDevice = Devices.Any() ? Devices.First() : null;
            }
        }
    }
}
