using ColorControl.Shared.Contracts.LG;
using System.Collections.Generic;

namespace ColorControl.Services.LG
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
		public int GameBarTop { get; set; }
		public int GameBarLeft { get; set; }
		public int ShutdownDelay { get; set; }
		public bool SetSelectedDeviceByPowerOn { get; set; }
		public string QuickAccessShortcut { get; set; }
		public int DefaultButtonDelay { get; set; }
		public int GameBarShowingTime { get; set; }

		public LgServiceConfig()
		{
			PowerOnDelayAfterResume = 5000;
			ShutdownDelay = 1000;
			DeviceSearchKey = "[LG]";
			PowerOnRetries = 10;
			Devices = new List<LgDevice>();
			GameBarTop = 100;
			GameBarLeft = 100;
			DefaultButtonDelay = 200;
			GameBarShowingTime = 5000;
		}

		internal void Update(LgServiceConfigDto config)
		{
			PowerOnAfterStartup = config.PowerOnAfterStartup;
			ShutdownDelay = config.ShutdownDelay;
			DeviceSearchKey = config.DeviceSearchKey;
			PowerOnRetries = config.PowerOnRetries;
			DefaultButtonDelay = config.DefaultButtonDelay;
			GameBarShowingTime = config.GameBarShowingTime;
			ShowAdvancedActions = config.ShowAdvancedActions;
			QuickAccessShortcut = config.QuickAccessShortcut;
			SetSelectedDeviceByPowerOn = config.SetSelectedDeviceByPowerOn;
		}
	}
}
