using ColorControl.Shared.Common;
using static ColorControl.Shared.Native.CCD;

namespace ColorControl.Shared.Contracts;

public class DisplayConfig
{
    public bool ApplyResolution { get; set; }
    public VirtualResolution Resolution { get; set; }
    public bool ApplyRefreshRate { get; set; }
    public Rational RefreshRate { get; set; }
    public DisplayConfigScaling Scaling { get; set; }
    public DisplayConfigRotation Rotation { get; set; }
    public bool? IsPrimary { get; set; }

    public DisplayConfig()
    {
        RefreshRate = new Rational(60000, 1000);
        Resolution = new VirtualResolution(0, 0);
        Scaling = DisplayConfigScaling.Identity;
        Rotation = DisplayConfigRotation.Identity;
    }

    public DisplayConfig(DisplayConfig displayConfig)
    {
        ApplyResolution = displayConfig.ApplyResolution;
        Resolution = new VirtualResolution(displayConfig.Resolution);
        ApplyRefreshRate = displayConfig.ApplyRefreshRate;
        RefreshRate = new Rational(displayConfig.RefreshRate.Numerator, displayConfig.RefreshRate.Denominator);
        Scaling = displayConfig.Scaling;
        Rotation = displayConfig.Rotation;
    }

    public string GetResolutionDesc()
    {
        var text = Resolution.ToString();

        if (Rotation is not DisplayConfigRotation.Identity or DisplayConfigRotation.Zero)
        {
            text += $" - {Rotation.GetDescription()}";
        }

        if (Scaling is not DisplayConfigScaling.Identity or DisplayConfigScaling.Zero)
        {
            text += $" - {Scaling.GetDescription()}";
        }

        return text;
    }
}
