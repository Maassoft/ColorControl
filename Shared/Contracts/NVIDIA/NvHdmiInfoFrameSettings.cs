using NvAPIWrapper.Native.Display;

namespace ColorControl.Shared.Contracts.NVIDIA
{
    public class NvHdmiInfoFrameSettings
    {
        public InfoFrameVideoColorFormat? ColorFormat { get; set; }
        public InfoFrameVideoColorimetry? Colorimetry { get; set; }
        public InfoFrameVideoExtendedColorimetry? ExtendedColorimetry { get; set; }
        public InfoFrameVideoRGBQuantization? RGBQuantization { get; set; }
        public InfoFrameVideoYCCQuantization? YCCQuantization { get; set; }
        public InfoFrameVideoITC? ContentMode { get; set; }
        public InfoFrameVideoContentType? ContentType { get; set; }

        public NvHdmiInfoFrameSettings()
        {
            //ColorFormat = InfoFrameVideoColorFormat.Auto;
            //Colorimetry = InfoFrameVideoColorimetry.Auto;
            //ExtendedColorimetry = InfoFrameVideoExtendedColorimetry.Auto;
            //RGBQuantization = InfoFrameVideoRGBQuantization.Auto;
            //YCCQuantization = InfoFrameVideoYCCQuantization.Auto;
            //ContentMode = InfoFrameVideoITC.Auto;
            //ContentType = InfoFrameVideoContentType.Auto;
        }

        public NvHdmiInfoFrameSettings(NvHdmiInfoFrameSettings settings)
        {
            settings ??= new NvHdmiInfoFrameSettings();

            ColorFormat = settings.ColorFormat;
            Colorimetry = settings.Colorimetry;
            ExtendedColorimetry = settings.ExtendedColorimetry;
            RGBQuantization = settings.RGBQuantization;
            YCCQuantization = settings.YCCQuantization;
            ContentMode = settings.ContentMode;
            ContentType = settings.ContentType;
        }

        public override string ToString()
        {
            var value = string.Empty;

            return value;
        }
    }
}
