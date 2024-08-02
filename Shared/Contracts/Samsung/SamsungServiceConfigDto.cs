namespace ColorControl.Shared.Contracts.Samsung;

public class SamsungServiceConfigDto
{
    public bool PowerOnAfterStartup { get; set; }
    public int PowerOnRetries { get; set; }
    public string PreferredMacAddress { get; set; }
    public string DeviceSearchKey { get; set; }
    public List<SamsungDeviceDto> Devices { get; set; }
    public int ShutdownDelay { get; set; }
    public bool SetSelectedDeviceByPowerOn { get; set; }
    public string QuickAccessShortcut { get; set; }
    public int DefaultButtonDelay { get; set; }
    public bool ShowAdvancedActions { get; set; }

    public SamsungServiceConfigDto()
    {
        ShutdownDelay = 1000;
        DeviceSearchKey = "Samsung";
        PowerOnRetries = 10;
        DefaultButtonDelay = 500;
        Devices = new List<SamsungDeviceDto>();
    }

    public static List<FieldDefinition> GetFields()
    {
        return
        [
            new FieldDefinition { FieldType = FieldType.TrackBar, Label = "M", SubLabel = "R",  }

        ];
    }
}
