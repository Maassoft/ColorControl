using System.Collections.Generic;
using System.Linq;

namespace ColorControl
{
    class PnpDev
    {
        public string name { get; set; }
        public string ipAddress { get; set; }

        public PnpDev(string name, string ipAddress)
        {
            this.name = name;
            this.ipAddress = ipAddress;
        }

        public override string ToString()
        {
            return name + " (" + ipAddress + ")";
        }
    }
}
