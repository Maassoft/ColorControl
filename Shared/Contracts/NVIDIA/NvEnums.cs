using System.ComponentModel;

namespace ColorControl.Shared.Contracts.NVIDIA;

public enum NvDitherState
{
	[Description("Auto")]
	Auto = 0,
	[Description("Enabled")]
	Enabled = 1,
	[Description("Disabled")]
	Disabled = 2
}

public enum NvDitherBits
{
	[Description("6-bit")]
	Bits6 = 0,
	[Description("8-bit")]
	Bits8 = 1,
	[Description("10-bit")]
	Bits10 = 2
}

public enum NvDitherMode
{
	[Description("Spatial Dynamic")]
	SpatialDynamic = 0,
	[Description("Spatial Static")]
	SpatialStatic = 1,
	[Description("Spatial Dynamic 2x2")]
	SpatialDynamic2x2 = 2,
	[Description("Spatial Static 2x2")]
	SpatialStatic2x2 = 3,
	[Description("Temporal")]
	Temporal = 4
}

public enum NovideoColorSpace
{
	sRGB,
	DisplayP3,
	AdobeRGB,
	BT2020
}

public enum NvThermalSensorType
{
	Gpu = 0,
	HotSpot = 1,
	Memory = 3
}