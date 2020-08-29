using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;

namespace ColorControl
{
    class PnpDev
    {
        public DeviceInformation DevInfo { get; private set; }
        public PnpObject PnpObject { get; private set; }

        public string Name { get; private set; }
        public string IpAddress { get; private set; }

        public string MacAddress { get; private set; }

        public PnpDev(DeviceInformation devInfo, PnpObject pnpObject, string name, string ipAddress, string macAddress)
        {
            DevInfo = devInfo;
            PnpObject = pnpObject;
            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
        }

        public override string ToString()
        {
            return Name + " (" + IpAddress + ")";
        }
    }
}
