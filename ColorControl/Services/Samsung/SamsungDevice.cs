using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.Samsung;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Services.Samsung
{
    public enum PowerOffSource
    {
        Unknown,
        App,
        External
    }

    public delegate void GenericDelegate(object sender);

    class SamsungDevice
    {
        public class InvokableAction
        {
            public Func<Dictionary<string, object>, Task<bool>> AsyncFunction { get; set; }
            public string Name { get; set; }
            public Type EnumType { get; set; }
            public decimal MinValue { get; set; }
            public decimal MaxValue { get; set; }
            public string Category { get; set; }
            public string Title { get; set; }
            public int CurrentValue { get; set; }
            public int NumberOfValues { get; set; }
            public bool Advanced { get; set; }
            public SamsungPreset Preset { get; set; }
            public List<string> ValueLabels { get; set; }
        }


        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; private set; }
        public string IpAddress { get; private set; }
        public string MacAddress { get; private set; }
        public bool IsCustom { get; private set; }
        public string Token { get; set; }
        public SamsungDeviceOptions Options { get; set; }

        [JsonIgnore]
        public bool IsDummy { get; private set; }

        [JsonIgnore]
        public bool PoweredOn { get; private set; }
        [JsonIgnore]
        public DateTimeOffset PoweredOffAt { get; private set; }
        [JsonIgnore]
        public PowerOffSource PoweredOffBy { get; internal set; }

        public event GenericDelegate Connected;

        [JsonIgnore]
        private SamTvConnection _samTvConnection;

        [JsonIgnore]
        private ServiceManager _serviceManager;
        [JsonIgnore]
        private Timer _powerOffTimer;
        [JsonIgnore]
        private SemaphoreSlim _connectSemaphore = new SemaphoreSlim(1);

        [JsonIgnore]
        private List<InvokableAction> _invokableActions = new List<InvokableAction>();

        [JsonConstructor]
        public SamsungDevice(string name, string ipAddress, string macAddress, bool isCustom = true, bool isDummy = false)
        {
            Options = new SamsungDeviceOptions();

            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
            IsCustom = isCustom;
            IsDummy = isDummy;

            AddInvokableAction("WOL", WakeAction);
            AddInvokableAction("ServiceMenu", ServiceMenuAction, true);
            AddInternalPresetAction(new SamsungPreset("PictureOff", null, new[] { "KEY_AD:1400", "KEY_DOWN:200", "KEY_DOWN:200", "KEY_ENTER" }));
            AddInternalPresetAction(new SamsungPreset("PictureOn", null, new[] { "KEY_RETURN:1000", "KEY_RETURN", "KEY_RETURN" }));

            _serviceManager = Program.ServiceProvider.GetRequiredService<ServiceManager>();
        }

        public SamsungDevice(SamsungDeviceDto spec) : this(spec.Name, spec.IpAddress, spec.MacAddress, spec.IsCustom)
        {
            // TODO: call update method
            if (spec.Options != null)
            {
                Options = spec.Options;
            }
        }

        ~SamsungDevice()
        {
            ClearPowerOffTask();
        }

        public bool Update(SamsungDeviceDto spec)
        {
            var reconnectNeeded = false;

            Name = spec.Name;

            if (IpAddress != spec.IpAddress)
            {
                reconnectNeeded = IsConnected();
                IpAddress = spec.IpAddress;
            }

            MacAddress = spec.MacAddress;
            IsCustom = spec.IsCustom;

            // TODO: call update method
            if (spec.Options != null)
            {
                Options = spec.Options;
            }

            return reconnectNeeded;
        }

        public async Task<bool> ConnectAsync(bool force = false, bool retryToken = false)
        {
            if (!retryToken)
            {
                await _connectSemaphore.WaitAsync();
            }
            try
            {
                if (force)
                {
                    if (_samTvConnection != null)
                    {
                        await _samTvConnection.DisposeAsync();
                        _samTvConnection = null;
                    }
                }
                else if (IsConnected())
                {
                    return true;
                }

                _samTvConnection = new SamTvConnection();
                _samTvConnection.Connected += Connection_Connected;
                var b64 = Utils.Base64Encode("ColorControl");

                var firstTime = Token == null;
                var readTimeout = firstTime ? 60000 : 5000;

                try
                {
                    var protocol = Options.UseSecureConnection ? "wss" : "ws";
                    var port = Options.UseSecureConnection ? 8002 : 8001;

                    var uriString = $"{protocol}://{IpAddress}:{port}/api/v2/channels/samsung.remote.control?name={b64}";

                    if (Token != null)
                    {
                        uriString += $"&token={Token}";
                    }

                    var result = await _samTvConnection.Connect(new Uri(uriString), readTimeout);

                    if (!result)
                    {
                        return false;
                    }

                    if (firstTime && Token != null && !retryToken)
                    {
                        await _samTvConnection.DisposeAsync();
                        _samTvConnection = null;

                        return await ConnectAsync(force, true);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex);

                    return false;
                }

            }
            finally
            {
                if (!retryToken)
                {
                    _connectSemaphore.Release();
                }

                if (true)
                {
                    Connected?.Invoke(this);
                }
            }
        }

        private void Connection_Connected(string token, bool disconnect)
        {
            if (disconnect)
            {
                Logger.Debug($"Disconnect event, powered off by: {PoweredOffBy}");

                if (_samTvConnection?.ClosedByDispose != true && PoweredOffBy == PowerOffSource.Unknown)
                {
                    PoweredOffBy = PowerOffSource.External;
                }

                return;
            }

            Token = token;

            //_taskCompletionSource?.SetResult(token);
            PoweredOffBy = PowerOffSource.Unknown;
        }

        internal async Task PowerOffAsync()
        {
            try
            {
                await SendKeyAsync("KEY_POWER").WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(true);
            }
            catch (TimeoutException ex)
            {
                Logger.Debug($"Timeout when turning off tv: {ex.Message}");
            }

            PoweredOn = false;
            PoweredOffAt = DateTimeOffset.Now;
            PoweredOffBy = PowerOffSource.App;
        }

        internal async Task<bool> WakeAndConnectWithRetries(int retries = 5)
        {
            var maxRetries = retries <= 1 ? 5 : retries;

            var result = false;
            for (var retry = 0; retry < maxRetries && !result; retry++)
            {
                Logger.Debug($"WakeAndConnectWithRetries: attempt {retry + 1} of {maxRetries}...");

                var ms = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                result = await WakeAndConnect();
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

        internal async Task<bool> WakeAndConnect(int connectDelay = 1000)
        {
            try
            {
                var result = await Utils.PingHost(IpAddress);

                Logger.Debug($"Ping result: {result}");

                var powerState = default(string);

                if (result)
                {
                    powerState = await GetPowerStateAsync();
                }

                if (result && powerState == "on" && IsConnected())
                {
                    Logger.Debug("Already powered on and connected");
                    return true;
                }

                Logger.Debug($"Power state: {powerState}");

                async Task<bool> SendWol()
                {
                    if (!Wake())
                    {
                        Logger.Debug("WOL failed");
                        return false;
                    }
                    Logger.Debug("WOL succeeded");
                    await Task.Delay(connectDelay);

                    return true;
                }

                if (!result || string.IsNullOrEmpty(powerState))
                {
                    await SendWol();
                }

                result = await ConnectAsync(true);

                if (powerState == "standby" || string.IsNullOrWhiteSpace(powerState))
                {
                    if (result)
                    {
                        Logger.Debug("Connected while standby");
                        await SendKeyAsync("KEY_POWER");

                        await SendWol();
                        //var powerState = await GetPowerStateAsync();

                        ////if (!PoweredOn && (PoweredOffAt == DateTimeOffset.MinValue || PoweredOffAt >= DateTimeOffset.Now.AddMinutes(-2)))
                        //if (powerState == "standby")
                        //{
                        //    await SendKeyAsync("KEY_POWER");
                        //}
                    }
                    else
                    {
                        Logger.Debug("Initial connect while standby failed, send WOL");
                        result = await SendWol();
                        if (result)
                        {
                            result = await ConnectAsync(true);
                            Logger.Debug($"Connect after WOL result: {result}");
                        }
                    }
                }

                PoweredOn = true;

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
                var wolService = Program.ServiceProvider.GetRequiredService<WolService>();

                result = wolService.SendWol(MacAddress, IpAddress);
            }
            else
            {
                Logger.Debug("Cannot wake device: the device has no MAC-address");
            }

            return result;
        }

        public async Task<string> GetPowerStateAsync()
        {
            var url = $"http://{IpAddress}:8001/api/v2/";

            var json = await Utils.GetRestJsonAsync(url);

            if (json == null)
            {
                Logger.Error("No power state received");

                return string.Empty;
            }

            var powerState = (string)json.device?.PowerState;

            Logger.Debug($"Current power state: {powerState}");

            return powerState;
        }

        internal async Task<bool> TestConnectionAsync()
        {
            var result = await Utils.PingHost(IpAddress);

            if (result)
            {
                return await ConnectAsync();
            }

            return false;
        }

        public bool IsConnected()
        {
            return _samTvConnection?.ConnectionClosed == false;
        }

        internal void ConvertToCustom()
        {
            IsCustom = true;
        }

        public override string ToString()
        {
            return $"{(IsDummy ? string.Empty : (IsCustom ? "Custom: " : "Auto detect: "))}{Name}{(!string.IsNullOrEmpty(IpAddress) ? ", " + IpAddress : string.Empty)}";
        }

        internal async Task<bool> ExecutePresetAsync(SamsungPreset preset, Shared.Common.GlobalContext appContext = null, SamsungServiceConfig config = null)
        {
            if (config == null)
            {
                var service = Program.ServiceProvider.GetRequiredService<SamsungService>();
                config = service.Config;
            }

            var hasApp = !string.IsNullOrEmpty(preset.AppId);

            var hasWOL = preset.Steps.Any(s => s.Equals("WOL", StringComparison.OrdinalIgnoreCase));

            if (hasWOL)
            {
                var connected = await WakeAndConnect();
                if (!connected)
                {
                    return false;
                }
            }

            for (var tries = 0; tries <= 1; tries++)
            {
                if (!await ConnectAsync())
                {
                    return false;
                }

                if (hasApp)
                {
                    try
                    {
                        //await _lgTvApi.LaunchApp(preset.appId, @params);
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

                    await Task.Delay(1000);
                }

                if (preset.Steps.Any())
                {
                    if (hasApp)
                    {
                        await Task.Delay(1500);
                    }
                    try
                    {
                        await ExecuteStepsAsync(preset, appContext, config);
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

        private async Task ExecuteStepsAsync(SamsungPreset preset, Shared.Common.GlobalContext appContext, SamsungServiceConfig config)
        {
            foreach (var step in preset.Steps)
            {
                var keySpec = step.Split(':');

                var delay = config.DefaultButtonDelay;
                var cmd = "Click";
                var key = step;
                if (keySpec.Length >= 2)
                {
                    delay = Utils.ParseInt(keySpec[1]);
                    if (delay > 0)
                    {
                        key = keySpec[0];
                    }
                }
                if (keySpec.Length >= 3)
                {
                    cmd = keySpec[2];
                }

                var index = key.IndexOf("(");
                string[] parameters = null;
                if (index > -1)
                {
                    var keyValue = key.Split('(');
                    key = keyValue[0];
                    parameters = keyValue[1].Substring(0, keyValue[1].Length - 1).Split(';');
                }

                var executeKey = true;
                var action = _invokableActions.FirstOrDefault(a => a.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (action != null)
                {
                    await ExecuteActionAsync(action, parameters, appContext, config);

                    executeKey = false;
                }

                if (parameters != null && await _serviceManager.HandleExternalServiceAsync(key, parameters))
                {
                    executeKey = false;
                }

                if (executeKey)
                {
                    await SendKeyAsync(key, cmd);
                    delay = delay == 0 ? config.DefaultButtonDelay : delay;
                }

                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
        }

        public async Task ExecuteActionAsync(InvokableAction action, string[] parameters, Shared.Common.GlobalContext appContext, SamsungServiceConfig config)
        {
            if (action.Preset != null)
            {
                await ExecutePresetAsync(action.Preset, appContext, config);

                return;
            }
            if (action.AsyncFunction != null)
            {
                await action.AsyncFunction(null);
            }
        }

        private async Task SendKeyAsync(string key, string cmd = "Click")
        {
            var dynamic = new
            {
                Cmd = cmd,
                DataOfCmd = key,
                Option = "false",
                TypeOfRemote = "SendRemoteKey"
            };

            var message = new RequestMessage("ms.remote.control", dynamic);

            await _samTvConnection.SendCommandAsync(message);
        }

        internal void ClearPowerOffTask()
        {
            _powerOffTimer?.Dispose();
        }

        internal void PowerOffIn(int seconds)
        {
            ClearPowerOffTask();

            _powerOffTimer = new Timer(PowerOffByTimer);
            _powerOffTimer.Change(TimeSpan.FromSeconds(seconds), TimeSpan.FromSeconds(seconds));
        }

        private void PowerOffByTimer(object _)
        {
            ClearPowerOffTask();

            var x = PerformActionOnScreenSaver();
        }

        private bool IsPowerOffAllowed() => true;

        public async Task<bool> PerformActionOnScreenSaver()
        {
            if (!IsPowerOffAllowed())
            {
                return true;
            }

            if (Options.TurnScreenOffOnScreenSaver)
            {
                Logger.Debug("Turning screen off on screen saver");
                await TurnPictureOff();

                return true;
            }

            Logger.Debug($"Device {Name} is now powering off due to delayed screensaver task");

            await PowerOffAsync();

            return true;
        }

        private async Task TurnPictureOff()
        {
            var action = _invokableActions.FirstOrDefault(a => a.Name == "PictureOff");

            if (action?.Preset != null)
            {
                await ExecutePresetAsync(action.Preset);
            }
        }

        private async Task TurnPictureOn()
        {
            var action = _invokableActions.FirstOrDefault(a => a.Name == "PictureOn");

            if (action?.Preset != null)
            {
                await ExecutePresetAsync(action.Preset);
            }
        }

        public async Task<bool> PerformActionAfterScreenSaver(int powerOnRetries)
        {
            if (Options.TurnScreenOffOnScreenSaver && IsConnected())
            {
                if (Options.TurnScreenOnAfterScreenSaver)
                {
                    try
                    {
                        Logger.Debug("Turning screen on after screen saver");
                        await TurnPictureOn();
                    }
                    catch (Exception) { }
                }

                return true;
            }

            return await WakeAndConnectWithRetries(powerOnRetries);
        }

        internal async Task<IEnumerable<SamsungApp>> GetAppsAsync(bool force)
        {
            var messages = new List<RequestMessage>
            {
                new RequestMessage("ms.channel.emit", new
                {
                    data = "",
                    @event = "ed.edenApp.get",
                    to = "host",
                }),
                //new RequestMessage("ms.channel.emit", new
                //{
                //    data = "",
                //    @event = "ed.installedApp.get",
                //    to = "host",
                //})
            };

            foreach (var message in messages)
            {
                var result = await _samTvConnection.SendCommandAsync(message);
            }


            return new List<SamsungApp>();
        }

        private void AddInternalPresetAction(SamsungPreset preset)
        {
            var action = new InvokableAction
            {
                Name = preset.name,
                Preset = preset
            };

            _invokableActions.Add(action);
        }

        private void AddInvokableAction(string name, Func<Dictionary<string, object>, Task<bool>> function, bool advanced = false)
        {
            var action = new InvokableAction
            {
                Name = name,
                AsyncFunction = function,
                Category = "misc",
                Title = name,
                Advanced = advanced
            };

            _invokableActions.Add(action);
        }

        internal IEnumerable<InvokableAction> GetInvokableActions(bool showAdvanced)
        {
            return _invokableActions.Where(a => showAdvanced || !a.Advanced);
        }

        private async Task<bool> WakeAction(Dictionary<string, object> _)
        {
            return await WakeAndConnect();
        }

        private async Task<bool> ServiceMenuAction(Dictionary<string, object> _)
        {
            var serviceMenuTypes = new[] { ServiceMenuType.FactoryMenu, ServiceMenuType.HospitalityMenu }.Select(t => Utils.GetDescription(t));

            var serviceMenuTypeField = new FieldDefinition
            {
                Label = "Service Menu Access Type",
                SubLabel = "Both types will not reset your settings. 'FactoryMenu' is recommended for 2024 series but will also work for older ones. Click 'Next >' to continue or 'X' to stop",
                FieldType = FieldType.DropDown,
                Values = serviceMenuTypes,
                Value = ServiceMenuType.FactoryMenu
            };

            if (!MessageForms.ShowDialog($"Access Service Menu", [serviceMenuTypeField], okButtonText: "Next >").Any())
            {
                return true;
            }

            return serviceMenuTypeField.ValueAsEnum<ServiceMenuType>() switch
            {
                ServiceMenuType.FactoryMenu => await OpenFactoryMenu(),
                ServiceMenuType.HospitalityMenu => await OpenHospitalityMenu(),
                _ => throw new NotImplementedException()
            };
        }

        private async Task<bool> OpenHospitalityMenu()
        {
            await ExecutePresetWithProgressAsync(SamsungHospitalityMenuPresets.Preset1, "Step 1: opening Service Menu. It is normal to see messages like 'Not Available'.");

            if (!MessageForms.ShowDialog($"Access Service Menu - Step 2", [
                    new FieldDefinition
                    {
                        Label = "Service Menu opened?",
                        SubLabel = "If the menu is opened with the 'Hospitality Mode' item highlighted and the rest disabled, click 'Next >' to continue or 'X' to stop",
                        FieldType = FieldType.Label
                    } ], okButtonText: "Next >").Any())
            {
                return true;
            }

            await ExecutePresetWithProgressAsync(SamsungHospitalityMenuPresets.Preset2, "Step 2: accessing Service Menu...");

            var values = MessageForms.ShowDialog($"Access Service Menu - Step 3", new[] {
                    new FieldDefinition
                    {
                        Label = "Service Menu - Advanced",
                        SubLabel = "If the 'Advanced' item is highlighted and you want to access these settings, click 'Next >' or click 'X' to skip",
                        FieldType = FieldType.Label
                    } }, okButtonText: "Next >");

            if (values.Any())
            {
                await ExecutePresetWithProgressAsync(SamsungHospitalityMenuPresets.Preset3, "Step 3: accessing Advanced Service Menu...");
            }

            if (!MessageForms.ShowDialog($"Reboot TV - Final Step", new[] {
                    new FieldDefinition
                    {
                        Label = "Reboot TV",
                        SubLabel = "After you have made the necessary changes, click 'Reboot TV' to reboot the TV or click 'X' to close. If the TV turns not back on automatically, power it on manually.",
                        FieldType = FieldType.Label
                    } }, okButtonText: "Reboot TV").Any())
            {
                return true;
            }

            await ExecutePresetWithProgressAsync(SamsungGenericPresets.Reboot, "Final Step: rebooting TV...");

            return true;
        }

        private async Task<bool> OpenFactoryMenu()
        {
            await ExecutePresetWithProgressAsync(SamsungFactoryMenuPresets.Preset1, "Step 1: opening Service Menu. It is normal to see messages like 'Not Available'.");

            var values = MessageForms.ShowDialog($"Access Service Menu - Step 2", new[] {
                    new FieldDefinition
                    {
                        Label = "Service Menu - Advanced",
                        SubLabel = "If the service menu opened and you want to access the Advanced settings, click 'Next >' or click 'X' to skip",
                        FieldType = FieldType.Label
                    } }, okButtonText: "Next >");

            if (values.Any())
            {
                await ExecutePresetWithProgressAsync(SamsungFactoryMenuPresets.PresetAdvanced, "Step 2: accessing Advanced Service Menu...");
            }

            var exitServiceMenuTypes = new[] { ExitServiceMenuType.Exit, ExitServiceMenuType.Reboot }.Select(t => Utils.GetDescription(t));

            var exitServiceMenuTypeField = new FieldDefinition
            {
                Label = "Exit Service Menu Type",
                SubLabel = "Choose how to exit the service menu. You can either reboot the TV or just exit the service menu (if, for example, no changes have been made)",
                FieldType = FieldType.DropDown,
                Values = exitServiceMenuTypes,
                Value = ExitServiceMenuType.Exit
            };

            if (!MessageForms.ShowDialog($"Exit Service Menu", [exitServiceMenuTypeField], okButtonText: "Next >").Any())
            {
                return true;
            }

            var task = exitServiceMenuTypeField.ValueAsEnum<ExitServiceMenuType>() switch
            {
                ExitServiceMenuType.Exit => ExecutePresetWithProgressAsync(SamsungFactoryMenuPresets.Exit, "Final Step: exiting service menu..."),
                ExitServiceMenuType.Reboot => ExecutePresetWithProgressAsync(SamsungGenericPresets.Reboot, "Final Step: rebooting TV..."),
                _ => throw new NotImplementedException()
            };

            await task;

            return true;
        }

        private async Task ExecutePresetWithProgressAsync(SamsungPreset preset, string text)
        {
            var progressForm = MessageForms.ShowProgress(text);
            try
            {
                //await Task.Delay(5000);
                await ExecutePresetAsync(preset);
            }
            finally
            {
                progressForm.Close();
            }
        }

        internal async Task<bool> ExecuteInvokableAction(InvokableActionDto<SamsungPreset> invokableAction, GlobalContext appContext, SamsungServiceConfig config)
        {
            var action = _invokableActions.FirstOrDefault(a => a.Name == invokableAction.Name);

            if (action == null)
            {
                return false;
            }

            await ExecuteActionAsync(action, [], appContext, config);

            return true;
        }
    }
}

