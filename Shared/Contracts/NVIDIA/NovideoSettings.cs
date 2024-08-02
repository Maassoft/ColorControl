namespace ColorControl.Shared.Contracts.NVIDIA;

public class NovideoSettings
{
	public bool ApplyClamp { get; set; }
	public NovideoColorSpace ColorSpace { get; set; }

	public NovideoSettings()
	{
	}

	public NovideoSettings(NovideoSettings settings)
	{
		settings ??= new NovideoSettings();

		ApplyClamp = settings.ApplyClamp;
		ColorSpace = settings.ColorSpace;
	}

	public override string ToString()
	{
		return ApplyClamp ? $"Clamped at {ColorSpace}" : "Not clamped";
	}
}
