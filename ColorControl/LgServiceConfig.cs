namespace ColorControl
{
    class LgServiceConfig
    {
        public bool PowerOnAfterStartup { get; set; }

        public bool PowerOnAfterResume { get; set; }

        public bool PowerOffOnShutdown { get; set; }

        public bool PowerOffOnStandby { get; set; }

        public bool PowerSwitchOnScreenSaver { get; set; }

        public int PowerOnDelayAfterResume { get; set; }

        public int PowerOnRetries { get; set; }

        public string PreferredMacAddress { get; set; }

        public string DeviceSearchKey { get; set; }

        public bool UseOldNpcapWol { get; set; }

        public LgServiceConfig()
        {
            PowerOnDelayAfterResume = 5000;
            DeviceSearchKey = "[LG]";
            PowerOnRetries = 10;
        }
    }
}
