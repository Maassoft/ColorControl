namespace ColorControl.Shared.Contracts.Game;

public class GameServiceConfig
{
    public string QuickAccessShortcut { get; set; }
    public bool ApplyExternallyLaunched { get; set; }

    public GameServiceConfig()
    {
    }

    public void Update(GameServiceConfig config)
    {
        QuickAccessShortcut = config.QuickAccessShortcut;
        ApplyExternallyLaunched = config.ApplyExternallyLaunched;
    }
}

