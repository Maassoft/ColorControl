namespace ColorControl.Shared.Contracts.LG;

public class LgDeviceOptions
{
    public bool PowerOnAfterStartup { get; set; }
    public bool PowerOnAfterResume { get; set; }
    public bool PowerOffOnShutdown { get; set; }
    public bool PowerOffOnStandby { get; set; }
    public bool PowerOffOnScreenSaver { get; set; }
    public int ScreenSaverMinimalDuration { get; set; }
    public bool TurnScreenOffOnScreenSaver { get; set; }
    public bool HandleManualScreenSaver { get; set; }
    public bool PowerOnAfterScreenSaver { get; set; }
    public bool TurnScreenOnAfterScreenSaver { get; set; }
    public bool PowerOnAfterManualPowerOff { get; set; }
    public bool PowerOnByWindows { get; set; }
    public bool PowerOffByWindows { get; set; }
    public bool PowerOnAfterSessionUnlock { get; set; }
    public bool PowerOffAfterSessionLock { get; set; }
    public bool TriggersEnabled { get; set; }
    public int HDMIPortNumber { get; set; }

    public bool UseSecureConnection { get; set; } = true;
}

