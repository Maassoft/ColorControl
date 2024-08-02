namespace ColorControl.Shared.Contracts.Samsung;

public static class SamsungFields
{
    public static List<FieldDefinition> GetSettingsFields(SamsungServiceConfigDto config)
    {
        return new List<FieldDefinition>
        {
            {
                new FieldDefinition
                {
                    Name = "DeviceSearchKey",
                    FieldType = FieldType.Text,
                    Label = "Device search filter",
                    SubLabel = "This filters the devices that are read from the Windows Device Manager.",
                    Value = config.DeviceSearchKey
                }
            },
            {
                new FieldDefinition
                {
                    Name = "PowerOnRetries",
                    FieldType = FieldType.Numeric,
                    Label = "Maximum number of retries powering on after startup/resume.",
                    SubLabel = "Retries are necessary to wait for the network link of your pc to be established.",
                    MinValue = 1,
                    MaxValue = 40,
                    Value = config.PowerOnRetries
                }
            },
            {
                new FieldDefinition
                {
                    Name = "ShutdownDelay",
                    FieldType = FieldType.Numeric,
                    Label = "Delay when shutting down/restarting PC (milliseconds).",
                    SubLabel = "This delay may prevent the tv from powering off when restarting the pc.",
                    MinValue = 0,
                    MaxValue = 5000,
                    Value = config.ShutdownDelay
                }
            },
            {
                new FieldDefinition
                {
                    Name = "DefaultButtonDelay",
                    FieldType = FieldType.Numeric,
                    Label = "Default delay between remote control button presses (milliseconds).",
                    SubLabel = "Increasing this delay may prevent skipped button presses.",
                    MinValue = 100,
                    MaxValue = 2000,
                    Value = config.DefaultButtonDelay
                }
            },
            {
                new FieldDefinition
                {
                    Name = "QuickAccessShortcut",
                    FieldType = FieldType.Shortcut,
                    Label = "Quick Access shortcut",
                    Value = config.QuickAccessShortcut
                }
            },
            {
                new FieldDefinition
                {
                    Name = "SetSelectedDeviceByPowerOn",
                    FieldType = FieldType.CheckBox,
                    Label = "Automatically set selected device to last powered on",
                    Value = config.SetSelectedDeviceByPowerOn
                }
            },
            {
                new FieldDefinition
                {
                    Name = "ShowAdvancedActions",
                    FieldType = FieldType.CheckBox,
                    Label = "Show advanced actions under the Expert-button (Service Menu)",
                    SubLabel = "WARNING: entering the Service Menu is not recommended. This app and its creator are in no way accountable for any damages it may cause to your tv.",
                    Value = config.ShowAdvancedActions
                }
            },
        };
    }
}
