using MHC2Gen;

namespace ColorControl.Shared.Contracts.DisplayInfo;

public class ColorPoint
{
	public double X { get; set; }
	public double Y { get; set; }
}

public class RgbPrimariesDto
{
	public RgbPrimariesDto(RgbPrimaries primaries)
	{
		Red = new ColorPoint { X = primaries.Red.x, Y = primaries.Red.y };
		Green = new ColorPoint { X = primaries.Green.x, Y = primaries.Green.y };
		Blue = new ColorPoint { X = primaries.Blue.x, Y = primaries.Blue.y };
		White = new ColorPoint { X = primaries.White.x, Y = primaries.White.y };
	}

	public RgbPrimariesDto(RgbPrimariesDto primaries)
	{
		Red = new ColorPoint { X = primaries.Red.X, Y = primaries.Red.Y };
		Green = new ColorPoint { X = primaries.Green.X, Y = primaries.Green.Y };
		Blue = new ColorPoint { X = primaries.Blue.X, Y = primaries.Blue.Y };
		White = new ColorPoint { X = primaries.White.X, Y = primaries.White.Y };
	}

	public RgbPrimariesDto() { }

	public ColorPoint Red { get; set; }
	public ColorPoint Green { get; set; }
	public ColorPoint Blue { get; set; }
	public ColorPoint White { get; set; }

	public RgbPrimaries ToInternal()
	{
		return new RgbPrimaries(
			new CIExy { x = Red.X, y = Red.Y },
			new CIExy { x = Green.X, y = Green.Y },
			new CIExy { x = Blue.X, y = Blue.Y },
			new CIExy { x = White.X, y = White.Y }
		);
	}
}

public class DisplayColorInfo
{
	public DisplayPrimariesSource DisplayPrimariesSource { get; set; }
	public string CustomName { get; set; }
	public RgbPrimariesDto RgbPrimaries { get; set; }
	public double MinCLL { get; set; }
	public double MaxCLL { get; set; }
	public double BlackLuminance { get; set; }
	public double WhiteLuminance { get; set; }
}

public class DisplayInfoDto
{
	public bool IsActive { get; set; }
	public string DisplayName { get; set; }
	public string AdapterName { get; set; }
	public string DeviceName { get; set; }
	public string DevicePath { get; set; }
	public string DisplayId { get; set; }
	public bool UsePerUserSettings { get; set; }

	public bool IsAdvancedColorSupported { get; set; }
	public bool IsAdvancedColorEnabled { get; set; }

	public List<DisplayColorInfo> DisplayColors { get; set; } = new();
	public List<ColorProfileDto> ColorProfiles { get; set; } = new();
	public string DefaultSdrColorProfile { get; set; }
	public string DefaultHdrColorProfile { get; set; }

	public bool IsHdrSupportedAndEnabled => IsAdvancedColorSupported && IsAdvancedColorEnabled;

	public override string ToString()
	{
		return $"{DisplayName} on {AdapterName}";
	}

}
