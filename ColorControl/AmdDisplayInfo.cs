using ATI.ADL;
using System.Collections.Generic;

namespace ColorControl
{
    class AmdDisplayInfo
    {
        public ADLDisplayInfo Display { get; }

        public List<string> Values { get; }

        public string InfoLine { get; }

        public AmdDisplayInfo(ADLDisplayInfo display, List<string> values, string infoLine)
        {
            Display = display;
            Values = values;
            InfoLine = infoLine;
        }
    }
}
