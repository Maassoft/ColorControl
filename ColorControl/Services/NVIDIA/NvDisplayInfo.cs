using NvAPIWrapper.Display;
using System.Collections.Generic;

namespace ColorControl.Services.NVIDIA
{
    class NvDisplayInfo
    {
        public Display Display { get; }

        public List<string> Values { get; }

        public string InfoLine { get; }

        public string Name { get; }

        public NvDisplayInfo(Display display, List<string> values, string infoLine, string name)
        {
            Display = display;
            Values = values;
            InfoLine = infoLine;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
