namespace ColorControl.Shared.Contracts;

public class Rational
{
    public uint Numerator { get; }
    public uint Denominator { get; }

    public Rational(uint numerator, uint denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public override string ToString()
    {
        var value = Numerator / (double)Denominator;

        return $"{value:0.000}";
    }
}
