namespace ColorControl.Shared.Contracts.NVIDIA
{
    public class NvHdrSettings
    {
        public enum NV_DISPLAY_OUTPUT_MODE
        {
            NV_DISPLAY_OUTPUT_MODE_SDR = 0,
            NV_DISPLAY_OUTPUT_MODE_HDR10 = 1,
            NV_DISPLAY_OUTPUT_MODE_HDR10PLUS_GAMING = 2
        }

        public NV_DISPLAY_OUTPUT_MODE? OutputMode { get; set; }

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
