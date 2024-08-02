namespace ColorControl.Shared.Contracts.LG;

public class LgServiceConfigDto
{
	public bool PowerOnAfterStartup { get; set; }
	public int PowerOnDelayAfterResume { get; set; }
	public int PowerOnRetries { get; set; }
	public string PreferredMacAddress { get; set; }
	public string DeviceSearchKey { get; set; }
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
}
