using System.Text.Json.Serialization;

namespace ColorControl.Shared.Contracts.NVIDIA
{
    public enum NvOutputMode
    {
        Default = 0,
        Hdr10 = 1,
        Hdr10Plus = 2
    }

    public class NvHdrSettings
    {
        public enum NV_DISPLAY_OUTPUT_MODE
        {
            NV_DISPLAY_OUTPUT_MODE_SDR = 0,
            NV_DISPLAY_OUTPUT_MODE_HDR10 = 1,
            NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING = 2
        }

        public NV_DISPLAY_OUTPUT_MODE? OutputMode { get; set; }

        [JsonIgnore]
        public NvOutputMode DisplayOutputMode
        {
            get => OutputMode switch
            {
                NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10 => NvOutputMode.Hdr10,
                NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING => NvOutputMode.Hdr10Plus,
                _ => NvOutputMode.Default
            };
            set
            {
                OutputMode = value switch
                {
                    NvOutputMode.Hdr10 => NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10,
                    NvOutputMode.Hdr10Plus => NV_DISPLAY_OUTPUT_MODE.NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING,
                    _ => null
                };
            }
        }

        public NvHdrSettings()
        {
        }

        public NvHdrSettings(NvHdrSettings settings)
        {
            settings ??= new NvHdrSettings();

            OutputMode = settings.OutputMode;
        }

        public override string ToString()
        {
            var value = string.Empty;

            return value;
        }
    }
}
