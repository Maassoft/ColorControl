using Newtonsoft.Json;

namespace ColorControl.Shared.Contracts;

public class Rational
{
    public uint Numerator { get; set; }
    public uint Denominator { get; set; }

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

    [JsonIgnore]
    public uint MilliValue => (uint)(Numerator / (double)Denominator * 1000);

    public bool Equals(Rational other)
    {
        return MilliValue == other.MilliValue;
    }
}
