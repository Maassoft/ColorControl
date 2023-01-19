using ColorControl.Common;
using LgTv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Services.LG
{
    class LgDevice
    {
        public class InvokableAction
        {
            public Func<Dictionary<string, object>, bool> Function { get; set; }
            public string Name { get; set; }
            public Type EnumType { get; set; }
            public decimal MinValue { get; set; }
            public decimal MaxValue { get; set; }
            public string Category { get; set; }
            public string Title { get; set; }
            public int CurrentValue { get; set; }
            public int NumberOfValues { get; set; }
            public LgPreset Preset { get; set; }
            public bool Advanced { get; set; }
            public ModelYear FromModelYear { get; set; } = ModelYear.None;
            public ModelYear ToModelYear { get; set; } = ModelYear.None;
        }

        public class LgDevicePictureSettings
        {
            public int Backlight { get; set; }
            public int Contrast { get; set; }
            public int Brightness { get; set; }
            public int Color { get; set; }
        }

        public enum PowerState
        {
            Unknown,
            Active,
            Power_Off,
            Suspend,
            Active_Standby,
            Screen_Off
        }

        public enum PowerOffSource
        {
            Unknown,
            App,
            Manually
        }

        public enum ModelYear
        {
            None = 0,
            SeriesPre2018,
            Series2018,
            Series2019,
            Series2020,
            Series2021,
            Series2022
        }

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static Func<string, string[], bool> ExternalServiceHandler;
        public static List<string> DefaultActionsOnGameBar = new() { "backlight", "contrast", "brightness", "color" };

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
        public bool PowerOffOnScreenSaver { get; set; }
        public bool PowerOnAfterScreenSaver { get; set; }
        public bool PowerOnAfterManualPowerOff { get; set; }
        public bool PowerOnByWindows { get; set; }
        public bool PowerOffByWindows { get; set; }
        public bool TriggersEnabled { get; set; }
        public int HDMIPortNumber { get; set; }

        public bool UseSecureConnection { get; set; } = true;

        private List<string> _actionsForGameBar;

        public List<string> ActionsOnGameBar
        {
            get
            {
                return _actionsForGameBar;
            }
            set
            {
                if (value?.Any() == false)
                {
                    _actionsForGameBar = DefaultActionsOnGameBar;
                }
                else
                {
                    _actionsForGameBar = value;
                }
            }
        }

        [JsonIgnore]
        public PowerState CurrentState { get; set; }

        [JsonIgnore]
        private LgTvApi _lgTvApi;
        [JsonIgnore]
        private bool _justWokeUp;
        [JsonIgnore]
        public PowerOffSource PoweredOffBy { get; private set; }

        [JsonIgnore]
        public bool PoweredOffViaApp { get; private set; }
        [JsonIgnore]
        private DateTimeOffset _poweredOffViaAppDateTime { get; set; }
        [JsonIgnore]
        private List<InvokableAction> _invokableActions = new List<InvokableAction>();
        [JsonIgnore]
        private SemaphoreSlim _connectSemaphore = new SemaphoreSlim(1, 1);

        [JsonIgnore]
        public string ModelName { get; private set; }
        [JsonIgnore]
        public ModelYear Year { get; private set; }

        [JsonIgnore]
        public LgDevicePictureSettings PictureSettings { get; private set; }

        [JsonIgnore]
        public string CurrentAppId { get; private set; }

        public event EventHandler PictureSettingsChangedEvent;
        public event EventHandler PowerStateChangedEvent;

        [JsonConstructor]
        public LgDevice(string name, string ipAddress, string macAddress, bool isCustom = true, bool isDummy = false)
        {
            PictureSettings = new LgDevicePictureSettings();
            ActionsOnGameBar = new List<string>(DefaultActionsOnGameBar);

            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
            IsCustom = isCustom;
            IsDummy = isDummy;
            TriggersEnabled = true;

            AddInvokableAction("WOL", new Func<Dictionary<string, object>, bool>(WakeAction));
            AddGenericPictureAction("backlight", minValue: 0, maxValue: 100);
            AddGenericPictureAction("brightness", minValue: 0, maxValue: 100, title: "Brightness/Black Level");
            AddGenericPictureAction("contrast", minValue: 0, maxValue: 100);
            AddGenericPictureAction("color", minValue: 0, maxValue: 100);
            AddGenericPictureAction("pictureMode", typeof(PictureMode), title: "Picture Mode");
            AddGenericPictureAction("sharpness", minValue: 0, maxValue: 50);
            //AddGenericPictureAction("dynamicRange", typeof(DynamicRange), category: "dimensionInfo", title: "Dynamic Range");
            AddGenericPictureAction("colorGamut", typeof(ColorGamut), title: "Color Gamut");
            AddGenericPictureAction("dynamicContrast", typeof(OffToHigh), title: "Dynamic Contrast");
            AddGenericPictureAction("gamma", typeof(GammaExp));
            AddGenericPictureAction("colorTemperature", minValue: -50, maxValue: 50, title: "Color Temperature");
            AddGenericPictureAction("whiteBalanceColorTemperature", typeof(WhiteBalanceColorTemperature), title: "White Balance Color Temperature");
            AddGenericPictureAction("eyeComfortMode", typeof(OffToOn), title: "Eye Comfort Mode");
            //AddGenericPictureAction("dynamicColor", typeof(OffToAuto));
            //AddGenericPictureAction("superResolution", typeof(OffToAuto));
            AddGenericPictureAction("peakBrightness", typeof(OffToHigh), title: "Peak Brightness", fromModelYear: ModelYear.Series2019);
            AddGenericPictureAction("smoothGradation", typeof(OffToAuto), title: "Smooth Gradation", fromModelYear: ModelYear.Series2019);
            AddGenericPictureAction("energySaving", typeof(EnergySaving), title: "Energy Saving");
            AddGenericPictureAction("hdrDynamicToneMapping", typeof(DynamicTonemapping), title: "HDR Dynamic Tone Mapping");
            AddGenericPictureAction("blackLevel", typeof(BlackLevel), title: "HDMI Black Level");
            AddGenericPictureAction("arcPerApp", typeof(AspectRatio), title: "Aspect Ratio", category: "aspectRatio");
            AddGenericPictureAction("justScan", typeof(OffToAuto2), title: "Just Scan", category: "aspectRatio");
            AddGenericPictureAction("allDirZoomHRatio", minValue: 0, maxValue: 10, title: "All-Direction Zoom Horizontal Ratio", category: "aspectRatio");
            AddGenericPictureAction("allDirZoomVRatio", minValue: 0, maxValue: 9, title: "All-Direction Zoom Vertical Ratio", category: "aspectRatio");
            AddGenericPictureAction("allDirZoomHPosition", minValue: -10, maxValue: 9, title: "All-Direction Zoom Horizontal Position", category: "aspectRatio");
            AddGenericPictureAction("allDirZoomVPosition", minValue: -9, maxValue: 9, title: "All-Direction Zoom Vertical Position", category: "aspectRatio");
            AddGenericPictureAction("vertZoomVPosition", minValue: -8, maxValue: 9, title: "Vertical Zoom Position", category: "aspectRatio");
            AddGenericPictureAction("vertZoomVRatio", minValue: 0, maxValue: 9, title: "Vertical Zoom Ratio", category: "aspectRatio");
            //AddGenericPictureAction("ambientLightCompensation", typeof(OffToAuto2));
            AddGenericPictureAction("truMotionMode", typeof(TruMotionMode), title: "TruMotion");
            AddGenericPictureAction("truMotionJudder", minValue: 0, maxValue: 10, title: "TruMotion Judder");
            AddGenericPictureAction("truMotionBlur", minValue: 0, maxValue: 10, title: "TruMotion Blur");
            AddGenericPictureAction("motionProOLED", typeof(OffToHigh), title: "OLED Motion Pro", fromModelYear: ModelYear.Series2019);
            AddGenericPictureAction("motionPro", typeof(OffToOn), title: "Motion Pro");
            AddGenericPictureAction("realCinema", typeof(OffToOn), title: "Real Cinema");
            AddGenericPictureAction("uhdDeepColorHDMI1", typeof(OffToOn), category: "other");
            AddGenericPictureAction("uhdDeepColorHDMI2", typeof(OffToOn), category: "other");
            AddGenericPictureAction("uhdDeepColorHDMI3", typeof(OffToOn), category: "other");
            AddGenericPictureAction("uhdDeepColorHDMI4", typeof(OffToOn), category: "other");
            AddGenericPictureAction("lowLevelAdjustment", minValue: -30, maxValue: 30, category: "other", title: "Fine Tune Dark Areas", fromModelYear: ModelYear.Series2019);
            AddGenericPictureAction("blackStabilizer", minValue: -30, maxValue: 30, category: "other", title: "Black Stabilizer", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("whiteStabilizer", minValue: -30, maxValue: 30, category: "other", title: "White Stabilizer", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("blueLight", typeof(BlueLight), category: "other", title: "Reduce Blue Light", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameMode_hdmi1", typeof(OffToOn), category: "other", title: "Game Optimizer HDMI1", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameMode_hdmi2", typeof(OffToOn), category: "other", title: "Game Optimizer HDMI2", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameMode_hdmi3", typeof(OffToOn), category: "other", title: "Game Optimizer HDMI3", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameMode_hdmi4", typeof(OffToOn), category: "other", title: "Game Optimizer HDMI4", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameOptimization", typeof(OffToOn), category: "other", title: "VRR & G-Sync", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("inputOptimization", typeof(InputOptimization), category: "other", title: "Prevent Input Delay(Input Lag)", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameOptimizationHDMI1", typeof(OffToOn), category: "other");
            AddGenericPictureAction("gameOptimizationHDMI2", typeof(OffToOn), category: "other");
            AddGenericPictureAction("gameOptimizationHDMI3", typeof(OffToOn), category: "other");
            AddGenericPictureAction("gameOptimizationHDMI4", typeof(OffToOn), category: "other");
            AddGenericPictureAction("freesync", typeof(OffToOn), category: "other", title: "AMD FreeSync Premium", fromModelYear: ModelYear.Series2020);
            AddGenericPictureAction("freesyncOLEDHDMI1", typeof(OffToOn), category: "other", fromModelYear: ModelYear.Series2020);
            AddGenericPictureAction("freesyncOLEDHDMI2", typeof(OffToOn), category: "other", fromModelYear: ModelYear.Series2020);
            AddGenericPictureAction("freesyncOLEDHDMI3", typeof(OffToOn), category: "other", fromModelYear: ModelYear.Series2020);
            AddGenericPictureAction("freesyncOLEDHDMI4", typeof(OffToOn), category: "other", fromModelYear: ModelYear.Series2020);
            //AddGenericPictureAction("freesyncSupport", typeof(OffToOn), category: "other");
            AddGenericPictureAction("hdmiPcMode_hdmi1", typeof(FalseToTrue), category: "other");
            AddGenericPictureAction("hdmiPcMode_hdmi2", typeof(FalseToTrue), category: "other");
            AddGenericPictureAction("hdmiPcMode_hdmi3", typeof(FalseToTrue), category: "other");
            AddGenericPictureAction("hdmiPcMode_hdmi4", typeof(FalseToTrue), category: "other");
            AddGenericPictureAction("adjustingLuminance", minValue: -50, maxValue: 50, numberOfValues: 22);
            AddGenericPictureAction("whiteBalanceBlue", minValue: -50, maxValue: 50, numberOfValues: 22);
            AddGenericPictureAction("whiteBalanceGreen", minValue: -50, maxValue: 50, numberOfValues: 22);
            AddGenericPictureAction("whiteBalanceRed", minValue: -50, maxValue: 50, numberOfValues: 22);
            //AddGenericPictureAction("wb20PointsGammaValue", minValue: -50, maxValue: 50);
            AddInvokableAction("turnScreenOff", new Func<Dictionary<string, object>, bool>(TurnScreenOffAction));
            AddInvokableAction("turnScreenOn", new Func<Dictionary<string, object>, bool>(TurnScreenOnAction));

            AddInternalPresetAction(new LgPreset("InStart", "com.webos.app.factorywin", new[] { "0", "4", "1", "3" }, new { id = "executeFactory", irKey = "inStart" }));
            AddInternalPresetAction(new LgPreset("EzAdjust", "com.webos.app.factorywin", new[] { "0", "4", "1", "3" }, new { id = "executeFactory", irKey = "ezAdjust" }));
            //AddInternalPresetAction(new LgPreset("PictureCheck", "com.webos.app.factorywin", null, new { id = "executeFactory", irKey = "pCheck" }));
            AddInternalPresetAction(new LgPreset("Software Update", "com.webos.app.softwareupdate", null, new { mode = "user", flagUpdate = true }));

            AddSetDeviceConfigAction("HDMI_1_icon", typeof(HdmiIcon), "HDMI 1 icon");
            AddSetDeviceConfigAction("HDMI_2_icon", typeof(HdmiIcon), "HDMI 2 icon");
            AddSetDeviceConfigAction("HDMI_3_icon", typeof(HdmiIcon), "HDMI 3 icon");
            AddSetDeviceConfigAction("HDMI_4_icon", typeof(HdmiIcon), "HDMI 4 icon");

            AddGenericPictureAction("soundMode", typeof(SoundMode), category: "sound", title: "Sound Mode");
            AddGenericPictureAction("soundOutput", typeof(SoundOutput), category: "sound", title: "Sound Output");
            AddGenericPictureAction("autoVolume", typeof(OffToOn), category: "sound", title: "Auto Volume");

            //AddGenericPictureAction("enableToastPopup", typeof(OffToOn), category: "option", title: "enableToastPopup");
            AddSetConfigAction("tv.conti.supportUsedTime", typeof(BoolFalseToTrue), title: "Total Power On Time");

            AddGenericPictureAction("wolwowlOnOff", typeof(FalseToTrue), category: "network", title: "Wake-On-LAN");
        }

        private void AddInvokableAction(string name, Func<Dictionary<string, object>, bool> function)
        {
            var action = new InvokableAction
            {
                Name = name,
                Function = function
            };

            _invokableActions.Add(action);
        }

        private void AddInternalPresetAction(LgPreset preset)
        {
            var action = new InvokableAction
            {
                Name = preset.name,
                Preset = preset
            };

            _invokableActions.Add(action);
        }

        private void AddGenericPictureAction(string name, Type type = null, decimal minValue = 0, decimal maxValue = 0, string category = "picture", string title = null, int numberOfValues = 1, ModelYear fromModelYear = ModelYear.None)
        {
            var action = new InvokableAction
            {
                Name = name,
                Function = new Func<Dictionary<string, object>, bool>(GenericPictureAction),
                EnumType = type,
                MinValue = minValue,
                MaxValue = maxValue,
                NumberOfValues = numberOfValues,
                Category = category,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title,
                FromModelYear = fromModelYear
            };

            _invokableActions.Add(action);
        }

        private void AddSetDeviceConfigAction(string name, Type type, string title)
        {
            var action = new InvokableAction
            {
                Name = name,
                Function = new Func<Dictionary<string, object>, bool>(GenericDeviceConfigAction),
                EnumType = type,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title
            };

            _invokableActions.Add(action);
        }

        private void AddSetConfigAction(string name, Type type, string title)
        {
            var action = new InvokableAction
            {
                Name = name,
                Function = new Func<Dictionary<string, object>, bool>(GenericSetConfigAction),
                EnumType = type,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title,
                Category = "Config",
                Advanced = true
            };

            _invokableActions.Add(action);
        }

        public void AddGameBarAction(string name)
        {
            if (!ActionsOnGameBar.Contains(name))
            {
                ActionsOnGameBar.Add(name);
            }
        }

        public void RemoveGameBarAction(string name)
        {
            ActionsOnGameBar.Remove(name);
        }

        public override string ToString()
        {
            //return (IsDummy ? string.Empty : (IsCustom ? "Custom: " : "Auto detect: ")) + $"{Name}" + (!string.IsNullOrEmpty(IpAddress) ? $" ({IpAddress})" : string.Empty);
            return $"{(IsDummy ? string.Empty : (IsCustom ? "Custom: " : "Auto detect: "))}{Name}{(!string.IsNullOrEmpty(IpAddress) ? ", " + IpAddress : string.Empty)}";
        }

        public async Task<bool> Connect(int retries = 3)
        {
            var locked = _connectSemaphore.CurrentCount == 0;
            await _connectSemaphore.WaitAsync();
            try
            {
                if (locked && _lgTvApi != null)
                {
                    return true;
                }

                try
                {
                    DisposeConnection();
                    _lgTvApi = await LgTvApi.CreateLgTvApi(IpAddress, retries, UseSecureConnection);

                    //Test();
                    //_lgTvApi.Test3();
                    if (_lgTvApi != null && !Utils.ConsoleOpened)
                    {
                        var info = await _lgTvApi.GetSystemInfo("modelName");
                        if (info != null)
                        {
                            ModelName = info.modelName;
                            SetModelYear();
                        }

                        //await _lgTvApi.SubscribeVolume(VolumeChanged);
                        await _lgTvApi.SubscribePowerState(PowerStateChanged);
                        await _lgTvApi.SubscribePictureSettings(PictureSettingsChanged);
                        await _lgTvApi.SubscribeForegroundApp(ForegroundAppChanged);

                        //await _lgTvApi.SetConfig("tv.conti.supportUsedTime", true);

                        //await _lgTvApi.Reboot();

                        //var result = await GetPictureSettings();

                        //await _lgTvApi.SetSystemSettings("adjustingLuminance", new[] { 0, 0, -5, -10, -15, -20, -25, -30, -35, -40, -45, -50, -50, -50, -40, -30, -20, -10, 10, 20, 30, 50 });
                        //await _lgTvApi.SetSystemSettings("adjustingLuminance", new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

                        //await _lgTvApi.SetInput("HDMI_1");
                        //await Task.Delay(2000);
                        //await _lgTvApi.SetConfig("com.palm.app.settings.enableHdmiPcLabel", true);
                        //await _lgTvApi.SetInput("HDMI_2");
                    }
                    return _lgTvApi != null;
                }
                catch (Exception ex)
                {
                    string logMessage = ex.ToLogString(Environment.StackTrace);
                    Logger.Error($"Error while connecting to {IpAddress}: {logMessage}");
                    return false;
                }
            }
            finally
            {
                _connectSemaphore.Release();
            }
        }

        private void SetModelYear()
        {
            if (ModelName == null || ModelName.Contains("OLED") != true)
            {
                Year = ModelYear.None;
                return;
            }

            var normalized = ModelName.Replace("OLED", "").Replace(" ", "");

            if (normalized.Length > 3)
            {
                normalized = normalized.Substring(3);
            }

            var suffix = normalized.First();

            Year = suffix switch
            {
                '6' => ModelYear.SeriesPre2018,
                '7' => ModelYear.SeriesPre2018,
                '8' => ModelYear.Series2018,
                '9' => ModelYear.Series2019,
                'X' => ModelYear.Series2020,
                '1' => ModelYear.Series2021,
                '2' => ModelYear.Series2022,
                _ => ModelYear.None
            };

            _invokableActions = _invokableActions.Where(a => Year == ModelYear.None || a.FromModelYear == ModelYear.None || Year >= a.FromModelYear).ToList();
        }

        public bool VolumeChanged(dynamic payload)
        {
            return true;
        }

        public bool PictureSettingsChanged(dynamic payload)
        {
            if (payload.settings != null)
            {
                var settings = payload.settings;

                Logger.Debug($"[{Name}] PictureSettingsChanged: {JsonConvert.SerializeObject(settings)}");

                if (settings.backlight != null)
                {
                    PictureSettings.Backlight = Utils.ParseDynamicAsInt(settings.backlight, PictureSettings.Backlight);
                }
                if (settings.contrast != null)
                {
                    PictureSettings.Contrast = Utils.ParseDynamicAsInt(settings.contrast, PictureSettings.Contrast);
                }
                if (settings.brightness != null)
                {
                    PictureSettings.Brightness = Utils.ParseDynamicAsInt(settings.brightness, PictureSettings.Brightness);
                }
                if (settings.color != null)
                {
                    PictureSettings.Color = Utils.ParseDynamicAsInt(settings.color, PictureSettings.Color);
                }
            }

            PictureSettingsChangedEvent?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public bool PowerStateChanged(dynamic payload)
        {
            Logger.Debug($"[{Name}] Power state change: {JsonConvert.SerializeObject(payload)}");

            var state = payload.state != null ? ((string)payload.state).Replace(' ', '_') : PowerState.Unknown.ToString();

            PowerState newState;
            if (Enum.TryParse(state, out newState))
            {
                CurrentState = newState;

                if (CurrentState == PowerState.Active)
                {
                    if (payload.processing == null && (DateTimeOffset.Now - _poweredOffViaAppDateTime).TotalMilliseconds > 500)
                    {
                        PoweredOffViaApp = false;
                        PoweredOffBy = PowerOffSource.Unknown;
                        _poweredOffViaAppDateTime = DateTimeOffset.MinValue;
                    }
                    else if (payload.processing != null && ((string)payload.processing).Equals("Request Power Off", StringComparison.Ordinal))
                    {
                        PoweredOffBy = PoweredOffViaApp ? PowerOffSource.App : PowerOffSource.Manually;
                    }
                }
                else
                {
                    PoweredOffBy = PoweredOffViaApp ? PowerOffSource.App : PowerOffSource.Manually;
                }
            }
            else
            {
                CurrentState = PowerState.Unknown;
                Logger.Warn($"Unknown power state: {state}");
            }

            Logger.Debug($"PoweredOffBy: {PoweredOffBy}, PoweredOffViaApp: {PoweredOffViaApp}");

            PowerStateChangedEvent?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public bool ForegroundAppChanged(dynamic payload)
        {
            Logger.Debug($"[{Name}] ForegroundAppChanged: {JsonConvert.SerializeObject(payload)}");

            if (payload.appId != null)
            {
                CurrentAppId = payload.appId;
            }

            return true;
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

        public async Task<bool> ExecutePreset(LgPreset preset, bool reconnect, LgServiceConfig config)
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
                        var @params = preset.AppParams;
                        if (@params == null && config.ShowAdvancedActions)
                        {
                            if (preset.appId.Equals("com.webos.app.softwareupdate"))
                            {
                                @params = new { mode = "user", flagUpdate = true };
                            }
                            else if (preset.appId.Equals("com.webos.app.factorywin"))
                            {
                                if (preset.name.Contains("ezadjust", StringComparison.OrdinalIgnoreCase))
                                {
                                    @params = new { id = "executeFactory", irKey = "ezAdjust" };
                                }
                                else
                                {
                                    @params = new { id = "executeFactory", irKey = "inStart" };
                                }
                            }
                        }

                        await _lgTvApi.LaunchApp(preset.appId, @params);
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
            LgWebOsMouseService mouse = null;

            foreach (var step in preset.steps)
            {
                var keySpec = step.Split(':');

                var delay = 0;
                var key = step;
                if (keySpec.Length == 2)
                {
                    delay = Utils.ParseInt(keySpec[1]);
                    if (delay > 0)
                    {
                        key = keySpec[0];
                    }
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
                    ExecuteAction(action, parameters);

                    executeKey = false;
                }
                if (ExternalServiceHandler != null && parameters != null)
                {
                    if (ExternalServiceHandler(key, parameters))
                    {
                        executeKey = false;
                    }
                }

                if (executeKey)
                {
                    mouse ??= await api.GetMouse();
                    SendKey(mouse, key);
                    delay = delay == 0 ? 180 : delay;
                }

                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
        }

        public void ExecuteAction(InvokableAction action, string[] parameters)
        {
            var function = action.Function;
            if (function == null)
            {
                return;
            }

            if (parameters?.Length > 0)
            {
                var keyValues = new Dictionary<string, object> {
                    { "name", action.Name },
                    { "value", parameters },
                    { "category", action.Category }
                };

                function(keyValues);
                return;
            }

            function(null);
        }

        private void SendKey(LgWebOsMouseService mouse, string key)
        {
            key = key.ToUpper();
            if (key.Length >= 1 && int.TryParse(key[0].ToString(), out _))
            {
                key = "_" + key;
            }
            var button = (ButtonType)Enum.Parse(typeof(ButtonType), key);
            mouse.SendButton(button);
        }

        public async Task<LgWebOsMouseService> GetMouseAsync()
        {
            await CheckConnectionAsync();

            return await _lgTvApi.GetMouse();
        }

        private async Task CheckConnectionAsync()
        {
            if (!await Connected())
            {
                throw new InvalidOperationException("Not connected");
            }
        }

        private void CheckConnection()
        {
            var task = CheckConnectionAsync();

            if (task.Status != TaskStatus.RanToCompletion)
            {
                throw new InvalidOperationException("Not connected");
            }
        }

        public async Task<IEnumerable<LgApp>> GetApps(bool force = false)
        {
            if (!force)
            {
                await Task.Delay(5000);
            }

            if (!await Connected(force))
            {
                Logger.Debug("Cannot refresh apps: no connection could be made");
                return new List<LgApp>();
            }

            return await _lgTvApi.GetApps(force);
        }

        internal async Task<bool> PowerOff(bool checkHdmi = false)
        {
            if (!await Connected(true) || CurrentState != PowerState.Active)
            {
                return false;
            }

            if (checkHdmi && CurrentAppId != null && HDMIPortNumber != 0 && !CurrentAppId.EndsWith($".hdmi{HDMIPortNumber}", StringComparison.InvariantCulture))
            {
                Logger.Debug($"[{Name}] PowerOff is ignored because current app {CurrentAppId} does not match configured HDMI port {HDMIPortNumber}");
                return true;
            }

            PoweredOffViaApp = true;
            _poweredOffViaAppDateTime = DateTimeOffset.Now;

            try
            {
                await _lgTvApi.TurnOff().WaitAsync(TimeSpan.FromSeconds(2));
            }
            catch (TimeoutException ex)
            {
                Logger.Debug($"Timeout when turning off tv: {ex.Message}");
            }

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
                await Task.Delay(wakeDelay);
                var result = Wake();
                if (!result)
                {
                    Logger.Debug("WOL failed");
                    return false;
                }
                Logger.Debug("WOL succeeded");
                await Task.Delay(connectDelay);
                result = await Connect(1);
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
                result = WOL.WakeFunctionCheckAdmin(MacAddress, IpAddress);
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

            return result;
        }

        public List<InvokableAction> GetInvokableActions(bool includedAdvanced = false)
        {
            return _invokableActions.Where(a => includedAdvanced || !a.Advanced).ToList();
        }

        public List<InvokableAction> GetInvokableActionsForGameBar()
        {
            var actions = GetInvokableActions();
            return actions.Where(a => a.Category == "picture" && (a.EnumType != null && a.EnumType != typeof(PictureMode) || a.MinValue >= 0 && a.MaxValue > 0)).ToList();
        }

        public List<InvokableAction> GetActionsForGameBar()
        {
            var actions = GetInvokableActions();
            return actions.Where(a => ActionsOnGameBar.Contains(a.Name)).ToList();
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

        public bool IsUsingHDRPictureMode()
        {
            // Temporary workaround because I cannot read the picture mode at this time
            return PictureSettings.Contrast == 100;
        }

        internal async Task SetBacklight(int backlight)
        {
            await CheckConnectionAsync();

            await _lgTvApi.SetSystemSettings("backlight", backlight.ToString());
        }

        internal async Task SetContrast(int contrast)
        {
            await CheckConnectionAsync();

            await _lgTvApi.SetSystemSettings("contrast", contrast.ToString());
        }

        public async Task SetOLEDMotionPro(string mode)
        {
            await CheckConnectionAsync();

            await _lgTvApi.SetConfig("tv.model.motionProMode", mode);
        }

        internal async Task SetConfig(string key, object value)
        {
            await CheckConnectionAsync();

            await _lgTvApi.SetConfig(key, value);
        }
        internal async Task SetSystemSettings(string name, string value)
        {
            await CheckConnectionAsync();

            await _lgTvApi.SetSystemSettings(name, value);

            UpdateCurrentValueOfAction(name, value);
        }

        internal async Task<dynamic> GetPictureSettings()
        {
            //var keys = new[] { "backlight", "brightness", "contrast", "color", "pictureMode", "colorGamut", "dynamicContrast", "peakBrightness", "smoothGradation", "energySaving", "motionProOLED" };
            //var keys = new[] { "backlight", "brightness", "contrast", "color" };

            await CheckConnectionAsync();

            return await _lgTvApi.GetSystemSettings2("picture");
        }

        private bool WakeAction(Dictionary<string, object> parameters)
        {
            return Wake();
        }

        private bool TurnScreenOffAction(Dictionary<string, object> parameters)
        {
            CheckConnection();

            var task = _lgTvApi.TurnScreenOff();
            Utils.WaitForTask(task);

            return true;
        }

        private bool TurnScreenOnAction(Dictionary<string, object> parameters)
        {
            CheckConnection();

            var task = _lgTvApi.TurnScreenOn();
            Utils.WaitForTask(task);

            return true;
        }

        private bool GenericPictureAction(Dictionary<string, object> parameters)
        {
            CheckConnection();

            var settingName = parameters["name"].ToString();
            var stringValues = parameters["value"] as string[];
            var category = parameters["category"].ToString();
            object value = stringValues[0];
            if (stringValues.Length > 1)
            {
                value = stringValues.Select(s => int.Parse(s)).ToArray();
            }
            var task = _lgTvApi.SetSystemSettings(settingName, value, category);
            Utils.WaitForTask(task);

            UpdateCurrentValueOfAction(settingName, value.ToString());

            return true;
        }

        private bool GenericDeviceConfigAction(Dictionary<string, object> parameters)
        {
            CheckConnection();

            var id = parameters["name"].ToString().Replace("_icon", string.Empty);
            var stringValues = parameters["value"] as string[];
            var value = stringValues[0];

            var description = Utils.GetDescriptionByEnumName<HdmiIcon>(value);

            var task = _lgTvApi.SetDeviceConfig(id, value, description);
            Utils.WaitForTask(task);

            return true;
        }

        private bool GenericSetConfigAction(Dictionary<string, object> parameters)
        {
            CheckConnection();

            var key = parameters["name"].ToString();
            var values = parameters["value"] as object[];
            var value = values[0];

            var task = _lgTvApi.SetConfig(key, value);
            Utils.WaitForTask(task);

            return true;
        }

        private void UpdateCurrentValueOfAction(string settingName, string value)
        {
            if (settingName == "backlight" || settingName == "contrast" || settingName == "brightness" || settingName == "color")
            {
                return;
            }
            var action = _invokableActions.FirstOrDefault(a => a.Name == settingName);
            if (action != null)
            {
                if (action.EnumType != null)
                {
                    try
                    {
                        var enumValue = Enum.Parse(action.EnumType, value);
                        var intEnum = (int)enumValue;
                        action.CurrentValue = intEnum;
                    }
                    catch (Exception) { }
                }
                else
                {
                    action.CurrentValue = 0;
                }
            }
        }
    }
}
