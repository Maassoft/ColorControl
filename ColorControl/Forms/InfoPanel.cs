using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using System.Diagnostics;
using System.Windows.Forms;

namespace ColorControl.Forms
{
    public partial class InfoPanel : UserControl
    {
        private readonly WinApiService _winApiService;
        private bool _initialized;

        public InfoPanel(WinApiService winApiService)
        {
            _winApiService = winApiService;
            InitializeComponent();

            _initialized = true;

            if (DarkModeUtils.UseDarkMode)
            {
                grpInfo.ForeColor = FormUtils.CurrentForeColor;
            }

            Init();
        }

        private void Init()
        {
            var currentVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

            Text = Application.ProductName + " " + Application.ProductVersion;

            if (_winApiService.IsAdministrator())
            {
                Text += " (administrator)";
            }

            lblInfo.Text = Text + " - " + currentVersionInfo.LegalCopyright;

            lbPlugins.Items.Add("lgtv.net by gr4b4z");
            lbPlugins.Items.Add("Newtonsoft.Json by James Newton-King");
            lbPlugins.Items.Add("NLog by Jarek Kowalski, Kim Christensen, Julian Verdurmen");
            lbPlugins.Items.Add("NvAPIWrapper.Net by Soroush Falahati");
            lbPlugins.Items.Add("NWin32 by zmjack");
            lbPlugins.Items.Add("TaskScheduler by David Hall");
            lbPlugins.Items.Add("NVIDIA Profile Inspector by Orbmu2k");
            lbPlugins.Items.Add("NvidiaML wrapper by LibreHardwareMonitor");
            lbPlugins.Items.Add("Novideo sRGB by ledoge");
            lbPlugins.Items.Add("NLogViewer by dojo90");
            lbPlugins.Items.Add("WPFDarkTheme by AngryCarrot789");

        }

    }
}
