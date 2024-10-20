using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.LG;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using LgTv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json;
using NStandard;
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

				SelectedDeviceChangedEvent?.Invoke(_selectedDevice, EventArgs.Empty);
			}
		}
		public event EventHandler SelectedDeviceChangedEvent;

		public LgServiceConfig Config { get; private set; }

		protected override string PresetsBaseFilename => "LgPresets.json";

		private string _configFilename;
		private bool _allowPowerOn;
		private readonly PowerEventDispatcher _powerEventDispatcher;
		private readonly SessionSwitchDispatcher _sessionSwitchDispatcher;
		private readonly ProcessEventDispatcher _processEventDispatcher;
		private readonly DisplayChangeEventDispatcher _displayChangeEventDispatcher;
		private readonly ServiceManager _serviceManager;
		private LgDevice _selectedDevice;
		private string _rcButtonsFilename;
		private List<LgPreset> _remoteControlButtons;
		private List<LgApp> _lgApps = new List<LgApp>();

		private bool _poweredOffByScreenSaver;
		private int _poweredOffByScreenSaverProcessId;
		private RestartDetector _restartDetector;
		private readonly WinApiService _winApiService;
		private readonly WinApiAdminService _winApiAdminService;
		private SynchronizationContext _syncContext;
		private LgGameBar _gameBarForm;

		public ProcessChangedEventArgs CurrentProcessEvent => _processEventDispatcher?.MonitorContext;

		public static readonly int SHORTCUTID_LGQA = -202;
		public static readonly int SHORTCUTID_GAMEBAR = -101;

		public LgService(
			GlobalContext globalContext,
			PowerEventDispatcher powerEventDispatcher,
			SessionSwitchDispatcher sessionSwitchDispatcher,
			ProcessEventDispatcher processEventDispatcher,
			DisplayChangeEventDispatcher displayChangeEventDispatcher,
			ServiceManager serviceManager,
			RestartDetector restartDetector,
			WinApiService winApiService,
			WinApiAdminService winApiAdminService) : base(globalContext)
		{
			_allowPowerOn = globalContext.StartUpParams.RunningFromScheduledTask;
			_powerEventDispatcher = powerEventDispatcher;
			_sessionSwitchDispatcher = sessionSwitchDispatcher;
			_processEventDispatcher = processEventDispatcher;
			_displayChangeEventDispatcher = displayChangeEventDispatcher;
			_serviceManager = serviceManager;
			LgPreset.GetDeviceName += LgPreset_GetDeviceName;
			LgPreset.GetAppName += LgPreset_GetAppName;

			_restartDetector = restartDetector;
			_winApiService = winApiService;
			_winApiAdminService = winApiAdminService;
			_syncContext = globalContext.SynchronizationContext;
			LgTvApiCore.SyncContext = _syncContext;

			LoadConfig();
			LoadPresets();
			LoadRemoteControlButtons();
		}

		private string LgPreset_GetAppName(string appId)
		{
			var name = appId;

			var item = _lgApps.FirstOrDefault(x => x.appId.Equals(appId));
			if (item != null)
			{
				name = item.title + " (" + appId + ")";
			}

			return name;
		}

		private string LgPreset_GetDeviceName(string macAddress)
		{
			var device = Devices?.FirstOrDefault(d => !string.IsNullOrEmpty(d.MacAddress) && d.MacAddress.Equals(macAddress));
			if (device != null)
			{
				return device.Name;
			}

			return "Unknown: device not found";
		}

		~LgService()
		{
			GlobalSave();
		}

		protected override async Task HotKeyPressed(object sender, KeyboardShortcutEventArgs e, CancellationToken _)
		{
			await base.HotKeyPressed(sender, e, _);

			if (e.HotKeyId == SHORTCUTID_GAMEBAR)
			{
				ToggleGameBar();
				return;
			}
		}

		public override List<string> GetInfo()
		{
			return [$"{_presets.Count} presets", $"{Devices?.Count ?? 0} devices"];
		}

		public static async Task<bool> ExecutePresetAsync(string presetName)
		{
			try
			{
				var lgService = Program.ServiceProvider.GetRequiredService<LgService>();

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

			SendConfigToService();
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

		private void SaveConfig()
		{
			Config.PowerOnAfterStartup = Devices?.Any(d => d.PowerOnAfterStartup) ?? false;

			Utils.WriteObject(_configFilename, Config);
		}

		private void SaveRemoteControlButtons()
		{
			Utils.WriteObject(_rcButtonsFilename, _remoteControlButtons);
		}

		public async Task<bool> RefreshDevices(bool connect = true, bool afterStartUp = false)
		{
			Devices = Config.Devices;

			if (!_globalContext.StartUpParams.NoDeviceRefresh)
			{
				var customIpAddresses = Devices.Where(d => d.IsCustom).Select(d => d.IpAddress);

				var pnpDevices = await Utils.GetPnpDevices(Config.DeviceSearchKey);

				var autoDevices = pnpDevices.Where(p => !customIpAddresses.Contains(p.IpAddress)).Select(d => new LgDevice(d.Name, d.IpAddress, d.MacAddress, false)).ToList();
				var autoIpAddresses = pnpDevices.Select(d => d.IpAddress);

				Devices.RemoveAll(d => !d.IsCustom && !autoIpAddresses.Contains(d.IpAddress));

				var newAutoDevices = autoDevices.Where(ad => ad.IpAddress != null && !Devices.Any(d => d.IpAddress != null && d.IpAddress.Equals(ad.IpAddress)));
				Devices.AddRange(newAutoDevices);
			}

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
				return await RefreshDevices();
			}

			foreach (var device in Devices)
			{
				device.PowerStateChangedEvent += LgDevice_PowerStateChangedEvent;
				device.PictureSettingsChangedEvent += Device_PictureSettingsChangedEvent;
			}

			if (connect && SelectedDevice != null)
			{
				if (_allowPowerOn)
				{
					await WakeAfterStartupOrResume();
				}
				else
				{
					if (afterStartUp)
					{
						var _ = SelectedDevice.Connect();
					}
					else
					{
						return await SelectedDevice.Connect(1);
					}
				}
			}

			return true;
		}

		private void Device_PictureSettingsChangedEvent(object sender, EventArgs e)
		{
			//
		}

		public override async Task<bool> ApplyPreset(LgPreset preset)
		{
			return await ApplyPreset(preset, false);
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

		internal Task WakeAfterStartupOrResume(PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
		{
			Devices.ForEach(d => d.ClearPowerOffTask());

			var wakeDevices = Devices.Where(d => state == PowerOnOffState.StartUp && d.PowerOnAfterStartup ||
												 state == PowerOnOffState.Resume && d.PowerOnAfterResume ||
												 state == PowerOnOffState.ScreenSaver && d.PowerOnAfterScreenSaver);

			return PowerOnDevicesTask(wakeDevices, state, checkUserSession);
		}

		public override void InstallEventHandlers()
		{
			base.InstallEventHandlers();

			SetShortcuts(SHORTCUTID_LGQA, Config.QuickAccessShortcut);

			var keyboardShortcutDispatcher = _globalContext.ServiceProvider.GetService<KeyboardShortcutDispatcher>();
			keyboardShortcutDispatcher?.RegisterShortcut(SHORTCUTID_GAMEBAR, Config.GameBarShortcut);

			_powerEventDispatcher.RegisterAsyncEventHandler(PowerEventDispatcher.Event_Startup, PowerStateChanged);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_Suspend, PowerModeChanged);
			_powerEventDispatcher.RegisterAsyncEventHandler(PowerEventDispatcher.Event_Resume, PowerModeResume);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_Shutdown, PowerModeChanged);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_MonitorOff, PowerModeChanged);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_MonitorOn, PowerModeChanged);

			_sessionSwitchDispatcher.RegisterEventHandler(SessionSwitchDispatcher.Event_SessionSwitch, SessionSwitched);
			_sessionSwitchDispatcher.RegisterAsyncEventHandler(SessionSwitchDispatcher.Event_SessionSwitch, SessionSwitchedAsync);

			_processEventDispatcher.RegisterAsyncEventHandler(ProcessEventDispatcher.Event_ProcessChanged, ProcessChanged);

			_displayChangeEventDispatcher.RegisterAsyncEventHandler(DisplayChangeEventDispatcher.Event_DisplayChanged, DisplayChanged);
		}

		private async Task DisplayChanged(object sender, DisplayChangedEventArgs e, CancellationToken ct)
		{
			await ExecutePresetsForEvent(PresetTriggerType.DisplayChange);
		}

		private async Task PowerStateChanged(object sender, PowerStateChangedEventArgs e, CancellationToken token)
		{
			await RefreshDevices(afterStartUp: true);

			if (_globalContext.StartUpParams.RunningFromScheduledTask)
			{
				await ExecutePresetsForEvent(PresetTriggerType.Startup);
			}


			if (_globalContext.StartUpParams.ExecuteLgPreset)
			{
				await ExecutePresetAsync(_globalContext.StartUpParams.LgPresetName);
			}
		}

		private void SessionSwitched(object sender, SessionSwitchEventArgs e)
		{
			if (!_sessionSwitchDispatcher.UserLocalSession)
			{
				return;
			}

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

		private async Task SessionSwitchedAsync(object sender, SessionSwitchEventArgs e, CancellationToken token)
		{
			if (e.Reason == SessionSwitchReason.SessionLock)
			{
				var devices = Devices.Where(d => d.PowerOffAfterSessionLock);
				PowerOffDevices(devices, PowerOnOffState.StandBy);
			}
			else if (e.Reason == SessionSwitchReason.SessionUnlock)
			{
				var devices = Devices.Where(d => d.PowerOnAfterSessionUnlock).ToList();
				await PowerOnDevicesTask(devices);
			}
		}

		private async Task PowerModeResume(object sender, PowerStateChangedEventArgs e, CancellationToken _)
		{
			Logger.Debug($"PowerModeChanged: {e.State}");

			await WakeAfterStartupOrResume(PowerOnOffState.Resume);

			await ExecutePresetsForEvent(PresetTriggerType.Resume);
		}

		private void PowerModeChanged(object sender, PowerStateChangedEventArgs e)
		{
			Logger.Debug($"PowerModeChanged: {e.State}");

			if (Devices?.Any() != true)
			{
				Logger.Debug("Devices have not been loaded, ignoring event");
				return;
			}

			switch (e.State)
			{
				case PowerOnOffState.StandBy:
					{
						_ = ExecutePresetsForEvent(PresetTriggerType.Standby);

						var devices = Devices.Where(d => d.PowerOffOnStandby);
						PowerOffDevices(devices, PowerOnOffState.StandBy);
						break;
					}
				case PowerOnOffState.ShutDown:
					{
						_ = ExecutePresetsForEvent(PresetTriggerType.Shutdown);

						var devices = Devices.Where(d => d.PowerOffOnShutdown);

						PowerOffDevices(devices);
						break;
					}

				case PowerOnOffState.MonitorOff:
					{
						var devices = Devices.Where(d => d.PowerOffByWindows).ToList();
						PowerOffDevices(devices, PowerOnOffState.MonitorOff);
						break;
					}
				case PowerOnOffState.MonitorOn:
					{
						var devices = Devices.Where(d => d.PowerOnByWindows).ToList();
						PowerOnDevicesTask(devices);
						break;
					}
			}
		}

		private async Task ExecutePresetsForEvent(PresetTriggerType triggerType)
		{
			var presets = _presets.Where(p => p.Triggers.Any(t => t.Trigger == triggerType)).ToList();

			if (!presets.Any())
			{
				return;
			}

			var applicableDevices = Devices.Where(d => d.TriggersEnabled &&
				presets.Any(p => (string.IsNullOrEmpty(p.DeviceMacAddress) && d == SelectedDevice) || p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase))).ToList();

			if (!applicableDevices.Any())
			{
				return;
			}

			await ExecuteEventPresets(_serviceManager, new[] { triggerType }, isHDRActive: applicableDevices.Any(d => d.IsUsingHDRPictureMode())).ConfigureAwait(true);
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

			if (device.CurrentState == LgDevice.PowerState.Active)
			{
				if (Config.SetSelectedDeviceByPowerOn)
				{
					SelectedDevice = device;
				}
			}
		}

		public async Task ProcessChanged(object sender, ProcessChangedEventArgs args, CancellationToken token)
		{
			try
			{
				var wasConnected = Devices?.Any(d => (d.PowerOffOnScreenSaver || d.PowerOnAfterScreenSaver) && d.IsConnected());

				if (!wasConnected.HasValue)
				{
					return;
				}

				var applicableDevices = Devices.Where(d =>
							   d.PowerOffOnScreenSaver
							|| d.PowerOnAfterScreenSaver
							|| (d.TriggersEnabled
									// only these trigger types are ever triggered by a process changed event
									&& _presets.Any(p => p.Triggers.Any(t => t.Trigger == PresetTriggerType.ProcessSwitch || t.Trigger == PresetTriggerType.ScreensaverStart || t.Trigger == PresetTriggerType.ScreensaverStop)
									&& ((string.IsNullOrEmpty(p.DeviceMacAddress) && d == SelectedDevice) || p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase)))))
					.ToList();

				if (!applicableDevices.Any())
				{
					return;
				}

				if (_poweredOffByScreenSaver && !_sessionSwitchDispatcher.UserLocalSession)
				{
					return;
				}

				var processes = args.RunningProcesses;

				if (_poweredOffByScreenSaver)
				{
					if (processes.Any(p => p.Id == _poweredOffByScreenSaverProcessId))
					{
						return;
					}

					var process = processes.FirstOrDefault(p => p.ProcessName.ToLowerInvariant().EndsWith(".scr"));

					if (process != null)
					{
						Logger.Debug($"Detected screen saver process switch from {_poweredOffByScreenSaverProcessId} to {process.Id}");

						_poweredOffByScreenSaverProcessId = process.Id;
						return;
					}

					Logger.Debug("Screensaver stopped, waking");
					_poweredOffByScreenSaver = false;
					_poweredOffByScreenSaverProcessId = 0;

					await WakeAfterStartupOrResume(PowerOnOffState.ScreenSaver);

					await ExecuteScreenSaverPresetsCustom(args, applicableDevices);

					return;
				}

				var connectedDevices = applicableDevices.Where(d => d.IsConnected()).ToList();

				if (!connectedDevices.Any())
				{
					if (wasConnected == true)
					{
						Logger.Debug("Process monitor: TV(s) where connected, but not any longer");
						wasConnected = false;

						return;
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
							return;
						}
					}
				}

				if (wasConnected == false)
				{
					Logger.Debug("Process monitor: TV(s) where not connected, but connection has now been established");
					wasConnected = true;
				}

				if (connectedDevices.Any(d => d.PowerOffOnScreenSaver || d.PowerOnAfterScreenSaver))
				{
					await HandleScreenSaverProcessAsync(processes, connectedDevices);
				}

				await ExecuteProcessPresets(args, connectedDevices);

				await ExecuteScreenSaverPresetsCustom(args, connectedDevices);
			}
			catch (Exception ex)
			{
				Logger.Error("ProcessChanged: " + ex.ToLogString());
			}
		}

		private async Task ExecuteScreenSaverPresetsCustom(ProcessChangedEventArgs context, IList<LgDevice> connectedDevices)
		{
			var triggerDevices = connectedDevices.Where(d => d.TriggersEnabled).ToList();

			if (!triggerDevices.Any())
			{
				return;
			}

			await ExecuteScreenSaverPresets(_serviceManager, context, triggerDevices.Any(d => d.IsUsingHDRPictureMode()));
		}

		private async Task ExecuteProcessPresets(ProcessChangedEventArgs context, IList<LgDevice> connectedDevices)
		{
			var triggerDevices = connectedDevices.Where(d => d.TriggersEnabled).ToList();

			if (context.IsScreenSaverActive)
			{
				return;
			}

			var selectedDevice = SelectedDevice;

			var presets = _presets.Where(p => p.Triggers.Any(t => t.Trigger == PresetTriggerType.ProcessSwitch) &&
										((string.IsNullOrEmpty(p.DeviceMacAddress) && selectedDevice?.TriggersEnabled == true)
											|| Devices.Any(d => d.TriggersEnabled && p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase))));
			if (!presets.Any())
			{
				return;
			}

			if (context.ForegroundProcess != null && context.ForegroundProcessIsFullScreen)
			{
				presets = presets.Where(p => p.Triggers.Any(t => t.Conditions.HasFlag(PresetConditionType.FullScreen)));
			}
			else if (context.StoppedFullScreen)
			{
				presets = presets.Where(p => p.Triggers.Any(t => !t.Conditions.HasFlag(PresetConditionType.FullScreen)));
			}

			var triggerContext = await CreateTriggerContext(_serviceManager, context, triggerDevices.Any(d => d.IsUsingHDRPictureMode()));

			var toApplyPresets = presets.Where(p => p.Triggers.Any(t => t.TriggerActive(triggerContext))).ToList();

			if (toApplyPresets.Any())
			{
				var toApplyPreset = toApplyPresets.FirstOrDefault(p => toApplyPresets.Count == 1 || p.Triggers.Any(t => !t.IncludedProcesses.Contains("*"))) ?? toApplyPresets.First();

				if (_lastTriggeredPreset != toApplyPreset)
				{
					Logger.Debug($"Applying triggered preset: {toApplyPreset.name} because trigger conditions are met");

					await ApplyPreset(toApplyPreset);

					_lastTriggeredPreset = toApplyPreset;
				}
			}
		}

		private async Task HandleScreenSaverProcessAsync(IList<Process> processes, List<LgDevice> connectedDevices)
		{
			var process = processes.FirstOrDefault(p => p.ProcessName.ToLowerInvariant().EndsWith(".scr"));

			var powerOffDevices = connectedDevices.Where(d => d.PowerOffOnScreenSaver).ToList();

			if (process == null)
			{
				powerOffDevices.ForEach(d => d.ClearPowerOffTask());

				return;
			}

			var parent = process.Parent(processes);

			// Ignore manually started screen savers if no devices prefer this
			if (parent?.ProcessName.Contains("winlogon") != true && !connectedDevices.Any(d => d.HandleManualScreenSaver))
			{
				return;
			}

			Logger.Debug($"Screensaver started: {process.ProcessName}, parent: {parent.ProcessName}");
			try
			{
				foreach (var device in powerOffDevices)
				{
					Logger.Debug($"Screensaver check: test connection with {device.Name}...");

					var test = await device.TestConnection();
					Logger.Debug("Screensaver check: test connection result: " + test);
					if (!test)
					{
						continue;
					}

					if (device.ScreenSaverMinimalDuration > 0)
					{
						Logger.Debug($"Screensaver check: powering off tv {device.Name} in {device.ScreenSaverMinimalDuration} seconds because of screensaver");

						_poweredOffByScreenSaver = true;

						device.PowerOffIn(device.ScreenSaverMinimalDuration);

						continue;
					}

					Logger.Debug($"Screensaver check: powering off tv {device.Name} because of screensaver");
					try
					{
						_poweredOffByScreenSaver = await device.PerformActionOnScreenSaver().WaitAsync(TimeSpan.FromSeconds(5));
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
			}
			catch (Exception e)
			{
				Logger.Error($"Screensaver check: can't power off: " + e.ToLogString());
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

			if (state == PowerOnOffState.ShutDown)
			{
				var sleep = Config.ShutdownDelay;

				Logger.Debug($"PowerOffDevices: Waiting for a maximum of {sleep} milliseconds...");

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
				var task = device.PowerOff(true, false);

				tasks.Add(task);
			}

			if (state == PowerOnOffState.StandBy)
			{
				var standByScript = Path.Combine(Program.DataDir, "StandByScript.bat");
				if (File.Exists(standByScript))
				{
					_winApiAdminService.StartProcess(standByScript, hidden: true, wait: true);
				}
			}

			// We can't use async here because we need to stay on the main thread...
			var _ = Task.WhenAll(tasks.ToArray()).ConfigureAwait(true);

			Logger.Debug("Done powering off tv(s)");
		}

		public Task PowerOnDevicesTask(IEnumerable<LgDevice> devices, PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
		{
			return Task.Run(async () => await PowerOnDevices(devices, state, checkUserSession));
		}

		public async Task PowerOnDevices(IEnumerable<LgDevice> devices, PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
		{
			if (!devices.Any())
			{
				return;
			}

			if (checkUserSession && !_sessionSwitchDispatcher.UserLocalSession)
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

				if (state == PowerOnOffState.ScreenSaver)
				{
					tasks.Add(device.PerformActionAfterScreenSaver(Config.PowerOnRetries));
					continue;
				}

				var task = device.WakeAndConnectWithRetries(Config.PowerOnRetries);
				tasks.Add(task);
			}

			await Task.WhenAll(tasks.ToArray());

			if (File.Exists(resumeScript))
			{
				_winApiAdminService.StartProcess(resumeScript, hidden: true);
			}
		}

		private void SendConfigToService()
		{
			if (!_winApiService.IsServiceRunning())
			{
				return;
			}

			var json = JsonConvert.SerializeObject(Config);

			var message = new SvcMessage
			{
				MessageType = SvcMessageType.SetLgConfig,
				Data = json
			};

			PipeUtils.SendMessage(message);
		}

		public async Task ExecuteEventPresets(PresetTriggerType triggerType)
		{
			await ExecuteEventPresets(_serviceManager, new[] { triggerType });
		}

		public void ToggleGameBar()
		{
			if (_gameBarForm == null || !_gameBarForm.Visible)
			{
				if (_gameBarForm == null || _gameBarForm.IsDisposed)
				{
					if (SelectedDevice == null)
					{
						return;
					}

					_gameBarForm = new LgGameBar(this);
				}

				_gameBarForm.Show();
				_gameBarForm.Activate();
			}
			else
			{
				_gameBarForm?.Hide();
			}
		}

		public List<LgDeviceDto> GetDevices()
		{
			if (Devices == null)
			{
				return new List<LgDeviceDto>();
			}

			return Devices.Select(d => new LgDeviceDto
			{
				Name = d.Name,
				MacAddress = d.MacAddress,
				IpAddress = d.IpAddress,
				IsCustom = d.IsCustom,
				IsDummy = d.IsDummy,
				Options = new LgDeviceOptions
				{
					HandleManualScreenSaver = d.HandleManualScreenSaver,
					HDMIPortNumber = d.HDMIPortNumber,
					PowerOffByWindows = d.PowerOffByWindows,
					PowerOffOnScreenSaver = d.PowerOffOnScreenSaver,
					PowerOffOnShutdown = d.PowerOffOnShutdown,
					PowerOffOnStandby = d.PowerOffOnStandby,
					PowerOnAfterManualPowerOff = d.PowerOnAfterManualPowerOff,
					PowerOnAfterResume = d.PowerOnAfterResume,
					PowerOnAfterScreenSaver = d.PowerOnAfterScreenSaver,
					PowerOnAfterStartup = d.PowerOnAfterStartup,
					PowerOnByWindows = d.PowerOnByWindows,
					PowerOffAfterSessionLock = d.PowerOffAfterSessionLock,
					PowerOnAfterSessionUnlock = d.PowerOnAfterSessionUnlock,
					ScreenSaverMinimalDuration = d.ScreenSaverMinimalDuration,
					TriggersEnabled = d.TriggersEnabled,
					TurnScreenOffOnScreenSaver = d.TurnScreenOffOnScreenSaver,
					TurnScreenOnAfterScreenSaver = d.TurnScreenOnAfterScreenSaver,
					UseSecureConnection = d.UseSecureConnection
				},
				IsConnected = d.IsConnected(),
				IsSelected = d == SelectedDevice
			}).ToList();
		}

		public async Task<bool> UpdateDevice(LgDeviceDto deviceSpec)
		{
			if (Devices == null)
			{
				return false;
			}

			var device = Devices.FirstOrDefault(d => d.MacAddress == deviceSpec.MacAddress);

			if (device == null)
			{
				device = new LgDevice(deviceSpec);

				Devices.Add(device);
			}
			else
			{
				var reconnect = device.Update(deviceSpec);

				if (reconnect)
				{
					await device.Connect();
				}
			}

			SaveConfig();

			return true;
		}

		public async Task<bool> TestDevice(LgDeviceDto deviceSpec)
		{
			if (Devices == null)
			{
				return false;
			}

			try
			{
				var device = new LgDevice(deviceSpec);

				return await device.TestConnection();
			}
			catch
			{
				return false;
			}
		}

		public LgServiceConfigDto GetConfig()
		{
			return new LgServiceConfigDto
			{
				DefaultButtonDelay = Config.DefaultButtonDelay,
				DeviceSearchKey = Config.DeviceSearchKey,
				//Devices = GetDevices(),
				PowerOnAfterStartup = Config.PowerOnAfterStartup,
				PowerOnRetries = Config.PowerOnRetries,
				PreferredMacAddress = Config.PreferredMacAddress,
				QuickAccessShortcut = Config.QuickAccessShortcut,
				SetSelectedDeviceByPowerOn = Config.SetSelectedDeviceByPowerOn,
				ShowAdvancedActions = Config.ShowAdvancedActions,
				ShutdownDelay = Config.ShutdownDelay,
				GameBarShortcut = Config.GameBarShortcut,
				GameBarShowingTime = Config.GameBarShowingTime
			};
		}

		public bool UpdateConfig(LgServiceConfigDto config)
		{
			Config.Update(config);

			SetShortcuts(SHORTCUTID_LGQA, Config.QuickAccessShortcut);

			SaveConfig();

			return true;
		}

		public bool UpdatePreset(LgPreset specPreset)
		{
			var currentPreset = _presets.FirstOrDefault(p => p.id == specPreset.id);

			if (currentPreset != null)
			{
				currentPreset.Update(specPreset);

				SavePresets();
				return true;
			}

			var newPreset = new LgPreset(specPreset);
			newPreset.name = specPreset.name;

			_presets.Add(newPreset);

			SavePresets();

			return true;
		}

		public List<InvokableActionDto<LgPreset>> GetInvokableActions()
		{
			return SelectedDevice?.GetInvokableActions(Config.ShowAdvancedActions).Select(a => new InvokableActionDto<LgPreset>
			{
				Advanced = a.Advanced,
				Category = Utils.FirstCharUpperCase(a.Category ?? "Misc"),
				CurrentValue = a.CurrentValue,
				EnumType = a.EnumType,
				MaxValue = a.MaxValue,
				MinValue = a.MinValue,
				Name = a.Name,
				NumberOfValues = a.NumberOfValues,
				Preset = a.Preset,
				Title = a.Title ?? Utils.FirstCharUpperCase(a.Name),
				ValueLabels = a.ValueLabels
			}).ToList();
		}

		public async Task<bool> ExecuteInvokableAction(InvokableActionDto<LgPreset> invokableAction, List<string> parameters = null)
		{
			var device = SelectedDevice;

			if (invokableAction.SelectedDevice != null)
			{
				device = MatchDevice(invokableAction.SelectedDevice) ?? SelectedDevice;
			}

			if (invokableAction.Preset != null)
			{
				await SelectedDevice?.ExecutePreset(invokableAction.Preset, false, Config);

				return true;
			}

			return await device?.ExecuteInvokableAction(invokableAction, parameters);
		}

		public bool SetSelectedDevice(LgDeviceDto deviceDto)
		{
			if (deviceDto == null)
			{
				return false;
			}

			var device = MatchDevice(deviceDto);
			if (device == null)
			{
				return false;
			}

			SelectedDevice = device;

			return true;
		}

		private LgDevice MatchDevice(LgDeviceDto deviceDto)
		{
			return Devices?.FirstOrDefault(d => string.IsNullOrEmpty(deviceDto.MacAddress) ? d.IpAddress == deviceDto.IpAddress : d.MacAddress == deviceDto.MacAddress);
		}
	}
}