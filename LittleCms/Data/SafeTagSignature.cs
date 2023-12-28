namespace LittleCms.Data
{
    public static partial class SafeTagSignature
    {
        public static ISafeTagSignature<CIExyYTRIPLE> ChromaticityTag { get; } = new BlittableDataTag<CIExyYTRIPLE>(TagSignature.ChromaticityTag);
        public static ISafeTagSignature<CIEXYZ> MediaWhitePointTag { get; } = new BlittableDataTag<CIEXYZ>(TagSignature.MediaWhitePointTag);
        public static ISafeTagSignature<CIEXYZ> BlueColorantTag { get; } = new BlittableDataTag<CIEXYZ>(TagSignature.BlueColorantTag);
        public static ISafeTagSignature<CIEXYZ> GreenColorantTag { get; } = new BlittableDataTag<CIEXYZ>(TagSignature.GreenColorantTag);
        public static ISafeTagSignature<CIEXYZ> LuminanceTag { get; } = new BlittableDataTag<CIEXYZ>(TagSignature.LuminanceTag);
        public static ISafeTagSignature<CIEXYZ> MediaBlackPointTag { get; } = new BlittableDataTag<CIEXYZ>(TagSignature.MediaBlackPointTag);
        public static ISafeTagSignature<CIEXYZ> RedColorantTag { get; } = new BlittableDataTag<CIEXYZ>(TagSignature.RedColorantTag);

        public static ISafeTagSignature<double[,]> ChromaticAdaptationTag { get; } = new ChromaticAdaptationTagSignature();

        public static ISafeTagSignature<ICCData> Mhc2Tag { get; } = new IccDataTagSignature(TagSignature.MHC2);

        public static ISafeTagSignature<ICCData> DataTag { get; } = new IccDataTagSignature(TagSignature.DataTag);
        public static ISafeTagSignature<ICCData> Ps2CRD0Tag { get; } = new IccDataTagSignature(TagSignature.Ps2CRD0Tag);
        public static ISafeTagSignature<ICCData> Ps2CRD1Tag { get; } = new IccDataTagSignature(TagSignature.Ps2CRD1Tag);
        public static ISafeTagSignature<ICCData> Ps2CRD2Tag { get; } = new IccDataTagSignature(TagSignature.Ps2CRD2Tag);
        public static ISafeTagSignature<ICCData> Ps2CRD3Tag { get; } = new IccDataTagSignature(TagSignature.Ps2CRD3Tag);
        public static ISafeTagSignature<ICCData> Ps2CSATag { get; } = new IccDataTagSignature(TagSignature.Ps2CSATag);
        public static ISafeTagSignature<ICCData> Ps2RenderingIntentTag { get; } = new IccDataTagSignature(TagSignature.Ps2RenderingIntentTag);

        public static ISafeTagSignature<ToneCurve> BlueTRCTag { get; } = new ToneCurveTagSignature(TagSignature.BlueTRCTag);
        public static ISafeTagSignature<ToneCurve> GrayTRCTag { get; } = new ToneCurveTagSignature(TagSignature.GrayTRCTag);
        public static ISafeTagSignature<ToneCurve> GreenTRCTag { get; } = new ToneCurveTagSignature(TagSignature.GreenTRCTag);
        public static ISafeTagSignature<ToneCurve> RedTRCTag { get; } = new ToneCurveTagSignature(TagSignature.RedTRCTag);

        public static ISafeTagSignature<RgbToneCurve> VcgtTag { get; } = new VideoCardGammaTableTagSignature();

        public static ISafeTagSignature<MLU> CharTargetTag { get; } = new MluTagSignature(TagSignature.CharTargetTag);
        public static ISafeTagSignature<MLU> CopyrightTag { get; } = new MluTagSignature(TagSignature.CopyrightTag);
        public static ISafeTagSignature<MLU> DeviceMfgDescTag { get; } = new MluTagSignature(TagSignature.DeviceMfgDescTag);
        public static ISafeTagSignature<MLU> DeviceModelDescTag { get; } = new MluTagSignature(TagSignature.DeviceModelDescTag);
        public static ISafeTagSignature<MLU> ProfileDescriptionTag { get; } = new MluTagSignature(TagSignature.ProfileDescriptionTag);
        public static ISafeTagSignature<MLU> ScreeningDescTag { get; } = new MluTagSignature(TagSignature.ScreeningDescTag);
        public static ISafeTagSignature<MLU> ViewingCondDescTag { get; } = new MluTagSignature(TagSignature.ViewingCondDescTag);

    }
}
