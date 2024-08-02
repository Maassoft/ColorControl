namespace ColorControl.Shared.Contracts.NVIDIA;

public static class NvSettingConstants
{
    public const uint DRS_GSYNC_APPLICATION_MODE = 294973784;
    public const uint DRS_VSYNC_CONTROL = 11041231;
    //public const uint DRS_ULTRA_LOW_LATENCY      = 0x10835000;
    public const uint DRS_PRERENDERED_FRAMES = 8102046;
    public const uint DRS_FRAME_RATE_LIMITER_V3 = 277041154;
    public const uint DRS_RTX_HDR_PEAK_BRIGHTNESS = 0x00DD48FC;
    public const uint DRS_RTX_HDR_MIDDLE_GREY = 0x00DD48FD;
    public const uint DRS_RTX_HDR_CONTRAST = 0x00DD48FE;
    public const uint DRS_RTX_HDR_SATURATION = 0x00DD48FF;
    public const uint DRS_RTX_DYNAMIC_VIBRANCE_INTENSITY = 0x00ABAB22;
    public const uint DRS_RTX_DYNAMIC_VIBRANCE_SATURATION_BOOST = 0x00ABAB13;

    public const uint DRS_ANISOTROPIC_FILTERING_SETTING = 0x101E61A9;
    public const uint DRS_ANISOTROPIC_FILTER_OPTIMIZATION = 0x0084CD70;
    public const uint DRS_ANISOTROPIC_FILTER_SAMPLE_OPTIMIZATION = 0x00E73211;
    public const uint DRS_TEXTURE_FILTERING_QUALITY = 0x00CE2691;
    public const uint DRS_TEXTURE_FILTERING_NEGATIVE_LOD_BIAS = 0x0019BB68;

    public const uint DRS_RBAR_FEATURE = 0X000F00BA;
    public const uint DRS_RBAR_OPTIONS = 0X000F00BB;
    public const uint DRS_RBAR_SIZE_LIMIT = 0X000F00FF;

    public static uint[] RangeDriverSettings = [DRS_FRAME_RATE_LIMITER_V3, DRS_RTX_HDR_PEAK_BRIGHTNESS, DRS_RTX_HDR_MIDDLE_GREY, DRS_RTX_HDR_CONTRAST, DRS_RTX_HDR_SATURATION, DRS_RTX_DYNAMIC_VIBRANCE_INTENSITY, DRS_RTX_DYNAMIC_VIBRANCE_SATURATION_BOOST];

    public const uint UnsetDwordValue = uint.MaxValue - 1;
}