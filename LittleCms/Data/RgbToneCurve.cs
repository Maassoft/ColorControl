namespace LittleCms.Data
{
    public record RgbToneCurve(ToneCurve Red, ToneCurve Green, ToneCurve Blue)
    {
        public ToneCurve[] ToArray() => new[] { Red, Green, Blue };
    }
}
