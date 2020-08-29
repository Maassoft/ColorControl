namespace ColorControl
{
    class LgServiceConfig
    {
        public bool PowerOnAfterStartup { get; set; }

        public bool PowerOnAfterResume { get; set; }

        public bool PowerOffOnShutdown { get; set; }

        public bool PowerOffOnStandby { get; set; }

        public int PowerOnDelayAfterResume { get; set; }

        public LgServiceConfig()
        {
            PowerOnDelayAfterResume = 5000;
        }
    }
}
