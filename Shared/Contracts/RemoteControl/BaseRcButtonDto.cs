namespace ColorControl.Shared.Contracts.RemoteControl;

public class BaseRcButtonDto<TPreset, TButtonEnum> where TPreset : PresetBase where TButtonEnum : Enum
{
    public string Name { get; set; }
    public TButtonEnum Button { get; set; }
    public TPreset Preset { get; set; }
    public double Top { get; set; }
    public double Left { get; set; }

    public BaseRcButtonDto()
    {

    }

    public BaseRcButtonDto(BaseRcButtonDto<TPreset, TButtonEnum> button)
    {
        Update(button);
    }

    public void Update(BaseRcButtonDto<TPreset, TButtonEnum> button)
    {
        Name = button.Name;
        Button = button.Button;
        Preset = button.Preset;
        Top = button.Top;
        Left = button.Left;
    }
}

