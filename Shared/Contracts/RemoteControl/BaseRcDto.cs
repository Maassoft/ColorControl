
namespace ColorControl.Shared.Contracts.RemoteControl;

public class BaseRcDto<TPreset, TButtonEnum> where TPreset : PresetBase where TButtonEnum : Enum
{
    public string Name { get; set; }
    public string ImageFileName { get; set; }
    public List<BaseRcButtonDto<TPreset, TButtonEnum>> Buttons { get; set; }

    public void Update(BaseRcDto<TPreset, TButtonEnum> remoteSpec)
    {
        Name = remoteSpec.Name;
        ImageFileName = remoteSpec.ImageFileName;
        Buttons = remoteSpec.Buttons;
    }
}

