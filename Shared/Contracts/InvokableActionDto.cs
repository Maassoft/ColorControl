using ColorControl.Shared.Contracts.LG;

namespace ColorControl.Shared.Contracts;

public class InvokableActionDto<T> where T : PresetBase
{
    public string Name { get; set; }
    public Type EnumType { get; set; }
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public int CurrentValue { get; set; }
    public int NumberOfValues { get; set; }
    public bool Advanced { get; set; }
    public T Preset { get; set; }
    public List<string> ValueLabels { get; set; }
    public LgDeviceDto SelectedDevice { get; set; }
}
