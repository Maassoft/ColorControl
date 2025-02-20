using System;

namespace MHC2Gen;

public class GenerateProfileCommand
{
    public string Description { get; set; } = "";
    public bool HasExtraInfo { get; set; }
    public bool IsHDRProfile { get; set; }
    public RgbPrimaries DevicePrimaries { get; set; } = new RgbPrimaries(RgbPrimaries.sRGB);
    public SDRTransferFunction SDRTransferFunction { get; set; }
    public ColorGamut ColorGamut { get; set; }
    public double MinCLL { get; set; }
    public double MaxCLL { get; set; }
    public double WhiteLuminance { get; set; } = 2000;
    public double BlackLuminance { get; set; }
    public double SDRMinBrightness { get; set; }
    public double SDRMaxBrightness { get; set; } = 100;
    public double SDRBrightnessBoost { get; set; }
    public double ShadowDetailBoost { get; set; }
    public double Gamma { get; set; } = 2.2;

    public double ToneMappingFromLuminance { get; set; }
    public double ToneMappingToLuminance { get; set; }

    public double HdrGammaMultiplier { get; set; }
    public double HdrBrightnessMultiplier { get; set; }
}
