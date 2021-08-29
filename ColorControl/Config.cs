namespace ColorControl
{
    public class Config
    {
        public bool StartMinimized { get; set; }

        public bool MinimizeOnClose { get; set; }

        public bool MinimizeToTray { get; set; }

        public bool CheckForUpdates { get; set; }

        public int DisplaySettingsDelay { get; set; }

        public string ScreenSaverShortcut { get; set; }

        public int FormWidth { get; set; }
        public int FormHeight { get; set; }

        public int NvPresetId_ApplyOnStartup { get; set; }

        public int AmdPresetId_ApplyOnStartup { get; set; }

        public bool FixChromeFonts { get; set; }

        public Config()
        {
            DisplaySettingsDelay = 1000;
            ScreenSaverShortcut = string.Empty;
            FormWidth = 900;
            FormHeight = 600;
            MinimizeToTray = true;
            CheckForUpdates = true;
            FixChromeFonts = false;
        }
    }
}
