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
		return
			((DigitalVibranceLevel != 50 ? $"Digital vibrance: {DigitalVibranceLevel}" : "") +
			(HueAngle != 0 ? $" Hue: {HueAngle}" : "")).Trim();
	}
}
