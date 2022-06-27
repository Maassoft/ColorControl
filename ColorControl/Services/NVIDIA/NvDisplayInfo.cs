using NvAPIWrapper.Display;
using System.Collections.Generic;

namespace ColorControl.Services.NVIDIA
{
    class NvDisplayInfo
    {
        public Display Display { get; }

        public List<string> Values { get; }

        public string InfoLine { get; }

        public NvDisplayInfo(Display display, List<string> values, string infoLine)
        {
            Display = display;
            Values = values;
            InfoLine = infoLine;
        }
    }
}
