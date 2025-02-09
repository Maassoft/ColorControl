using System.Text.Json.Serialization;

namespace ColorControl.Shared.Contracts.NVIDIA;

public enum NvSettingType
{
	DwordType,
	BinaryType
}

public class NvSettingItemDto
{
	public uint SettingId { get; set; }

	public string SettingText { get; set; }

	public string ValueText { get; set; }

	public string ValueRaw { get; set; }

	[JsonIgnore]
	public bool ValueChanged { get; set; }

	private uint _value;

	public uint Value
	{
		get
		{
			return _value;
		}

		set
		{
			_value = value;
			ValueChanged = true;
		}
	}

	public string GroupName { get; set; }

	public int State { get; set; }

	public bool IsStringValue { get; set; }

	public bool IsApiExposed { get; set; }

	public bool IsSettingHidden { get; set; }

	public NvSettingType SettingType { get; set; }

	public uint DefaultValue { get; set; }

	public List<NvSettingItemValue> Values { get; set; }

	public string FriendlyName { get; set; }

	public override string ToString()
	{
		return string.Format("{0}; 0x{1:X8}; {2}; {3}; {4};", State, SettingId, SettingText, ValueText, ValueRaw);
	}
}

public class NvSettingItemValue
{
	public uint Value { get; set; }
	public string ValueName { get; set; }
}