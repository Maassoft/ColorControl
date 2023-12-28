namespace ColorControl.Shared.Contracts.NVIDIA;

public class NvColorEnhancementSettings
{
    public int DigitalVibranceLevel { get; set; } = 50;
    public int HueAngle { get; set; }

    public NvColorEnhancementSettings()
    {
    }

    public NvColorEnhancementSettings(NvColorEnhancementSettings settings)
    {
        settings ??= new NvColorEnhancementSettings();

        DigitalVibranceLevel = settings.DigitalVibranceLevel;
        HueAngle = settings.HueAngle;
    }

    public override string ToString()
    {
        var value = string.Empty;

        return value;
    }
}
