using ColorControl.Shared.Forms;
using NvAPIWrapper.Display;
using System.Windows.Forms;

namespace ColorControl.Services.NVIDIA
{
    public partial class NvInfoPanel : UserControl, IModulePanel
    {
        private NvService _nvService;

        internal NvInfoPanel(NvService nvService)
        {
            _nvService = nvService;

            InitializeComponent();

            if (DarkModeUtils.UseDarkMode)
            {
                grpNVIDIAInfo.ForeColor = FormUtils.CurrentForeColor;
            }

            RefreshInfo();
        }

        public void UpdateInfo()
        {
            RefreshInfo();
        }

        private void btnRefreshNVIDIAInfo_Click(object sender, System.EventArgs e)
        {
        }

        private void RefreshInfo()
        {
            tvNVIDIAInfo.Nodes.Clear();
            var displays = Display.GetDisplays();
            for (var i = 0; i < displays.Length; i++)
            {
                var display = displays[i];
                var node = TreeNodeBuilder.CreateTree(display, $"Display[{i}]");
                tvNVIDIAInfo.Nodes.Add(node);
            }
        }
    }
}
