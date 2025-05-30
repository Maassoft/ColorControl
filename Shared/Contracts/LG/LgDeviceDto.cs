using ColorControl.Shared.Contracts.Base;

namespace ColorControl.Shared.Contracts.LG;

public class LgDeviceDto : BaseDeviceDto
{
    public LgDeviceOptions Options { get; set; } = new();
}

