using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.Samsung;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NWin32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Services.Samsung
{
	class SamsungService : ServiceBase<SamsungPreset>, ISamsungService
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public override string ServiceName => "Samsung";

		protected override string PresetsBaseFilename => "SamsungPresets.json";

		public SamsungServiceConfig Config { get; private set; }
		public SamsungDevice SelectedDevice { get; set; }

		private bool _allowPowerOn;
		private ServiceManager _serviceManager;
		private readonly PowerEventDispatcher _powerEventDispatcher;
		private readonly SessionSwitchDispatcher _sessionSwitchDispatcher;
		private readonly RestartDetector _restartDetector;
		private readonly ProcessEventDispatcher _processEventDispatcher;
		private readonly WinApiAdminService _winApiAdminService;
		private string _configFilename;
		private bool _poweredOffByScreenSaver;
		private int _poweredOffByScreenSaverProcessId;
		private List<SamsungApp> _samsungApps = new List<SamsungApp>();
		private Shared.Common.GlobalContext _appContext;

		public List<SamsungDevice> Devices { get; private set; }

		public static readonly int SHORTCUTID_SAMSUNGQA = -204;

		public SamsungService(GlobalContext globalContext, ServiceManager serviceManager, PowerEventDispatcher powerEventDispatcher, SessionSwitchDispatcher sessionSwitchDispatcher, RestartDetector restartDetector, ProcessEventDispatcher processEventDispatcher, WinApiAdminService winApiAdminService) : base(globalContext)
		{
			_appContext = globalContext;
			_allowPowerOn = _appContext.StartUpParams.RunningFromScheduledTask;
			_serviceManager = serviceManager;
			_powerEventDispatcher = powerEventDispatcher;
			_sessionSwitchDispatcher = sessionSwitchDispatcher;
			_restartDetector = restartDetector;
			_processEventDispatcher = processEventDispatcher;
			_winApiAdminService = winApiAdminService;
			SamTvConnection.SyncContext = _appContext.SynchronizationContext;
			SamsungPreset.GetDeviceName += SamsungPreset_GetDeviceName;

			LoadConfig();
			LoadPresets();
		}

		private string SamsungPreset_GetDeviceName(string macAddress)
		{
			var device = Devices?.FirstOrDefault(d => !string.IsNullOrEmpty(d.MacAddress) && d.MacAddress.Equals(macAddress));
			if (device != null)
			{
				return device.Name;
			}

			return "Unknown: device not found";
		}

		public static async Task<bool> ExecutePresetAsync(string presetName)
		{
			try
			{
				var samsungService = Program.ServiceProvider.GetRequiredService<SamsungService>();

				await samsungService.RefreshDevices(afterStartUp: true);

				var result = await samsungService.ApplyPreset(presetName);

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

		public override void InstallEventHandlers()
		{
			base.InstallEventHandlers();

			SetShortcuts(SHORTCUTID_SAMSUNGQA, Config.QuickAccessShortcut);

			_powerEventDispatcher.RegisterAsyncEventHandler(PowerEventDispatcher.Event_Startup, PowerStateChanged);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_MonitorOn, PowerModeChanged);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_MonitorOff, PowerModeChanged);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_Suspend, PowerModeChanged);
			_powerEventDispatcher.RegisterAsyncEventHandler(PowerEventDispatcher.Event_Resume, PowerModeResume);
			_powerEventDispatcher.RegisterEventHandler(PowerEventDispatcher.Event_Shutdown, PowerModeChanged);
			_processEventDispatcher.RegisterAsyncEventHandler(ProcessEventDispatcher.Event_ProcessChanged, ProcessChanged);
		}

		public override List<string> GetInfo()
		{
			return [$"{_presets.Count} presets", $"{Devices?.Count ?? 0} devices"];
		}

		private async Task PowerStateChanged(object sender, PowerStateChangedEventArgs e, CancellationToken token)
		{
			await RefreshDevices(afterStartUp: true);

			if (_appContext.StartUpParams.RunningFromScheduledTask)
			{
				await ExecutePresetsForEvent(PresetTriggerType.Startup);
			}

			if (_appContext.StartUpParams.ExecuteSamsungPreset)
			{
				await ExecutePresetAsync(_appContext.StartUpParams.SamsungPresetName);
			}
		}

		private void LoadConfig()
		{
			_configFilename = Path.Combine(_dataPath, "SamsungConfig.json");
			try
			{
				if (File.Exists(_configFilename))
				{
					Config = JsonConvert.DeserializeObject<SamsungServiceConfig>(File.ReadAllText(_configFilename));
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"LoadConfig: {ex.Message}");
			}
			Config ??= new SamsungServiceConfig();
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
						//_ = ExecutePresetsForEvent(PresetTriggerType.Standby).ConfigureAwait(true);

						var devices = Devices.Where(d => d.Options.PowerOffOnStandby);
						PowerOffDevices(devices, PowerOnOffState.StandBy);
						break;
					}
				case PowerOnOffState.ShutDown:
					{
						_ = ExecutePresetsForEvent(PresetTriggerType.Shutdown).ConfigureAwait(true);

						var devices = Devices.Where(d => d.Options.PowerOffOnShutdown);

						PowerOffDevices(devices);
						break;
					}

				case PowerOnOffState.MonitorOff:
					{
						var devices = Devices.Where(d => d.Options.PowerOffByWindows).ToList();
						PowerOffDevices(devices, PowerOnOffState.MonitorOff);
						break;
					}
				case PowerOnOffState.MonitorOn:
					{
						var devices = Devices.Where(d => d.Options.PowerOnByWindows).ToList();
						PowerOnDevicesTask(devices);
						break;
					}
			}
		}

		private async Task ExecutePresetsForEvent(PresetTriggerType triggerType)
		{
			Logger.Debug($"Executing presets for event {triggerType}");

			var presets = _presets.Where(p => p.Triggers.Any(t => t.Trigger == triggerType)).ToList();

			if (!presets.Any())
			{
				return;
			}

			var applicableDevices = Devices.Where(d => d.Options.TriggersEnabled &&
				presets.Any(p => (string.IsNullOrEmpty(p.DeviceMacAddress) && d == SelectedDevice) || p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase))).ToList();

			if (!applicableDevices.Any())
			{
				return;
			}

			await ExecuteEventPresets(_serviceManager, new[] { triggerType }).ConfigureAwait(true);
		}

		public async Task<bool> RefreshDevices(bool connect = true, bool afterStartUp = false)
		{
			Devices = Config.Devices;

			if (!_globalContext.StartUpParams.NoDeviceRefresh)
			{
				var customIpAddresses = Devices.Where(d => d.IsCustom).Select(d => d.IpAddress);

				var pnpDevices = await Utils.GetPnpDevices(Config.DeviceSearchKey, Utils.DEV_CLASS_DISPLAY_TV_LCD);

				var autoDevices = pnpDevices.Where(p => !customIpAddresses.Contains(p.IpAddress)).Select(d => new SamsungDevice(d.Name, d.IpAddress, d.MacAddress, false)).ToList();
				var autoIpAddresses = pnpDevices.Select(d => d.IpAddress);

				Devices.RemoveAll(d => !d.IsCustom && !autoIpAddresses.Contains(d.IpAddress));

				var newAutoDevices = autoDevices.Where(ad => ad.IpAddress != null && !Devices.Any(d => d.IpAddress != null && d.IpAddress.Equals(ad.IpAddress)));
				Devices.AddRange(newAutoDevices);
			}

			if (Devices.Any())
			{
				var preferredDevice = Devices.FirstOrDefault(x => x.MacAddress != null && x.MacAddress.Equals(Config.PreferredMacAddress)) ?? Devices[0];

				SelectedDevice = preferredDevice;

				SelectedDevice.Connected += SelectedDevice_Connected;
			}
			else
			{
				SelectedDevice = null;
			}

			if (afterStartUp && SelectedDevice == null && Config.PowerOnAfterStartup && !string.IsNullOrEmpty(Config.PreferredMacAddress))
			{
				Logger.Debug("No device has been found, trying to wake it first...");

				var tempDevice = new SamsungDevice("Test", string.Empty, Config.PreferredMacAddress);
				tempDevice.Wake();

				await Task.Delay(4000);
				return await RefreshDevices();
			}

			if (connect && SelectedDevice != null)
			{
				if (_allowPowerOn)
				{
					await WakeAfterStartupOrResume();
				}
				else
				{
					var _ = SelectedDevice.ConnectAsync();
				}
			}

			return true;
		}

		private async void SelectedDevice_Connected(object sender)
		{
			if (_samsungApps.Any())
			{
				return;
			}

			//await RefreshAppsAsync();
		}

		protected override List<SamsungPreset> GetDefaultPresets()
		{
			return new List<SamsungPreset>();
		}

		private void SaveConfig()
		{
			Config.PowerOnAfterStartup = Devices?.Any(d => d.Options.PowerOnAfterStartup) ?? false;

			Utils.WriteObject(_configFilename, Config);
		}

		public void GlobalSave()
		{
			SavePresets();
			SaveConfig();
		}

		public SamsungDevice GetPresetDevice(SamsungPreset preset)
		{
			var device = string.IsNullOrEmpty(preset.DeviceMacAddress)
				? SelectedDevice
				: Devices.FirstOrDefault(d => d.MacAddress?.Equals(preset.DeviceMacAddress, StringComparison.OrdinalIgnoreCase) ?? false);
			return device;
		}

		public override async Task<bool> ApplyPreset(SamsungPreset preset)
		{
			return await ApplyPreset(preset, _appContext);
		}

		private async Task<bool> ApplyPreset(SamsungPreset preset, Shared.Common.GlobalContext appContext)
		{
			var result = true;

			var device = GetPresetDevice(preset);

			if (device == null)
			{
				return false;
			}

			await device.ExecutePresetAsync(preset, appContext, Config);

			_lastAppliedPreset = preset;

			PresetApplied();

			return result;
		}

		internal void AddCustomDevice(SamsungDevice device)
		{
			Devices.Add(device);
		}

		internal void RemoveCustomDevice(SamsungDevice device)
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

		private Task WakeAfterStartupOrResume(PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
		{
			Devices.ForEach(d => d.ClearPowerOffTask());

			var wakeDevices = Devices.Where(d => state == PowerOnOffState.StartUp && d.Options.PowerOnAfterStartup ||
												 state == PowerOnOffState.Resume && d.Options.PowerOnAfterResume ||
												 state == PowerOnOffState.ScreenSaver && d.Options.PowerOnAfterScreenSaver);

			return PowerOnDevicesTask(wakeDevices, state, checkUserSession);
		}

		private void PowerOffDevices(IEnumerable<SamsungDevice> devices, PowerOnOffState state = PowerOnOffState.ShutDown)
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
				var task = device.PowerOffAsync();

				tasks.Add(task);
			}

			if (state == PowerOnOffState.StandBy)
			{
				var task = Task.Run(() =>
				{
					var standByScript = Path.Combine(Program.DataDir, "StandByScript.bat");
					if (File.Exists(standByScript))
					{
						_winApiAdminService.StartProcess(standByScript, hidden: true, wait: true, useShellExecute: false);
					}
				});

				tasks.Add(task);
			}

			// We can't use async here because we need to stay on the main thread...
			var i = 0;
			while (tasks.Any(t => t.Status < TaskStatus.WaitingForChildrenToComplete) && i < 200)
			{
				Thread.Sleep(10);
				System.Windows.Forms.Application.DoEvents();
				i++;
			}

			//var _ = Task.WhenAll(tasks.ToArray());

			Logger.Debug("Done powering off tv(s)");
		}

		private Task PowerOnDevicesTask(IEnumerable<SamsungDevice> devices, PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
		{
			return Task.Run(async () => await PowerOnDevices(devices, state, checkUserSession));
		}

		private async Task PowerOnDevices(IEnumerable<SamsungDevice> devices, PowerOnOffState state = PowerOnOffState.StartUp, bool checkUserSession = true)
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
				if (!device.Options.PowerOnAfterManualPowerOff && device.PoweredOffBy == PowerOffSource.External)
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

		private async Task RefreshAppsAsync(bool force = false)
		{
			if (SelectedDevice == null)
			{
				Logger.Debug("Cannot refresh apps: no device has been selected");
				return;
			}

			var apps = await SelectedDevice.GetAppsAsync(force);

			if (apps.Any())
			{
				_samsungApps.Clear();
				_samsungApps.AddRange(apps);
			}
		}

		internal List<SamsungApp> GetApps()
		{
			return _samsungApps;
		}

		private async Task ProcessChanged(object sender, ProcessChangedEventArgs args, CancellationToken token)
		{
			try
			{
				var wasConnected = Devices?.Any(d => d.IsConnected());

				if (!wasConnected.HasValue)
				{
					return;
				}

				var applicableDevices = Devices.Where(d => d.Options.PowerOffOnScreenSaver || d.Options.PowerOnAfterScreenSaver || d.Options.TriggersEnabled && _presets.Any(p => p.Triggers.Any(t => t.Trigger != PresetTriggerType.None) &&
					((string.IsNullOrEmpty(p.DeviceMacAddress) && d == SelectedDevice) || p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase)))).ToList();

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
							//Logger.Debug($"Process monitor: test connection with {device.Name}...");

							var test = await device.TestConnectionAsync();
							//Logger.Debug("Process monitor: test connection result: " + test);
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

				if (connectedDevices.Any(d => d.Options.PowerOffOnScreenSaver || d.Options.PowerOnAfterScreenSaver))
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

		private async Task ExecuteScreenSaverPresetsCustom(ProcessChangedEventArgs context, IList<SamsungDevice> connectedDevices)
		{
			var triggerDevices = connectedDevices.Where(d => d.Options.TriggersEnabled).ToList();

			if (!triggerDevices.Any())
			{
				return;
			}

			await ExecuteScreenSaverPresets(_serviceManager, context);
		}

		private async Task ExecuteProcessPresets(ProcessChangedEventArgs context, IList<SamsungDevice> connectedDevices)
		{
			var triggerDevices = connectedDevices.Where(d => d.Options.TriggersEnabled).ToList();

			if (context.IsScreenSaverActive)
			{
				return;
			}

			var selectedDevice = SelectedDevice;

			var presets = _presets.Where(p => p.Triggers.Any(t => t.Trigger == PresetTriggerType.ProcessSwitch) &&
										((string.IsNullOrEmpty(p.DeviceMacAddress) && selectedDevice?.Options.TriggersEnabled == true)
											|| Devices.Any(d => d.Options.TriggersEnabled && p.DeviceMacAddress.Equals(d.MacAddress, StringComparison.OrdinalIgnoreCase))));
			if (!presets.Any())
			{
				return;
			}

			var changedProcesses = new List<Process>();
			if (context.ForegroundProcess != null)
			{
				changedProcesses.Add(context.ForegroundProcess);
			}

			if (context.ForegroundProcess != null && context.ForegroundProcessIsFullScreen)
			{
				presets = presets.Where(p => p.Triggers.Any(t => t.Conditions.HasFlag(PresetConditionType.FullScreen)));
			}
			else if (context.StoppedFullScreen)
			{
				presets = presets.Where(p => p.Triggers.Any(t => !t.Conditions.HasFlag(PresetConditionType.FullScreen)));
			}

			var triggerContext = await CreateTriggerContext(_serviceManager, context);

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

		private async Task HandleScreenSaverProcessAsync(IList<Process> processes, List<SamsungDevice> connectedDevices)
		{
			var process = processes.FirstOrDefault(p => p.ProcessName.ToLowerInvariant().EndsWith(".scr"));

			var powerOffDevices = connectedDevices.Where(d => d.Options.PowerOffOnScreenSaver).ToList();

			if (process == null)
			{
				powerOffDevices.ForEach(d => d.ClearPowerOffTask());

				return;
			}

			var parent = process.Parent(processes);

			// Ignore manually started screen savers if no devices prefer this
			if (parent?.ProcessName.Contains("winlogon") != true && !connectedDevices.Any(d => d.Options.HandleManualScreenSaver))
			{
				return;
			}

			Logger.Debug($"Screensaver started: {process.ProcessName}, parent: {parent.ProcessName}");
			try
			{
				foreach (var device in powerOffDevices)
				{
					Logger.Debug($"Screensaver check: test connection with {device.Name}...");

					var test = await device.TestConnectionAsync();
					Logger.Debug("Screensaver check: test connection result: " + test);
					if (!test)
					{
						continue;
					}

					if (device.Options.ScreenSaverMinimalDuration > 0)
					{
						Logger.Debug($"Screensaver check: powering off tv {device.Name} in {device.Options.ScreenSaverMinimalDuration} seconds because of screensaver");

						_poweredOffByScreenSaver = true;

						device.PowerOffIn(device.Options.ScreenSaverMinimalDuration);

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

		public async Task ExecuteEventPresets(PresetTriggerType triggerType)
		{
			await ExecuteEventPresets(_serviceManager, new[] { triggerType });
		}

		public List<SamsungDeviceDto> GetDevices()
		{
			return Devices?.Select(d => new SamsungDeviceDto
			{
				Name = d.Name,
				MacAddress = d.MacAddress,
				IpAddress = d.IpAddress,
				IsCustom = d.IsCustom,
				IsDummy = d.IsDummy,
				Token = d.Token,
				Options = d.Options,
				IsConnected = d.IsConnected(),
				IsSelected = d == SelectedDevice
			}).ToList();
		}

		public async Task<bool> UpdateDevice(SamsungDeviceDto deviceSpec)
		{
			if (Devices == null)
			{
				return false;
			}

			var device = Devices.FirstOrDefault(d => d.MacAddress == deviceSpec.MacAddress);

			if (device == null)
			{
				device = new SamsungDevice(deviceSpec);

				Devices.Add(device);
			}
			else
			{
				var reconnect = device.Update(deviceSpec);

				if (reconnect)
				{
					await device.ConnectAsync(true);
				}
			}

			SavePresets();

			return true;
		}

		public async Task<bool> TestDevice(SamsungDeviceDto deviceSpec)
		{
			if (Devices == null)
			{
				return false;
			}

			try
			{
				var device = new SamsungDevice(deviceSpec);

				return await device.TestConnectionAsync();
			}
			catch
			{
				return false;
			}
		}

		public SamsungServiceConfigDto GetConfig()
		{
			return new SamsungServiceConfigDto
			{
				DefaultButtonDelay = Config.DefaultButtonDelay,
				DeviceSearchKey = Config.DeviceSearchKey,
				Devices = GetDevices(),
				PowerOnAfterStartup = Config.PowerOnAfterStartup,
				PowerOnRetries = Config.PowerOnRetries,
				PreferredMacAddress = Config.PreferredMacAddress,
				QuickAccessShortcut = Config.QuickAccessShortcut,
				SetSelectedDeviceByPowerOn = Config.SetSelectedDeviceByPowerOn,
				ShowAdvancedActions = Config.ShowAdvancedActions,
				ShutdownDelay = Config.ShutdownDelay,
			};
		}

		public bool UpdateConfig(SamsungServiceConfigDto config)
		{
			Config.Update(config);

			SetShortcuts(SHORTCUTID_SAMSUNGQA, Config.QuickAccessShortcut);

			SaveConfig();

			return true;
		}

		public bool UpdatePreset(SamsungPreset specPreset)
		{
			var currentPreset = _presets.FirstOrDefault(p => p.id == specPreset.id);

			if (currentPreset != null)
			{
				currentPreset.Update(specPreset);

				SavePresets();

				return true;
			}

			var newPreset = new SamsungPreset(specPreset);
			newPreset.name = specPreset.name;

			_presets.Add(newPreset);

			SavePresets();

			return true;
		}

		public List<InvokableActionDto<SamsungPreset>> GetInvokableActions()
		{
			return SelectedDevice?.GetInvokableActions(Config.ShowAdvancedActions).Select(a => new InvokableActionDto<SamsungPreset>
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

		public async Task<bool> ExecuteInvokableAction(InvokableActionDto<SamsungPreset> invokableAction)
		{
			return await SelectedDevice?.ExecuteInvokableAction(invokableAction, _appContext, Config);
		}

		public bool SetSelectedDevice(SamsungDeviceDto deviceDto)
		{
			if (deviceDto == null)
			{
				return false;
			}

			var device = Devices?.FirstOrDefault(d => string.IsNullOrEmpty(deviceDto.MacAddress) ? d.IpAddress == deviceDto.IpAddress : d.MacAddress == deviceDto.MacAddress);
			if (device == null)
			{
				return false;
			}

			SelectedDevice = device;

			return true;
		}
	}
}