namespace ColorControl.Shared.Native
{
    public class PnpDev
    {
        public bool IsCustom { get; private set; }

        public string Name { get; private set; }
        public string IpAddress { get; private set; }

        public string MacAddress { get; private set; }

        public PnpDev(string name, string ipAddress, string macAddress)
        {
            IsCustom = true;
            Name = name;
            IpAddress = ipAddress;
            MacAddress = macAddress;
        }

        public PnpDev(bool isCustom, string name, string ipAddress, string macAddress) : this(name, ipAddress, macAddress)
        {
            IsCustom = isCustom;
        }

        public override string ToString()
        {
            return (IsCustom ? "Custom: " : "Auto detect: ") + $"{Name}" + (!string.IsNullOrEmpty(IpAddress) ? $"({IpAddress})" : string.Empty);
        }
    }
}
