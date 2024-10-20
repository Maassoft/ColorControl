namespace ColorControl.Shared.Contracts.Samsung;


public static class SamsungFactoryMenuPresets
{
    public static SamsungPreset Preset1 = new("ServiceMenuStep1", null, ["KEY_INFO", "KEY_FACTORY:2000"]);
    public static SamsungPreset PresetAdvanced = new("ServiceMenuAdvanced", null, ["KEY_UP", "KEY_0", "KEY_0", "KEY_9", "KEY_8"]);
    public static SamsungPreset PresetAdvancedMonitor = new("ServiceMenuAdvancedMonitor", null, ["KEY_UP", "KEY_0", "KEY_0", "KEY_3", "KEY_8"]);
    public static SamsungPreset Exit = new("ServiceMenuStep3", null, ["KEY_FACTORY:1500", "KEY_FACTORY"]);
    public static SamsungPreset KeyFactory = new("ServiceMenuKeyFactory", null, ["KEY_FACTORY"]);
}

public static class SamsungHospitalityMenuPresets
{
    public static SamsungPreset Preset1 = new("ServiceMenuStep1", null, ["KEY_MUTE:600", "KEY_1:600", "KEY_1:600", "KEY_9:600", "KEY_ENTER:2000"]);
    public static SamsungPreset Preset2 = new("ServiceMenuStep2", null, ["KEY_1", "KEY_2", "KEY_3", "KEY_4", "KEY_UP"]);
    public static SamsungPreset Preset3 = new("ServiceMenuStep3", null, ["KEY_VOLUP:5100:Press", "KEY_VOLUP:500:Release"]);
}

public static class SamsungGenericPresets
{
    public static SamsungPreset Reboot = new("ServiceMenuStep4", null, ["KEY_POWER:5000", "WOL"]);
}
