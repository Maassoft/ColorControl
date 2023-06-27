using nspector.Native.NVAPI2;

namespace nspector.Common.Meta
{
    public class SettingMeta
    {
        public NVDRS_SETTING_TYPE? SettingType { get; set; }

        public string GroupName { get; set; }

        public string SettingName { get; set; }

        public string DefaultStringValue { get; set; }

        public uint DefaultDwordValue { get; set; }

        public byte[] DefaultBinaryValue { get; set; }

        public bool IsApiExposed { get; set; }

        public bool IsSettingHidden { get; set; }

        public string Description { get; set; }

        public List<SettingValue<string>> StringValues { get; set; }

        public List<SettingValue<uint>> DwordValues { get; set; }

        public List<SettingValue<byte[]>> BinaryValues { get; set; }

        public const uint UnsetDwordValue = uint.MaxValue - 1;

        public string? ToFriendlyName(string? textValue = null, uint intValue = 0, bool displayDefault = false)
        {
            var value = default(string);

            if (SettingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
            {
                var settingValue = DwordValues?.FirstOrDefault(s => textValue != null ? s.ValueName == textValue : s.Value == intValue);

                if (settingValue == null || !displayDefault && settingValue.Value == DefaultDwordValue)
                {
                    return value;
                }

                value = settingValue.ValueName;
            }
            else if (SettingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
            {
                var binValue = textValue ?? "0x" + intValue.ToString("x16");

                var settingValue = BinaryValues?.FirstOrDefault(s => s.ValueName.StartsWith(binValue));

                if (settingValue == null || !displayDefault && settingValue.Value == DefaultBinaryValue)
                {
                    return value;
                }

                value = settingValue.ValueName;
            }

            if (value != null && value.StartsWith("0x") && value.IndexOf(" ") > 1)
            {
                value = value[0..value.IndexOf(" ")];
            }

            return value;
        }

        public (uint intValue, bool isDefault) ToIntValue(string valueText)
        {
            var isDefault = true;
            var intValue = default(uint);

            if (SettingType == NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
            {
                var settingValue = DwordValues?.FirstOrDefault(s => s.ValueName == valueText);

                intValue = settingValue?.Value ?? UnsetDwordValue;
                isDefault = intValue == DefaultDwordValue || intValue == UnsetDwordValue;
            }
            else if (SettingType == NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
            {
                var settingValue = BinaryValues?.FirstOrDefault(s => s.ValueName.StartsWith(valueText));

                intValue = settingValue != null ? BitConverter.ToUInt32(settingValue?.Value) : UnsetDwordValue;
                isDefault = DefaultBinaryValue == null ? intValue == 0 : intValue == BitConverter.ToUInt32(DefaultBinaryValue);
            }

            return (intValue, isDefault);
        }
    }
}
