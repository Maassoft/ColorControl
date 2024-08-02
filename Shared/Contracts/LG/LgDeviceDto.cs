namespace ColorControl.Shared.Contracts.LG;

public class LgDeviceDto
{
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public string MacAddress { get; set; }
    public bool IsCustom { get; set; }
    public string Token { get; set; }
    public LgDeviceOptions Options { get; set; } = new();

    public bool IsDummy { get; set; }

    public bool PoweredOn { get; set; }

    public DateTimeOffset PoweredOffAt { get; set; }

    public bool IsConnected { get; set; }

    public bool IsSelected { get; set; }

    public override string ToString()
    {
        if (IpAddress != null)
        {
            return $"{Name} ({IpAddress})";
        }

        return Name;
    }
}

