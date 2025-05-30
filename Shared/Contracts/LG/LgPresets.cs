namespace ColorControl.Shared.Contracts.LG;

public static class LgPresets
{
    public static readonly LgPreset InStartPreset = new("InStart", "com.webos.app.factorywin", ["0", "4", "1", "3"], new { id = "executeFactory", irKey = "inStart" });
    public static readonly LgPreset EzAdjustPreset = new("EzAdjust", "com.webos.app.factorywin", ["0", "4", "1", "3"], new { id = "executeFactory", irKey = "ezAdjust" });
    public static readonly LgPreset SoftwareUpdatePreset = new("Software Update", "com.webos.app.softwareupdate", null, new { mode = "user", flagUpdate = true });
}
