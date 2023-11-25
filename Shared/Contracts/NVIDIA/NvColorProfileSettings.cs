namespace ColorControl.Shared.Contracts.NVIDIA
{
    public class NvColorProfileSettings
    {
        public string ProfileName { get; set; }

        public NvColorProfileSettings()
        {
        }

        public NvColorProfileSettings(NvColorProfileSettings settings)
        {
            settings ??= new NvColorProfileSettings();

            ProfileName = settings.ProfileName;
        }

        public override string ToString()
        {
            var value = string.Empty;

            return value;
        }
    }
}
