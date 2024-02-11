using System.ComponentModel;

namespace MHC2Gen;

public enum SDRTransferFunction
{
    [Description("BT.1886")]
    BT_1886 = 0,
    [Description("Pure Power")]
    PurePower = 1,
    [Description("Piecewise")]
    Piecewise = 2
}

public enum ColorGamut
{
    Native = 0,
    sRGB = 1,
    P3 = 2,
    Rec2020 = 3,
    AdobeRGB = 4
}

public class GenerateProfileCommand
{
    public string Description { get; set; }
    public bool IsHDRProfile { get; set; }
    public RgbPrimaries DevicePrimaries { get; set; } = new RgbPrimaries(RgbPrimaries.sRGB);
    public SDRTransferFunction SDRTransferFunction { get; set; }
    public ColorGamut ColorGamut { get; set; }
    public double MinCLL { get; set; }
    public double MaxCLL { get; set; }
    public double WhiteLuminance { get; set; } = 2000;
    public double BlackLuminance { get; set; } = 0;
    public double SDRMinBrightness { get; set; } = 0;
    public double SDRMaxBrightness { get; set; } = 100;
    public double SDRBrightnessBoost { get; set; } = 0;
    public double Gamma { get; set; } = 2.2;
}
