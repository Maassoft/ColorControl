namespace ColorControl.Shared.Contracts.Samsung;

public static class SamsungFactoryMenuPresets
{
    public static readonly SamsungPreset Preset1 = new("ServiceMenuStep1", null, ["KEY_INFO:600", "KEY_FACTORY:3000"]);
    public static readonly SamsungPreset PresetAdvanced = new("ServiceMenuAdvanced", null, ["KEY_0:800", "KEY_0:800", "KEY_9:800", "KEY_8:800"]);
    public static readonly SamsungPreset PresetAdvancedMonitor = new("ServiceMenuAdvancedMonitor", null, ["KEY_0:800", "KEY_0:800", "KEY_3:800", "KEY_8:800"]);
    public static readonly SamsungPreset Exit = new("ServiceMenuStep3", null, ["KEY_FACTORY:2500", "KEY_FACTORY"]);
    public static readonly SamsungPreset KeyFactory = new("ServiceMenuKeyFactory", null, ["KEY_FACTORY"]);
}

public static class SamsungHospitalityMenuPresets
{
    public static readonly SamsungPreset Preset1 = new("ServiceMenuStep1", null, ["KEY_MUTE:800", "KEY_1:800", "KEY_1:800", "KEY_9:800", "KEY_ENTER:3000"]);
    public static readonly SamsungPreset Preset2 = new("ServiceMenuStep2", null, ["KEY_1:800", "KEY_2:800", "KEY_3:800", "KEY_4:800", "KEY_UP"]);
    public static readonly SamsungPreset Preset3 = new("ServiceMenuStep3", null, ["KEY_VOLUP:5100:Press", "KEY_VOLUP:500:Release"]);
}

public static class SamsungGenericPresets
{
    public static readonly SamsungPreset Reboot = new("ServiceMenuStep4", null, ["KEY_POWER:5000", "WOL"]);
}
