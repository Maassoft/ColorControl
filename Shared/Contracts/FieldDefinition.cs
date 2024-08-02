using ColorControl.Shared.Common;

namespace ColorControl.Shared.Contracts;

public enum FieldType
{
    Text,
    Numeric,
    DropDown,
    Flags,
    Shortcut,
    CheckBox,
    TrackBar,
    Label
}

public class FieldDefinition
{
    public string Name { get; set; }
    public string Label { get; set; }
    public string SubLabel { get; set; }
    public FieldType FieldType { get; set; } = FieldType.Text;
    public IEnumerable<string> Values { get; set; }
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public int NumberOfValues { get; set; }
    public int StepSize { get; set; }
    public object Value { get; set; }
    public Type ValueType { get; set; }

    public int ValueAsInt => int.Parse(Value.ToString());
    public uint ValueAsUInt => uint.Parse(Value.ToString());
    public bool ValueAsBool => bool.Parse(Value.ToString());
    public T ValueAsEnum<T>() where T : struct => Utils.GetEnumValueByDescription<T>(Value.ToString());

    public int IntValue
    {
        get => ValueAsInt;
        set => Value = value;
    }

    public bool BoolValue
    {
        get => ValueAsBool;
        set => Value = value;
    }

    public string StringValue
    {
        get => Value?.ToString();
        set => Value = value;
    }

    public static FieldDefinition CreateEnumField<T>(string label, T value) where T : struct, IConvertible
    {
        return new FieldDefinition
        {
            Label = label,
            FieldType = FieldType.DropDown,
            Values = Utils.GetDescriptions(typeof(T), replaceUnderscore: true),
            Value = value.GetDescription()
        };
    }

    public static FieldDefinition CreateDropDownField(string label, IList<string> values)
    {
        return new FieldDefinition
        {
            Label = label,
            FieldType = FieldType.DropDown,
            Values = values,
            Value = values.FirstOrDefault()
        };
    }

    public static FieldDefinition CreateCheckField(string label, bool value = true)
    {
        return new FieldDefinition
        {
            Label = label,
            FieldType = FieldType.CheckBox,
            Value = value
        };
    }

    public static void UpdateObject(List<FieldDefinition> fields, object obj)
    {
        foreach (var field in fields)
        {
            var property = obj.GetType().GetProperty(field.Name);

            if (property == null)
            {
                continue;
            }

            switch (field.FieldType)
            {
                case FieldType.Text:
                case FieldType.Shortcut:
                    property.SetValue(obj, field.StringValue);
                    break;
                case FieldType.Numeric:
                    property.SetValue(obj, field.IntValue);
                    break;
                case FieldType.CheckBox:
                    property.SetValue(obj, field.BoolValue);
                    break;
            }
        }
    }
}
