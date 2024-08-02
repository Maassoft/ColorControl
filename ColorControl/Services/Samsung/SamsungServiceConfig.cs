using ColorControl.Shared.Contracts.Samsung;
using System.Collections.Generic;

namespace ColorControl.Services.Samsung
{
    class SamsungServiceConfig
    {
        public bool PowerOnAfterStartup { get; set; }
        public int PowerOnRetries { get; set; }
        public string PreferredMacAddress { get; set; }
        public string DeviceSearchKey { get; set; }
        public List<SamsungDevice> Devices { get; set; }
        public int ShutdownDelay { get; set; }
        public bool SetSelectedDeviceByPowerOn { get; set; }
        public string QuickAccessShortcut { get; set; }
        public int DefaultButtonDelay { get; set; }
        public bool ShowAdvancedActions { get; set; }

        public SamsungServiceConfig()
        {
            ShutdownDelay = 1000;
            DeviceSearchKey = "Samsung";
            PowerOnRetries = 10;
            DefaultButtonDelay = 500;
            Devices = new List<SamsungDevice>();
        }

        internal void Update(SamsungServiceConfigDto config)
        {
            PowerOnAfterStartup = config.PowerOnAfterStartup;
            PowerOnRetries = config.PowerOnRetries;
            DefaultButtonDelay = config.DefaultButtonDelay;
            PreferredMacAddress = config.PreferredMacAddress;
            DeviceSearchKey = config.DeviceSearchKey;
            ShutdownDelay = config.ShutdownDelay;
            SetSelectedDeviceByPowerOn = config.SetSelectedDeviceByPowerOn;
            QuickAccessShortcut = config.QuickAccessShortcut;
            DefaultButtonDelay = config.DefaultButtonDelay;
            ShowAdvancedActions = config.ShowAdvancedActions;
        }
    }
}
