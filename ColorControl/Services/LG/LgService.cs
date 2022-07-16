using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Services.Common;
using LgTv;
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

namespace ColorControl.Services.LG
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

        internal enum WindowsPowerSetting
        {
            Off = 0,
            On = 1,
            Dimmed = 2
        }

        internal class ProcessMonitorContext
        {
            public List<LgDevice> Devices { get; set; }
            public bool WasFullScreen { get; set; }
            public Process[] StartedProcesses { get; set; }
            public Process[] StoppedProcesses { get; set; }
            public Process[] RunningProcesses { get; set; }
            public bool IsNotificationDisabled { get; set; }
            public Process ForegroundProcess { get; set; }
            public bool ForegroundProcessIsFullScreen { get; set; }
            public string LastFullScreenProcessName = string.Empty;
        }

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string LgListAppsJson = "listApps.json";

        public override string ServiceName => "LG";

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
        public ProcessMonitorContext MonitorContext { get; private set; }

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
        private LgPreset _lastTriggeredPreset;
        private RestartDetector _restartDetector;

        public LgService(string dataPath, bool allowPowerOn) : base(dataPath, "LgPresets.json")
        {
            _allowPowerOn = allowPowerOn;

            LgPreset.LgApps = _lgApps;

            _restartDetector = new RestartDetector();

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

        protected override List<LgPreset> GetDefaultPresets()
        {
            List<LgPreset> presets = null;

            var defaultPresetsFileName = Path.Combine(Directory.GetCurrentDirectory(), "LgPresets.json");
            if (File.Exists(defaultPresetsFileName))
            {
                var json = File.ReadAllText(defaultPresetsFileName);

                presets = JsonConvert.DeserializeObject<List<LgPreset>>(json);
            }

            return presets;
        }

        private void LoadConfig()
        {
            _configFilename = Path.Combine(_dataPath, "LgConfig.json");
            try
            {
                if (File.Exists(_configFilename))
                {
                    Config = JsonConvert.DeserializeObject<LgServiceConfig>(File.ReadAllText(_configFilename));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"LoadConfig: {ex.Message}");
            }
            Config ??= new LgServiceConfig();

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
            list.Add(GeneratePreset("Wol", step: "WOL"));
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
                var json = JsonConvert.SerializeObject(_presets);
                File.WriteAllText(_presetsFilename, json);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        private void SaveConfig()
        {
            Config.PowerOnAfterStartup = Devices?.Any(d => d.PowerOnAfterStartup) ?? false;

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

            var newAutoDevices = autoDevices.Where(ad => ad.IpAddress != null && !Devices.Any(d => d.IpAddress != null && d.IpAddress.Equals(ad.IpAddress)));
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

            foreach (var device in Devices)
            {
                device.PowerStateChangedEvent += LgDevice_PowerStateChangedEvent;
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

        public override bool ApplyPreset(LgPreset preset)
        {
            var task = ApplyPreset(preset, false);

            Utils.WaitForTask(task);

            return task.Result;
        }

        public async Task<bool> ApplyPreset(LgPreset preset, bool reconnect = false)
        {
            var device = GetPresetDevice(preset);

            if (device == null)
            {
                Logger.Debug("Cannot apply preset: no device has been selected");
                return false;
            }

            var result = await device.ExecutePreset(preset, reconnect, Config);

            _lastAppliedPreset = preset;

            return result;
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

            var apps = await SelectedDevice.GetApps(force);

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

        internal bool WakeSelectedDevice()
        {
            return SelectedDevice?.Wake() ?? false;
        }

        internal void WakeAfterStartupOrResume(PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
        {
            var wakeDevices = Devices.Where(d => state == PowerOnOffState.StartUp && d.PowerOnAfterStartup ||
                                                 state == PowerOnOffState.Resume && d.PowerOnAfterResume ||
                                                 state == PowerOnOffState.ScreenSaver && d.PowerSwitchOnScreenSaver);

            PowerOnDevices(wakeDevices, state, checkUserSession);
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

            var devices = Devices.Where(d => d.PowerOffOnStandby);
            PowerOffDevices(devices, PowerOnOffState.StandBy);
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

        private void LgDevice_PowerStateChangedEvent(object sender, EventArgs e)
        {
            if (!(sender is LgDevice device))
            {
                return;
            }

            //if (device.CurrentState == LgDevice.PowerState.Active)
            //{
            //    _monitorTask = null;
            //    MonitorProcesses();
            //}
        }

        private void MonitorProcesses()
        {
            var enableMonitoring = ShouldMonitorProcesses();
            if (enableMonitoring && _monitorTask == null)
            {
                _monitorTask = CheckProcesses();
            }
            else if (!enableMonitoring && _monitorTask != null)
            {
                _monitorTask = null;
            }
        }

        public bool ShouldMonitorProcesses()
        {
            return Devices.Any(d => d.PowerSwitchOnScreenSaver) || _presets.Any(p => p.Triggers.Any(t => t.Trigger != PresetTriggerType.None));
        }

        public async Task CheckProcesses()
        {
            var wasConnected = Devices.Any(d => d.PowerSwitchOnScreenSaver && d.IsConnected());
            _monitorTaskCounter++;
            var validCounter = _monitorTaskCounter;
            var lastProcessId = 0;

            await Task.Delay(2000);

            MonitorContext ??= new ProcessMonitorContext();
            //Process[] lastProcesses = null;

            while (validCounter == _monitorTaskCounter)
            {
                await Task.Delay(_poweredOffByScreenSaver ? 500 : 1000);

                try
                {
                    var applicableDevices = Devices.Where(d => d.PowerSwitchOnScreenSaver || d.TriggersEnabled && _presets.Any(p => p.Triggers.Any(t => t.Trigger != PresetTriggerType.None) &&
                        ((string.IsNullOrEmpty(p.DeviceMacAddress) && d == SelectedDevice) || p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase))));

                    if (!applicableDevices.Any())
                    {
                        continue;
                    }

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

                    var connectedDevices = applicableDevices.Where(d => d.IsConnected()).ToList();

                    if (!connectedDevices.Any())
                    {
                        if (wasConnected)
                        {
                            Logger.Debug("Process monitor: TV(s) where connected, but not any longer");
                            wasConnected = false;
                            continue;
                        }
                        else
                        {
                            var devices = applicableDevices.ToList();
                            foreach (var device in devices)
                            {
                                Logger.Debug($"Process monitor: test connection with {device.Name}...");
                                var test = await device.TestConnection();
                                Logger.Debug("Process monitor: test connection result: " + test);
                            }

                            connectedDevices = applicableDevices.Where(d => d.IsConnected()).ToList();

                            if (!connectedDevices.Any())
                            {
                                continue;
                            }
                        }
                    }

                    if (!wasConnected)
                    {
                        Logger.Debug("Process monitor: TV(s) where not connected, but connection has now been established");
                        wasConnected = true;
                    }

                    if (connectedDevices.Any(d => d.PowerSwitchOnScreenSaver))
                    {
                        lastProcessId = await HandleScreenSaverProcessAsync(lastProcessId, processes, connectedDevices);
                    }

                    MonitorContext.Devices = connectedDevices.Where(d => d.TriggersEnabled).ToList();
                    MonitorContext.RunningProcesses = processes;

                    await ExecuteProcessPresets(MonitorContext);
                }
                catch (Exception ex)
                {
                    Logger.Error("CheckProcesses: " + ex.ToLogString());
                }
            }
        }

        private async Task<int> HandleScreenSaverProcessAsync(int lastProcessId, Process[] processes, List<LgDevice> connectedDevices)
        {
            var process = processes.FirstOrDefault(p => p.ProcessName.ToLowerInvariant().EndsWith(".scr"));

            if (process == null)
            {
                return 0;
            }

            var parent = process.Parent(processes);

            if (parent?.ProcessName.Contains("winlogon") ?? false)
            {
                Logger.Debug($"Screensaver started: {process.ProcessName}, parent: {parent.ProcessName}");
                try
                {
                    foreach (var device in connectedDevices)
                    {
                        Logger.Debug($"Screensaver check: test connection with {device.Name}...");
                        var test = await device.TestConnection();
                        Logger.Debug("Screensaver check: test connection result: " + test);
                        if (!test)
                        {
                            continue;
                        }

                        Logger.Debug($"Screensaver check: powering off tv {device.Name} because of screensaver");
                        try
                        {
                            _poweredOffByScreenSaver = await device.PowerOff().WaitAsync(TimeSpan.FromSeconds(5));
                        }
                        catch (TimeoutException)
                        {
                            // Assume powering off was successful
                            _poweredOffByScreenSaver = true;
                            Logger.Error($"Screensaver check: timeout exception while powering off (probably connection closed)");
                        }
                        catch (Exception pex)
                        {
                            // Assume powering off was successful
                            _poweredOffByScreenSaver = true;
                            Logger.Error($"Screensaver check: exception while powering off (probably connection closed): " + pex.Message);
                        }
                        Logger.Debug($"Screensaver check: powered off tv {device.Name}");
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

            return lastProcessId;
        }

        private async Task ExecuteProcessPresets(ProcessMonitorContext context)
        {
            var selectedDevice = SelectedDevice;

            var presets = _presets.Where(p => p.Triggers.Any(t => t.Trigger == PresetTriggerType.ProcessSwitch) &&
                                        ((string.IsNullOrEmpty(p.DeviceMacAddress) && selectedDevice?.TriggersEnabled == true)
                                            || Devices.Any(d => d.TriggersEnabled && p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase))));
            if (!presets.Any())
            {
                return;
            }

            context.IsNotificationDisabled = FormUtils.IsNotificationDisabled();

            var (processId, isFullScreen) = FormUtils.GetForegroundProcessIdAndIfFullScreen();

            if (processId == Environment.ProcessId)
            {
                return;
            }

            if (processId > 0)
            {
                var process = context.RunningProcesses.FirstOrDefault(p => p.Id == processId);

                if (process?.ProcessName?.Contains(".scr") ?? false)
                {
                    return;
                }

                context.ForegroundProcess = process;
                context.ForegroundProcessIsFullScreen = isFullScreen;
            }
            else
            {
                context.ForegroundProcess = null;
                context.ForegroundProcessIsFullScreen = false;
            }

            var changedProcesses = new List<Process>();
            if (context.ForegroundProcess != null)
            {
                changedProcesses.Add(context.ForegroundProcess);
            }

            if (context.ForegroundProcess != null && context.ForegroundProcessIsFullScreen)
            {
                if (!context.LastFullScreenProcessName.Equals(context.ForegroundProcess.ProcessName))
                {
                    context.LastFullScreenProcessName = context.ForegroundProcess.ProcessName;
                    //Logger.Debug($"Foreground fullscreen app detected: {context.ForegroundProcess.ProcessName}");
                }

                presets = presets.Where(p => p.Triggers.Any(t => t.Conditions.HasFlag(PresetConditionType.FullScreen)));
                context.WasFullScreen = true;
            }
            else if (context.WasFullScreen)
            {
                presets = presets.Where(p => p.Triggers.Any(t => !t.Conditions.HasFlag(PresetConditionType.FullScreen)));
                context.WasFullScreen = false;
                context.LastFullScreenProcessName = string.Empty;
            }

            var isHDRActive = context.Devices.Any(d => d.IsUsingHDRPictureMode());

            var isGsyncActive = LgDevice.ExternalServiceHandler("GsyncEnabled", new[] { "" });

            var triggerContext = new PresetTriggerContext
            {
                IsHDRActive = isHDRActive,
                IsGsyncActive = isGsyncActive,
                ForegroundProcess = context.ForegroundProcess,
                ForegroundProcessIsFullScreen = context.ForegroundProcessIsFullScreen,
                ChangedProcesses = changedProcesses,
                IsNotificationDisabled = context.IsNotificationDisabled
            };

            var toApplyPresets = new List<LgPreset>();
            foreach (var preset in presets)
            {
                if (preset.Triggers.Any(t => t.TriggerActive(triggerContext)))
                {
                    toApplyPresets.Add(preset);
                }
            }

            if (toApplyPresets.Any())
            {
                LgPreset toApplyPreset;
                if (toApplyPresets.Count > 1)
                {
                    toApplyPreset = toApplyPresets.FirstOrDefault(p => p.Triggers.Any(t => !t.IncludedProcesses.Contains("*")));
                    toApplyPreset = toApplyPreset != null ? toApplyPreset : toApplyPresets.First();
                }
                else
                {
                    toApplyPreset = toApplyPresets.First();
                }

                if (_lastTriggeredPreset != toApplyPreset)
                {
                    await ApplyPreset(toApplyPreset);

                    _lastTriggeredPreset = toApplyPreset;
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
                var presets = _presets.Where(p => !string.IsNullOrEmpty(p.DeviceMacAddress) && p.DeviceMacAddress.Equals(device.MacAddress, StringComparison.OrdinalIgnoreCase)).ToList();
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

        public void PowerSettingChanged(WindowsPowerSetting setting)
        {
            var devices = Devices.Where(d => d.PowerByWindows).ToList();

            if (setting == WindowsPowerSetting.Off)
            {
                PowerOffDevices(devices, PowerOnOffState.StandBy);
            }
            else if (setting == WindowsPowerSetting.On)
            {
                PowerOnDevices(devices);
            }
        }

        public void PowerOffDevices(IEnumerable<LgDevice> devices, PowerOnOffState state = PowerOnOffState.ShutDown)
        {
            if (!devices.Any())
            {
                return;
            }

            if (NativeMethods.GetAsyncKeyState(NativeConstants.VK_CONTROL) < 0 || NativeMethods.GetAsyncKeyState(NativeConstants.VK_RCONTROL) < 0)
            {
                Logger.Debug("Not powering off because CONTROL-key is down");
                return;
            }

            NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS | NativeConstants.ES_SYSTEM_REQUIRED | NativeConstants.ES_AWAYMODE_REQUIRED);
            try
            {
                if (state == PowerOnOffState.ShutDown)
                {
                    var sleep = Config.ShutdownDelay;

                    Logger.Debug($"PowerOffDevices: Waiting for {sleep} milliseconds...");

                    while (sleep > 0 && _restartDetector?.PowerOffDetected == false)
                    {
                        Thread.Sleep(100);

                        if (_restartDetector != null && (_restartDetector.RestartDetected || _restartDetector.IsRebootInProgress()))
                        {
                            Logger.Debug("Not powering off because of a restart");
                            return;
                        }

                        sleep -= 100;
                    }
                }

                Logger.Debug("Powering off tv(s)...");
                var tasks = new List<Task>();
                foreach (var device in devices)
                {
                    var task = device.PowerOff(true);

                    tasks.Add(task);
                }

                if (state == PowerOnOffState.StandBy)
                {
                    var standByScript = Path.Combine(Program.DataDir, "StandByScript.bat");
                    if (File.Exists(standByScript))
                    {
                        Utils.StartProcess(standByScript, hidden: true, wait: true);
                    }
                }

                var waitTask = Task.WhenAll(tasks.ToArray());
                Utils.WaitForTask(waitTask);
                Logger.Debug("Done powering off tv(s)");
            }
            finally
            {
                NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS);
            }
        }

        public void PowerOnDevices(IEnumerable<LgDevice> devices, PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
        {
            if (!devices.Any())
            {
                return;
            }

            if (checkUserSession && !UserSessionInfo.UserLocalSession)
            {
                Logger.Debug($"WakeAfterStartupOrResume: not waking because session info indicates no local session");
                return;
            }

            var resumeScript = Path.Combine(Program.DataDir, "ResumeScript.bat");

            var tasks = new List<Task>();

            foreach (var device in devices)
            {
                if (!device.PowerOnAfterManualPowerOff && device.PoweredOffBy == LgDevice.PowerOffSource.Manually)
                {
                    Logger.Debug($"[{device.Name}]: device was manually powered off by user, not powering on");
                    continue;
                }

                device.DisposeConnection();

                var task = device.WakeAndConnectWithRetries(Config.PowerOnRetries);
                tasks.Add(task);
            }

            if (File.Exists(resumeScript))
            {
                var waitTask = Task.WhenAll(tasks.ToArray());
                Utils.WaitForTask(waitTask);

                Utils.StartProcess(resumeScript, hidden: true);
            }
        }
    }
}