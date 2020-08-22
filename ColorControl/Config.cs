namespace ColorControl
{
    class Config
    {
        public bool StartMinimized { get; set; }

        public int DisplaySettingsDelay { get; set; }

        public Config()
        {
            DisplaySettingsDelay = 1000;
        }
    }
}
