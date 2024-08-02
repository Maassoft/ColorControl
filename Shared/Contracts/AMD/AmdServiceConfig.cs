
namespace ColorControl.Shared.Contracts.AMD;

public class AmdServiceConfig
{
	public string QuickAccessShortcut { get; set; }

	public AmdServiceConfig()
	{
	}

	public void Update(AmdServiceConfig config)
	{
		QuickAccessShortcut = config.QuickAccessShortcut;
	}
}

