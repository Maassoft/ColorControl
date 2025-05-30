using ColorControl.Shared.Contracts.Base;

namespace ColorControl.Shared.Contracts.Samsung;

public class SamsungDeviceDto : BaseDeviceDto
{
    public SamsungDeviceOptions Options { get; set; } = new();
}

