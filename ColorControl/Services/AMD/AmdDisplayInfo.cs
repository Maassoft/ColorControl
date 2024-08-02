using ATI.ADL;
using System.Collections.Generic;

namespace ColorControl.Services.AMD;

class AmdDisplayInfo
{
    public ADLDisplayInfo Display { get; }

    public List<string> Values { get; set; }

    public string InfoLine { get; set; }

    public AmdDisplayInfo(ADLDisplayInfo display, List<string> values, string infoLine)
    {
        Display = display;
        Values = values;
        InfoLine = infoLine;
    }
}
