using MHC2Gen;

namespace ColorControl.Shared.Contracts.DisplayInfo;

public class ColorProfileDto
{
	public string Name { get; set; }
	public string Description { get; set; }
	public bool HasExtraInfo { get; set; }
	public bool IsHDRProfile { get; set; }
	public RgbPrimariesDto DevicePrimaries { get; set; } = new RgbPrimariesDto(RgbPrimaries.sRGB);
	public SDRTransferFunction SDRTransferFunction { get; set; }
	public ColorGamut ColorGamut { get; set; }
	public double MinCLL { get; set; }
	public double MaxCLL { get; set; }
	public double WhiteLuminance { get; set; }
	public double BlackLuminance { get; set; }
	public double SDRMinBrightness { get; set; }
	public double SDRMaxBrightness { get; set; }
	public double SDRBrightnessBoost { get; set; }
	public double ShadowDetailBoost { get; set; }
	public double Gamma { get; set; } = 2.2;

	public void UpdatePrimariesAndLuminance(DisplayColorInfo displayColorInfo)
	{
		BlackLuminance = displayColorInfo.BlackLuminance;
		WhiteLuminance = displayColorInfo.WhiteLuminance;
		MinCLL = displayColorInfo.MinCLL;
		MaxCLL = displayColorInfo.MaxCLL;
		DevicePrimaries = new RgbPrimariesDto(displayColorInfo.RgbPrimaries);
	}

	public override string ToString()
	{
		return Name == "None" ? Name : $"{(IsHDRProfile ? "HDR" : "SDR")}: {Name}";
	}

	public static ColorProfileDto CreateDefault(bool isHdrProfile)
	{
		return new ColorProfileDto
		{
			IsHDRProfile = isHdrProfile,
			HasExtraInfo = true,
			MinCLL = 0,
			MaxCLL = 1000,
			BlackLuminance = 0,
			WhiteLuminance = 1000,
			SDRMinBrightness = 0,
			SDRMaxBrightness = 100,
			SDRTransferFunction = isHdrProfile ? SDRTransferFunction.Piecewise : SDRTransferFunction.PurePower
		};
	}
}
