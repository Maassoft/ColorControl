namespace ColorControl.Shared.Native;

public static class CmsFunctions
{

    private const double PQ_M1 = 0.1593017578125;
    private const double PQ_M2 = 78.84375;
    private const double PQ_C1 = 0.8359375;
    private const double PQ_C2 = 18.8515625;
    private const double PQ_C3 = 18.6875;

    public static double PqEotf(double signalValue)
    {
        var signalValuePow = Math.Pow(signalValue, 1D / PQ_M2);

        var value1 = Math.Max(signalValuePow - PQ_C1, 0);
        var value2 = PQ_C2 - PQ_C3 * signalValuePow;

        var luminance = 10000D * Math.Pow(value1 / value2, 1D / PQ_M1);

        return luminance;
    }

    public static double InvPqEotf(double luminance)
    {
        var normalizedLuminance = luminance / 10000D;

        var luminancePow = Math.Pow(normalizedLuminance, PQ_M1);

        var value1 = PQ_C1 + PQ_C2 * luminancePow;
        var value2 = 1 + PQ_C3 * luminancePow;

        var signalValue = Math.Pow(value1 / value2, PQ_M2);

        return signalValue;
    }

    public static double SrgbInvEotf(double luminance, double whiteLuminance, double blackLuminance, double gamma = 2.4)
    {
        //const double X1 = 0.0404482362771082;
        const double X2 = 0.00313066844250063;

        var x = (luminance - blackLuminance) / (whiteLuminance - blackLuminance);

        if (x > 1)
        {
            x = 1;
        }
        else if (x < 0)
        {
            x = 0;
        }
        else if (x <= X2)
        {
            x *= 12.92;
        }
        else
        {
            x = 1.055 * Math.Pow(x, 1 / gamma) - 0.055;
        }

        return x;
    }

    public static (double, double) SrgbAcm(int signalValue, double whiteLuminance, double blackLuminance = 0, double gamma = 2.2)
    {
        var b = signalValue / 1023;
        var c = PqEotf(b);
        var d = SrgbInvEotf(c, whiteLuminance, blackLuminance);
        var e = blackLuminance + (whiteLuminance - blackLuminance) * Math.Pow(d, gamma);
        var f = InvPqEotf(Math.Max(0, e));
        var x = f + Math.Min(1, c / whiteLuminance) * (b - f);

        return (b, x);
    }

    public static double GammaRgb(double signalValue, double gamma = 2.4)
    {
        return signalValue <= 0.0031308 ? signalValue * 12.92 : 1.055 * Math.Pow(signalValue, 1.0 / gamma) - 0.055;
    }

    public static double RgbToLinear(double signalValue, double gamma = 2.4)
    {
        return signalValue <= 0.04045 ? signalValue / 12.92 : Math.Pow((signalValue + 0.055) / 1.055, gamma);
    }

}
