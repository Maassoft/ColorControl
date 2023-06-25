using ColorControl.Common;
using ColorControl.Forms;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Services.NVIDIA
{
    public partial class NvDitherPanel : UserControl, IModulePanel
    {
        private NvService _nvService;
        private bool _updatingDitherSettings;
        private bool _initialized;

        internal NvDitherPanel(NvService nvService)
        {
            _nvService = nvService;

            InitializeComponent();

            cbxDitheringBitDepth.Items.AddRange(Utils.GetDescriptions<NvDitherBits>().ToArray());
            cbxDitheringMode.Items.AddRange(Utils.GetDescriptions<NvDitherMode>().ToArray());
            FillGradient();

            RefreshDisplays();

            UpdateDitherSettings();

            _initialized = true;

            if (DarkModeUtils.UseDarkMode)
            {
                grpNvidiaOptions.ForeColor = FormUtils.CurrentForeColor;
            }
        }

        private void RefreshDisplays()
        {
            var displays = _nvService.GetSimpleDisplayInfos();

            if (displays == null)
            {
                return;
            }

            var primaryDisplay = _nvService.GetPrimaryDisplay();
            var primaryDisplayInfo = displays.FirstOrDefault(d => d.Display == primaryDisplay);
            var index = primaryDisplayInfo != null ? displays.IndexOf(primaryDisplayInfo) : 0;

            cbxDitheringDisplay.Items.Clear();
            cbxDitheringDisplay.Items.AddRange(displays.ToArray());

            if (cbxDitheringDisplay.SelectedIndex == -1)
            {
                cbxDitheringDisplay.SelectedIndex = index;
            }
        }

        private void UpdateDitherSettings()
        {
            var display = ((NvDisplayInfo)cbxDitheringDisplay.SelectedItem)?.Display;
            var ditherInfo = _nvService.GetDithering(display);

            if (ditherInfo.state == -1)
            {
                MessageForms.ErrorOk("Error retrieving dithering settings. See log for details.");
                return;
            }

            var state = (NvDitherState)ditherInfo.state;

            _updatingDitherSettings = true;
            try
            {
                chkDitheringEnabled.CheckState = state switch { NvDitherState.Enabled => CheckState.Checked, NvDitherState.Disabled => CheckState.Unchecked, _ => CheckState.Indeterminate };
                cbxDitheringBitDepth.SelectedIndex = ditherInfo.bits;
                cbxDitheringMode.SelectedIndex = ditherInfo.mode;

                cbxDitheringBitDepth.Enabled = state == NvDitherState.Enabled;
                cbxDitheringMode.Enabled = state == NvDitherState.Enabled;
            }
            finally
            {
                _updatingDitherSettings = false;
            }
        }

        private void FillGradient()
        {
            if (pbGradient.Image == null)
            {
                pbGradient.Image = Utils.GenerateGradientBitmap(pbGradient.Width, pbGradient.Height);
            }
        }

        private void ApplyDitheringOptions()
        {
            if (_updatingDitherSettings)
            {
                return;
            }

            var state = chkDitheringEnabled.CheckState switch { CheckState.Checked => NvDitherState.Enabled, CheckState.Unchecked => NvDitherState.Disabled, _ => NvDitherState.Auto };
            var bitDepth = cbxDitheringBitDepth.SelectedIndex;
            var mode = cbxDitheringMode.SelectedIndex;
            var display = ((NvDisplayInfo)cbxDitheringDisplay.SelectedItem)?.Display;

            if (_nvService.SetDithering(state, (uint)bitDepth, (uint)(mode > -1 ? mode : (int)NvDitherMode.Temporal), currentDisplay: display) && state == NvDitherState.Auto)
            {
                UpdateDitherSettings();
            }
        }

        private void cbxDitheringBitDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDitheringOptions();
        }

        private void cbxDitheringMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDitheringOptions();
        }

        private void chkDitheringEnabled_CheckStateChanged(object sender, EventArgs e)
        {
            cbxDitheringBitDepth.Enabled = chkDitheringEnabled.CheckState == CheckState.Checked;
            cbxDitheringMode.Enabled = chkDitheringEnabled.CheckState == CheckState.Checked;

            ApplyDitheringOptions();
        }

        private void cbxDitheringDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            UpdateDitherSettings();
        }

        public void UpdateInfo()
        {
            RefreshDisplays();
            UpdateDitherSettings();
        }
    }
}
