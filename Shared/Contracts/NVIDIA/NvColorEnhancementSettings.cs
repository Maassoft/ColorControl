namespace ColorControl.Shared.Contracts.NVIDIA;

public class NvColorEnhancementSettings
{
	public int DigitalVibranceLevel { get; set; } = 50;
	public int HueAngle { get; set; }

	public bool ApplyDesktopColor { get; set; }
	public int Brightness { get; set; } = 50;
	public int Contrast { get; set; } = 50;
	public double Gamma { get; set; } = 1.0;

	public NvColorEnhancementSettings()
	{
	}

	public NvColorEnhancementSettings(NvColorEnhancementSettings settings)
	{
		settings ??= new NvColorEnhancementSettings();

		DigitalVibranceLevel = settings.DigitalVibranceLevel;
		HueAngle = settings.HueAngle;
		ApplyDesktopColor = settings.ApplyDesktopColor;
		Brightness = settings.Brightness;
		Contrast = settings.Contrast;
		Gamma = settings.Gamma;
	}

	public bool DesktopColorDiffers(NvColorEnhancementSettings other)
	{
		return Brightness != other.Brightness || Contrast != other.Contrast || Gamma != other.Gamma;
	}

	public override string ToString()
	{
		return
			((DigitalVibranceLevel != 50 ? $"Digital vibrance: {DigitalVibranceLevel}" : "") +
			(HueAngle != 0 ? $" Hue: {HueAngle}" : "") +
			(ApplyDesktopColor ? $" Brightness: {Brightness} Contrast: {Contrast} Gamma: {Gamma:0.00}" : "")).Trim();
	}
}
