using ColorControl.Shared.Native;
using NStandard;
using NvAPIWrapper.Display;
using System.Collections.Generic;

namespace ColorControl.Services.NVIDIA
{
    class NvDisplayInfo
    {
        public Display Display { get; }

        public List<string> Values { get; set; }

        public string InfoLine { get; set; }

        public string Name { get; private set; }

        public string DisplayId { get; private set; }

        public NvDisplayInfo(Display display, List<string> values, string infoLine, string name = null)
        {
            Display = display;
            Values = values;
            InfoLine = infoLine;
            Name = name;

            SetDisplayName();
        }

        private void SetDisplayName()
        {
            if (!Name.IsNullOrEmpty())
            {
                return;
            }


            if (Display == null)
            {
                Name = "Unknown";

                return;
            }

            var info = CCD.GetDisplayInfo(Display.Name);

            Name = info?.FriendlyName;
            DisplayId = info?.DisplayId;

            if (Name.IsNullOrEmpty())
            {
                Name = DisplayId;
            }
            else
            {
                Name = $"{Name} ({DisplayId})";
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
