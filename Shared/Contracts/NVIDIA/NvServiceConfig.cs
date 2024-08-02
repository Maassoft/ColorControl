
namespace ColorControl.Shared.Contracts.NVIDIA;

public class NvServiceConfig
{
	public string QuickAccessShortcut { get; set; }
	public bool ShowOverclocking { get; set; }
	public bool ApplyNovideoOnStartup { get; set; }

	public NvServiceConfig()
	{
	}

	public void Update(NvServiceConfig config)
	{
		QuickAccessShortcut = config.QuickAccessShortcut;
		ShowOverclocking = config.ShowOverclocking;
		ApplyNovideoOnStartup = config.ApplyNovideoOnStartup;
	}
}

