using Newtonsoft.Json;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;

namespace ColorControl
{
    class PnpDev
    {
        [JsonIgnore]
        public DeviceInformation DevInfo { get; private set; }
        [JsonIgnore]
        public PnpObject PnpObject { get; private set; }

        public string Name { get; private set; }
        public string IpAddress { get; private set; }

        public string MacAddress { get; private set; }

        [JsonConstructor]
        public PnpDev(string name, string ipAddress, string macAddress)
        {
            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
        }
        public PnpDev(DeviceInformation devInfo, PnpObject pnpObject, string name, string ipAddress, string macAddress) : this(name, ipAddress, macAddress)
        {
            DevInfo = devInfo;
            PnpObject = pnpObject;
        }

        public override string ToString()
        {
            return (IsCustom() ? "Custom: " : "Auto detect: ") + $"{Name}" + (!string.IsNullOrEmpty(IpAddress) ? $"({IpAddress})" : string.Empty);
        }

        public bool IsCustom()
        {
            return DevInfo == null;
        }
    }
}
