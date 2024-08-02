
namespace ColorControl.Shared.Contracts
{
    public enum ElevationMethod
    {
        None = 0,
        RunAsAdmin = 1,
        UseService = 2,
        UseElevatedProcess = 3
    }

    public enum UiType
    {
        WinForms = 0,
        WebDefaultBrowser = 1,
        WebEmbedded = 2
    }

    public class Config
    {
        public bool AutoStart { get; set; }
        public bool StartMinimized { get; set; }
        public bool MinimizeOnClose { get; set; }
        public bool MinimizeToTray { get; set; }
        public bool CheckForUpdates { get; set; }
        public bool AutoInstallUpdates { get; set; }
        public bool UseDarkMode { get; set; }
        public int DisplaySettingsDelay { get; set; }
        public string ScreenSaverShortcut { get; set; }
        public int FormWidth { get; set; }
        public int FormHeight { get; set; }
        public double XFormWidth { get; set; }
        public double XFormHeight { get; set; }
        public int NvPresetId_ApplyOnStartup { get; set; }
        public int AmdPresetId_ApplyOnStartup { get; set; }
        public bool FixChromeFonts { get; set; }
        public bool UseGdiScaling { get; set; }
        public ListViewSortState NvPresetsSortState { get; set; }
        public ListViewSortState AmdPresetsSortState { get; set; }
        public ListViewSortState LgPresetsSortState { get; set; }
        public ListViewSortState SamsungPresetsSortState { get; set; }
        public ListViewSortState GamePresetsSortState { get; set; }
        public string AmdQuickAccessShortcut { get; set; }
        public string LgQuickAccessShortcut { get; set; }
        public bool UseDedicatedElevatedProcess { get; set; }
        public ElevationMethod ElevationMethod { get; set; }
        public bool ElevationMethodAsked { get; set; }
        public int ProcessMonitorPollingInterval { get; set; }
        public string LogLevel { get; set; }
        public List<Module> Modules { get; set; }
        public bool UseRawInput { get; set; }
        public bool SetMinTmlAndMaxTml { get; set; }
        public bool DisableErrorPopupWhenApplyingPreset { get; set; }
        public UiType UiType { get; set; }
        public int UiPort { get; set; }

        public Config()
        {
            AutoStart = false;
            LogLevel = "DEBUG";
            DisplaySettingsDelay = 1000;
            ScreenSaverShortcut = string.Empty;
            FormWidth = 900;
            FormHeight = 600;
            XFormWidth = 1280;
            XFormHeight = 720;
            MinimizeToTray = true;
            CheckForUpdates = true;
            FixChromeFonts = false;
            UseGdiScaling = true;
            UseDedicatedElevatedProcess = false;
            ElevationMethod = ElevationMethod.None;
            ElevationMethodAsked = false;
            ProcessMonitorPollingInterval = 1000;
            UiType = UiType.WinForms;
            UiPort = 5000;
            LgPresetsSortState = new ListViewSortState();
            NvPresetsSortState = new ListViewSortState();
            AmdPresetsSortState = new ListViewSortState();
            SamsungPresetsSortState = new ListViewSortState();
            GamePresetsSortState = new ListViewSortState();
            Modules = new List<Module>();
        }

        public Config(Config config) : this()
        {
            Update(config);
        }

        public void Update(Config config)
        {
            AutoStart = config.AutoStart;
            StartMinimized = config.StartMinimized;
            MinimizeOnClose = config.MinimizeOnClose;
            MinimizeToTray = config.MinimizeToTray;
            CheckForUpdates = config.CheckForUpdates;
            AutoInstallUpdates = config.AutoInstallUpdates;
            UseDarkMode = config.UseDarkMode;
            ScreenSaverShortcut = config.ScreenSaverShortcut;
            FixChromeFonts = config.FixChromeFonts;
            UseGdiScaling = config.UseGdiScaling;
            ProcessMonitorPollingInterval = config.ProcessMonitorPollingInterval;
            UseRawInput = config.UseRawInput;
            SetMinTmlAndMaxTml = config.SetMinTmlAndMaxTml;
            DisableErrorPopupWhenApplyingPreset = config.DisableErrorPopupWhenApplyingPreset;
            UiType = config.UiType;
            UiPort = config.UiPort;
        }

        public bool HasModule(string name)
        {
            return Modules?.Any(m => m.DisplayName == name) == true;
        }
    }
}
