using System.ComponentModel;

namespace MHC2Gen;

public enum SaveOption
{
    Install,
    [Description("Install and associate to display")]
    InstallAndAssociate,
    [Description("Install and set as display default")]
    InstallAndSetAsDefault,
    [Description("Save to file")]
    SaveToFile
}

public enum DisplayPrimariesSource
{
    Custom,
    EDID,
    Windows,
    ColorProfile,
}

public enum SDRTransferFunction
{
    [Description("BT.1886")]
    BT_1886 = 0,
    [Description("Pure Power")]
    PurePower = 1,
    [Description("Piecewise")]
    Piecewise = 2,
    [Description("Tone Mapped Piecewise")]
    ToneMappedPiecewise = 3
}

public enum ColorGamut
{
    Native = 0,
    sRGB = 1,
    P3 = 2,
    Rec2020 = 3,
    AdobeRGB = 4
}
