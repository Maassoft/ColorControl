using System.Collections.Generic;

namespace ColorControl
{
    class LgServiceConfig
    {
        public bool PowerOnAfterStartup { get; set; }
        public int PowerOnDelayAfterResume { get; set; }
        public int PowerOnRetries { get; set; }
        public string PreferredMacAddress { get; set; }
        public string DeviceSearchKey { get; set; }
        public List<LgDevice> Devices { get; set; }
        public bool ShowRemoteControl { get; set; }
        public bool ShowAdvancedActions { get; set; }
        public string GameBarShortcut { get; set; }

        public LgServiceConfig()
        {
            PowerOnDelayAfterResume = 5000;
            DeviceSearchKey = "[LG]";
            PowerOnRetries = 10;
            Devices = new List<LgDevice>();
        }
    }
}
