using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.LG;
using ColorControl.Shared.Services;
using LgTv;
using Microsoft.Extensions.DependencyInjection;
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
            public Func<Dictionary<string, object>, Task<bool>> AsyncFunction { get; set; }
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
            public List<string> ValueLabels { get; set; }
        }

        public class LgDevicePictureSettings
        {
            public int Backlight { get; set; }
            public int Contrast { get; set; }
            public int Brightness { get; set; }
            public int Color { get; set; }
            public int Volume { get; set; }
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
            Series2022,
            Series2023
        }

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static List<string> DefaultActionsOnGameBar = new() { "backlight", "contrast", "brightness", "color" };

        private static List<string> IreValueLabels = ["IRE 2.5", "IRE 5", "IRE 7.5", "IRE 10", "IRE 15", "IRE 20", "IRE 25", "IRE 30", "IRE 35", "IRE 40", "IRE 45", "IRE 50", "IRE 55", "IRE 60", "IRE 65", "IRE 70", "IRE 75", "IRE 80", "IRE 85", "IRE 90", "IRE 95", "IRE 100"];
        private static List<string> Ire10ptValueLabels = ["IRE 10", "IRE 20", "IRE 30", "IRE 40", "IRE 50", "IRE 60", "IRE 70", "IRE 80", "IRE 90", "IRE 100"];

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
        public int ScreenSaverMinimalDuration { get; set; }
        public bool TurnScreenOffOnScreenSaver { get; set; }
        public bool HandleManualScreenSaver { get; set; }
        public bool PowerOnAfterScreenSaver { get; set; }
        public bool TurnScreenOnAfterScreenSaver { get; set; }
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

        [JsonIgnore]
        private Timer _powerOffTimer;

        [JsonIgnore]
        private ServiceManager _serviceManager;

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

            AddInvokableAction("WOL", WakeAction);
            AddInvokableAction("Reboot", RebootAction);
            AddGenericPictureAction("backlight", minValue: 0, maxValue: 100);
            AddGenericPictureAction("brightness", minValue: 0, maxValue: 100, title: "Brightness/Black Level");
            AddGenericPictureAction("contrast", minValue: 0, maxValue: 100);
            AddGenericPictureAction("color", minValue: 0, maxValue: 100);
            AddGenericPictureAction("pictureMode", typeof(PictureMode), title: "Picture Mode");
            AddGenericPictureAction("sharpness", minValue: 0, maxValue: 50);
            //AddGenericPictureAction("dynamicRange", typeof(DynamicRange), category: "dimensionInfo", title: "Dynamic Range");
            AddGenericPictureAction("colorGamut", typeof(ColorGamut), title: "Color Gamut");
            AddGenericPictureAction("dynamicContrast", typeof(OffToHigh), title: "Dynamic Contrast");
            AddGenericPictureAction("superResolution", typeof(OffToHigh), title: "Super Resolution");
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
            AddGenericPictureAction("dolbyPrecisionDetail", typeof(OffToOn), title: "Dolby Precision Detail", fromModelYear: ModelYear.Series2022);

            AddGenericPictureAction("ai_Brightness", typeof(OffToOn), title: "AI Brightness", fromModelYear: ModelYear.Series2019, category: "aiPicture");
            AddGenericPictureAction("ai_Genre", typeof(OffToOn), title: "AI Genre Selection", fromModelYear: ModelYear.Series2021, category: "aiPicture");
            AddGenericPictureAction("ai_Picture", typeof(OffToOn), title: "AI Picture Pro", fromModelYear: ModelYear.Series2021, category: "aiPicture");

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

            AddHdmiPictureAction("uhdDeepColorHDMI", typeof(OffToOn), category: "other");
            AddGenericPictureAction("lowLevelAdjustment", minValue: -30, maxValue: 30, category: "other", title: "Fine Tune Dark Areas", fromModelYear: ModelYear.Series2019);
            AddGenericPictureAction("blackStabilizer", minValue: -30, maxValue: 30, category: "other", title: "Black Stabilizer", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("whiteStabilizer", minValue: -30, maxValue: 30, category: "other", title: "White Stabilizer", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("blueLight", typeof(BlueLight), category: "other", title: "Reduce Blue Light", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("noiseReduction", typeof(OffToHigh), title: "Noise Reduction");
            AddGenericPictureAction("mpegNoiseReduction", typeof(OffToHigh), title: "MPEG Noise Reduction");

            AddHdmiPictureAction("gameMode_hdmi", typeof(OffToOn), category: "other", title: "Game Optimizer HDMI1", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("gameOptimization", typeof(OffToOn), category: "other", title: "VRR & G-Sync", fromModelYear: ModelYear.Series2021);
            AddGenericPictureAction("inputOptimization", typeof(InputOptimization), category: "other", title: "Prevent Input Delay(Input Lag)", fromModelYear: ModelYear.Series2021);
            AddHdmiPictureAction("gameOptimizationHDMI", typeof(OffToOn), category: "other");
            AddGenericPictureAction("freesync", typeof(OffToOn), category: "other", title: "AMD FreeSync Premium", fromModelYear: ModelYear.Series2020);
            AddHdmiPictureAction("freesyncOLEDHDMI", typeof(OffToOn), category: "other", fromModelYear: ModelYear.Series2020);

            AddHdmiPictureAction("444BypassHDMI", typeof(OffToOn), category: "other", title: "4:4:4 Pass Through HDMIX", fromModelYear: ModelYear.Series2023);
            AddGenericPictureAction("444BypassHDMINone", typeof(OffToOn), category: "other", title: "4:4:4 Pass Through Non-HDMI", fromModelYear: ModelYear.Series2023);
            AddGenericPictureAction("qmsVrr", typeof(OffToOn), category: "other", title: "QMS-VRR", fromModelYear: ModelYear.Series2023);

            AddGenericPictureAction("masterLuminanceLevel", typeof(MasterLuminanceLevel), category: "other");
            AddHdmiPictureAction("colorimetryHDMI", typeof(MasteringColor), category: "other", fromModelYear: ModelYear.Series2020);
            AddHdmiPictureAction("masteringColorHDMI", typeof(MasteringColor), category: "other", fromModelYear: ModelYear.Series2020);
            AddHdmiPictureAction("masteringPeakHDMI", typeof(MasteringNits), category: "other", fromModelYear: ModelYear.Series2020);
            AddHdmiPictureAction("maxCLLHDMI", typeof(MasteringNits), category: "other", fromModelYear: ModelYear.Series2020);
            AddHdmiPictureAction("maxFALLHDMI", typeof(MasteringNits), category: "other", fromModelYear: ModelYear.Series2020);

            AddHdmiPictureAction("hdmiPcMode_hdmi", typeof(FalseToTrue), category: "other");
            AddGenericPictureAction("adjustingLuminance", minValue: -50, maxValue: 50, numberOfValues: 22, valueLabels: IreValueLabels);
            AddGenericPictureAction("whiteBalanceBlue", minValue: -50, maxValue: 50, numberOfValues: 22, valueLabels: IreValueLabels);
            AddGenericPictureAction("whiteBalanceGreen", minValue: -50, maxValue: 50, numberOfValues: 22, valueLabels: IreValueLabels);
            AddGenericPictureAction("whiteBalanceRed", minValue: -50, maxValue: 50, numberOfValues: 22, valueLabels: IreValueLabels);
            AddGenericPictureAction("whiteBalanceBlue10pt", minValue: -50, maxValue: 50, numberOfValues: 10, valueLabels: Ire10ptValueLabels);
            AddGenericPictureAction("whiteBalanceGreen10pt", minValue: -50, maxValue: 50, numberOfValues: 10, valueLabels: Ire10ptValueLabels);
            AddGenericPictureAction("whiteBalanceRed10pt", minValue: -50, maxValue: 50, numberOfValues: 10, valueLabels: Ire10ptValueLabels);
            AddGenericPictureAction("whiteBalanceBlueOffset", minValue: -50, maxValue: 50, title: "White Balance - Blue Offset");
            AddGenericPictureAction("whiteBalanceBlueGain", minValue: -50, maxValue: 50, title: "White Balance - Blue Gain");
            AddGenericPictureAction("whiteBalanceGreenOffset", minValue: -50, maxValue: 50, title: "White Balance - Green Offset");
            AddGenericPictureAction("whiteBalanceGreenGain", minValue: -50, maxValue: 50, title: "White Balance - Green Gain");
            AddGenericPictureAction("whiteBalanceRedOffset", minValue: -50, maxValue: 50, title: "White Balance - Red Offset");
            AddGenericPictureAction("whiteBalanceRedGain", minValue: -50, maxValue: 50, title: "White Balance - Red Gain");
            AddGenericPictureAction("whiteBalanceMethod", typeof(WhiteBalanceMethod), title: "White Balance - Method");
            AddInvokableAction("turnScreenOff", TurnScreenOffAction);
            AddInvokableAction("turnScreenOn", TurnScreenOnAction);

            AddInternalPresetAction(new LgPreset("InStart", "com.webos.app.factorywin", new[] { "0", "4", "1", "3" }, new { id = "executeFactory", irKey = "inStart" }));
            AddInternalPresetAction(new LgPreset("EzAdjust", "com.webos.app.factorywin", new[] { "0", "4", "1", "3" }, new { id = "executeFactory", irKey = "ezAdjust" }));
            //AddInternalPresetAction(new LgPreset("PictureCheck", "com.webos.app.factorywin", null, new { id = "executeFactory", irKey = "pCheck" }));
            AddInternalPresetAction(new LgPreset("Software Update", "com.webos.app.softwareupdate", null, new { mode = "user", flagUpdate = true }));

            AddSetDeviceConfigAction("HDMI_1_icon", typeof(HdmiIcon), "HDMI 1 icon");
            AddSetDeviceConfigAction("HDMI_2_icon", typeof(HdmiIcon), "HDMI 2 icon");
            AddSetDeviceConfigAction("HDMI_3_icon", typeof(HdmiIcon), "HDMI 3 icon");
            AddSetDeviceConfigAction("HDMI_4_icon", typeof(HdmiIcon), "HDMI 4 icon");

            AddVolumeAction("audioVolume", "Volume");
            AddGenericPictureAction("soundMode", typeof(SoundMode), category: "sound", title: "Sound Mode");
            AddGenericPictureAction("smartSoundMode", typeof(OffToOn), category: "sound", title: "Adaptive Sound Control");
            AddGenericPictureAction("soundOutput", typeof(SoundOutput), category: "sound", title: "Sound Output");
            AddGenericPictureAction("autoVolume", typeof(OffToOn), category: "sound", title: "Auto Volume");
            AddGenericPictureAction("virtualSurround", typeof(OffToOn), category: "sound", title: "Virtual Surround");

            AddGenericPictureAction("screenShift", typeof(OffToOn), title: "Screen Shift");
            AddGenericPictureAction("logoLuminanceAdjust", typeof(LogoLuminance), title: "Logo Luminance");

            //AddGenericPictureAction("enableToastPopup", typeof(OffToOn), category: "option", title: "enableToastPopup");
            AddSetConfigAction("tv.conti.supportUsedTime", typeof(BoolFalseToTrue), title: "Total Power On Time");

            AddGenericPictureAction("wolwowlOnOff", typeof(FalseToTrue), category: "network", title: "Wake-On-LAN");
            AddLunaAction("TPC", typeof(BoolFalseToTrue), title: "Temporal Peak Luminance Control (TPC)", ModelYear.Series2020);
            AddLunaAction("GSR", typeof(BoolFalseToTrue), title: "Global Sticky/Stress Reduction (GSR)", ModelYear.Series2020);
        }

        public LgDevice(LgDeviceDto device) : this(device.Name, device.IpAddress, device.MacAddress, device.IsCustom, device.IsDummy)
        {
            UpdateOptions(device.Options);
        }

        private void UpdateOptions(LgDeviceOptions options)
        {
            UseSecureConnection = options.UseSecureConnection;
            TurnScreenOffOnScreenSaver = options.TurnScreenOffOnScreenSaver;
            TurnScreenOnAfterScreenSaver = options.TurnScreenOnAfterScreenSaver;
            ScreenSaverMinimalDuration = options.ScreenSaverMinimalDuration;
            HandleManualScreenSaver = options.HandleManualScreenSaver;
            PowerOffOnScreenSaver = options.PowerOffOnScreenSaver;
            PowerOnAfterScreenSaver = options.PowerOnAfterScreenSaver;
            PowerOnAfterManualPowerOff = options.PowerOnAfterManualPowerOff;
            PowerOnAfterResume = options.PowerOnAfterResume;
            PowerOffOnStandby = options.PowerOffOnStandby;
            PowerOffOnShutdown = options.PowerOffOnShutdown;
            PowerOffByWindows = options.PowerOffByWindows;
            PowerOnByWindows = options.PowerOnByWindows;
            PowerOnAfterStartup = options.PowerOnAfterStartup;
            TriggersEnabled = options.TriggersEnabled;
            HDMIPortNumber = options.HDMIPortNumber;
        }

        ~LgDevice()
        {
            ClearPowerOffTask();
        }

        public bool Update(LgDeviceDto device)
        {
            var reconnect = false;

            Name = device.Name;
            if (IpAddress != device.IpAddress)
            {
                IpAddress = device.IpAddress;
                reconnect = true;
            }
            MacAddress = device.MacAddress;
            IsCustom = device.IsCustom;
            IsDummy = device.IsDummy;

            UpdateOptions(device.Options);

            return reconnect;
        }

        private void AddInvokableAction(string name, Func<Dictionary<string, object>, Task<bool>> function)
        {
            var action = new InvokableAction
            {
                Name = name,
                AsyncFunction = function,
                Category = "misc",
                Title = name
            };

            _invokableActions.Add(action);
        }

        private void AddInternalPresetAction(LgPreset preset)
        {
            var action = new InvokableAction
            {
                Name = preset.name,
                Preset = preset,
                Advanced = true
            };

            _invokableActions.Add(action);
        }

        private void AddHdmiPictureAction(string name, Type type = null, decimal minValue = 0, decimal maxValue = 0, string category = "picture", string title = null, int numberOfValues = 1, ModelYear fromModelYear = ModelYear.None)
        {
            for (var i = 1; i <= 4; i++)
            {
                var replacedTitle = title?.Replace("HDMIX", $"HDMI{i}");

                AddGenericPictureAction($"{name}{i}", type, minValue, maxValue, category, replacedTitle, numberOfValues, fromModelYear);
            }
        }

        private void AddGenericPictureAction(string name, Type type = null, decimal minValue = 0, decimal maxValue = 0, string category = "picture", string title = null, int numberOfValues = 1, ModelYear fromModelYear = ModelYear.None, List<string> valueLabels = null)
        {
            var action = new InvokableAction
            {
                Name = name,
                AsyncFunction = GenericPictureAction,
                EnumType = type,
                MinValue = minValue,
                MaxValue = maxValue,
                NumberOfValues = numberOfValues,
                Category = category,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title,
                FromModelYear = fromModelYear,
                ValueLabels = valueLabels
            };

            _invokableActions.Add(action);
        }

        private void AddSetDeviceConfigAction(string name, Type type, string title)
        {
            var action = new InvokableAction
            {
                Name = name,
                AsyncFunction = GenericDeviceConfigAction,
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
                AsyncFunction = GenericSetConfigAction,
                EnumType = type,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title,
                Category = "Config",
                Advanced = true
            };

            _invokableActions.Add(action);
        }

        private void AddLunaAction(string name, Type type, string title, ModelYear fromModelYear = ModelYear.None)
        {
            var action = new InvokableAction
            {
                Name = name,
                AsyncFunction = GenericLunaAction,
                EnumType = type,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title,
                Category = "Config",
                FromModelYear = fromModelYear,
                Advanced = true
            };

            _invokableActions.Add(action);
        }

        private void AddVolumeAction(string name, string title, ModelYear fromModelYear = ModelYear.None)
        {
            var action = new InvokableAction
            {
                Name = name,
                AsyncFunction = GenericVolumeAction,
                MinValue = 0,
                MaxValue = 100,
                Title = title == null ? Utils.FirstCharUpperCase(name) : title,
                Category = "sound",
                FromModelYear = fromModelYear
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

                        Logger.Debug($"LG TV with ip-address {IpAddress} has model name {ModelName}");

                        await _lgTvApi.SubscribeVolume(VolumeChanged);
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
                '3' => ModelYear.Series2023,
                _ => ModelYear.None
            };

            _invokableActions = _invokableActions.Where(a => Year == ModelYear.None || a.FromModelYear == ModelYear.None || Year >= a.FromModelYear).ToList();
        }

        public bool VolumeChanged(dynamic payload)
        {
            if (payload.volume != null)
            {
                PictureSettings.Volume = Utils.ParseDynamicAsInt(payload.volume, PictureSettings.Volume);
            }

            var volumeAction = _invokableActions.FirstOrDefault(a => a.Name == "audioVolume");

            if (volumeAction != null)
            {
                volumeAction.CurrentValue = PictureSettings.Volume;
            }

            PictureSettingsChangedEvent?.Invoke(this, EventArgs.Empty);

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
                else if (CurrentState != PowerState.Screen_Off)
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

        private void DisposeConnection()
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
                        await ExecuteSteps(_lgTvApi, preset, config);
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

        private async Task ExecuteSteps(LgTvApi api, LgPreset preset, LgServiceConfig config)
        {
            LgWebOsMouseService mouse = null;

            foreach (var step in preset.steps)
            {
                var keySpec = step.Split(':');

                var delay = 10;
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
                    await ExecuteActionAsync(action, parameters);

                    executeKey = false;
                }
                if (parameters != null)
                {
                    _serviceManager ??= Program.ServiceProvider.GetRequiredService<ServiceManager>();

                    if (await _serviceManager.HandleExternalServiceAsync(key, parameters))
                    {
                        executeKey = false;
                    }
                }

                if (executeKey)
                {
                    mouse ??= await api.GetMouse();
                    await SendKey(mouse, key);
                    delay = delay <= 10 ? config.DefaultButtonDelay : delay;
                }

                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
        }

        public async Task ExecuteActionAsync(InvokableAction action, string[] parameters)
        {
            var function = action.AsyncFunction;
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

                await function(keyValues);
                return;
            }

            await function(null);
        }

        private async Task SendKey(LgWebOsMouseService mouse, string key)
        {
            key = key.ToUpper();
            if (key.Length >= 1 && int.TryParse(key[0].ToString(), out _))
            {
                key = "_" + key;
            }
            var button = (ButtonType)Enum.Parse(typeof(ButtonType), key);
            await mouse.SendButton(button);
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

        public async Task<bool> PerformActionOnScreenSaver()
        {
            if (!IsPowerOffAllowed())
            {
                return true;
            }

            if (TurnScreenOffOnScreenSaver)
            {
                Logger.Debug("Turning screen off on screen saver");
                await _lgTvApi.TurnScreenOff();

                return true;
            }

            Logger.Debug($"Device {Name} is now powering off due to delayed screensaver task");

            return await PowerOff(true, false);
        }

        public async Task<bool> PerformActionAfterScreenSaver(int powerOnRetries)
        {
            if (TurnScreenOffOnScreenSaver && IsConnected())
            {
                if (TurnScreenOnAfterScreenSaver)
                {
                    try
                    {
                        Logger.Debug("Turning screen on after screen saver");
                        await _lgTvApi.TurnScreenOn();
                    }
                    catch (Exception) { }
                }

                return true;
            }

            return await WakeAndConnectWithRetries(powerOnRetries);
        }

        internal async Task<bool> PowerOff(bool checkHdmi = false, bool reconnect = true)
        {
            if (!await Connected(reconnect) || CurrentState != PowerState.Active)
            {
                return false;
            }

            if (checkHdmi && !IsPowerOffAllowed())
            {
                return true;
            }

            PoweredOffViaApp = true;
            _poweredOffViaAppDateTime = DateTimeOffset.Now;

            try
            {
                await _lgTvApi.TurnOff().WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(true);
            }
            catch (TimeoutException ex)
            {
                Logger.Debug($"Timeout when turning off tv: {ex.Message}");
            }

            return true;
        }

        public bool IsPowerOffAllowed()
        {
            if (CurrentAppId != null && HDMIPortNumber != 0 && !CurrentAppId.EndsWith($".hdmi{HDMIPortNumber}", StringComparison.InvariantCulture))
            {
                Logger.Debug($"[{Name}] PowerOff/ScreenOff is ignored because current app {CurrentAppId} does not match configured HDMI port {HDMIPortNumber}");
                return false;
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
                var wolService = Program.ServiceProvider.GetRequiredService<WolService>();

                result = wolService.SendWol(MacAddress, IpAddress);
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
            return actions.Where(a =>
                                 a.Category == "picture" && (a.EnumType != null && a.EnumType != typeof(PictureMode) || a.MinValue >= 0 && a.MaxValue > 0) ||
                                 a.Name == "audioVolume"
                                 )
                          .ToList();
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

        public async Task SetSvcMenuFlag(bool enabled)
        {
            await CheckConnectionAsync();

            await _lgTvApi.SetSystemSettings("svcMenuFlag", enabled, "other");
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

        private async Task<bool> WakeAction(Dictionary<string, object> _)
        {
            return Wake();
        }

        private async Task<bool> TurnScreenOffAction(Dictionary<string, object> _)
        {
            await CheckConnectionAsync();

            await _lgTvApi.TurnScreenOff();

            return true;
        }

        private async Task<bool> TurnScreenOnAction(Dictionary<string, object> _)
        {
            await CheckConnectionAsync();

            await _lgTvApi.TurnScreenOn();

            return true;
        }

        private async Task<bool> RebootAction(Dictionary<string, object> _)
        {
            await CheckConnectionAsync();

            await _lgTvApi.Reboot();

            return true;
        }

        private async Task<bool> GenericPictureAction(Dictionary<string, object> parameters)
        {
            await CheckConnectionAsync();

            var settingName = parameters["name"].ToString();
            var stringValues = parameters["value"] as string[];
            var category = parameters["category"].ToString();
            object value = stringValues[0];
            if (stringValues.Length > 1)
            {
                value = stringValues.Select(s => int.Parse(s)).ToArray();
            }
            await _lgTvApi.SetSystemSettings(settingName, value, category);

            UpdateCurrentValueOfAction(settingName, value.ToString());

            return true;
        }

        private async Task<bool> GenericDeviceConfigAction(Dictionary<string, object> parameters)
        {
            await CheckConnectionAsync();

            var id = parameters["name"].ToString().Replace("_icon", string.Empty);
            var stringValues = parameters["value"] as string[];
            var value = stringValues[0];

            var description = Utils.GetDescriptionByEnumName<HdmiIcon>(value);

            await _lgTvApi.SetDeviceConfig(id, value, description);

            return true;
        }

        private async Task<bool> GenericSetConfigAction(Dictionary<string, object> parameters)
        {
            await CheckConnectionAsync();

            var key = parameters["name"].ToString();
            var values = parameters["value"] as object[];
            var value = values[0];

            await _lgTvApi.SetConfig(key, value);

            return true;
        }

        private async Task<bool> GenericLunaAction(Dictionary<string, object> parameters)
        {
            await CheckConnectionAsync();

            var key = parameters["name"].ToString();
            var values = parameters["value"] as object[];
            var value = values[0];

            await _lgTvApi.SetTpcOrGsr(key, value?.ToString() == "bool_true");

            return true;
        }

        private async Task<bool> GenericVolumeAction(Dictionary<string, object> parameters)
        {
            await CheckConnectionAsync();

            var values = parameters["value"] as object[];
            var value = values[0];

            var volume = Utils.ParseInt(value.ToString(), 0);

            await _lgTvApi.SetVolume(volume);

            return true;
        }

        public Task SetVolume(int value)
        {
            return _lgTvApi.SetVolume(value);
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

        internal async Task<bool> ExecuteInvokableAction(InvokableActionDto<LgPreset> invokableAction, List<string> parameters)
        {
            var action = _invokableActions.FirstOrDefault(a => a.Name == invokableAction.Name);

            if (action == null)
            {
                return false;
            }

            await ExecuteActionAsync(action, parameters?.ToArray() ?? []);

            return true;
        }
    }
}
