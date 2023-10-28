using ColorControl.Shared.Common;
using ColorControl.Shared.Forms;
using NWin32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Services.LG
{
    partial class LgGameBar : Form
    {
        private class TbSetting
        {
            public TrackBar TrackBar { get; set; }
            public string Name { get; set; }
            public Type EnumType { get; set; }
            public string PropertyName { get; set; }
        }

        private LgService _lgService;
        private LgDevice _lgDevice;
        private ToolTip _toolTip;
        private bool _settingChanged = false;
        private List<TrackBar> _trackBars = new List<TrackBar>();
        private List<string> _lastActions = new List<string>();

        public LgGameBar(LgService lgService)
        {
            _lgService = lgService;

            _lgDevice = _lgService.SelectedDevice;

            _lgDevice.PictureSettingsChangedEvent += LgDevice_PictureSettingsChangedEvent;

            InitializeComponent();

            _toolTip = new ToolTip();
        }

        private void GenerateComponents()
        {
            var actions = _lgDevice.GetActionsForGameBar();
            var actionNames = actions.Select(a => a.Name).ToList();

            if (_lastActions.Count == actions.Count && _lastActions.All(a => actionNames.Contains(a)))
            {
                return;
            }

            flowPanel.Controls.Clear();
            _trackBars.Clear();

            foreach (var action in actions)
            {
                CreateGroup(action.Title, action.Name, (int)action.MinValue, (int)action.MaxValue, action.EnumType);
            }

            _lastActions.Clear();
            _lastActions.AddRange(actionNames);

            Width = 28 + (_trackBars.Count() * 56);
        }

        private TrackBar CreateGroup(string title, string settingName = null, int min = 0, int max = 100, Type enumType = null)
        {
            if (settingName == null)
            {
                settingName = title.ToLowerInvariant();
            }

            var panel = new Panel();
            panel.Width = 50;
            panel.Height = flowPanel.Height;
            var label = new Label();
            label.Text = title;
            label.ForeColor = Color.White;
            label.Parent = panel;

            if (min == 0 && max == 0 && enumType != null)
            {
                max = Enum.GetValues(enumType).Length - 1;
            }

            var trackBar = CreateTrackBar(min, max);
            trackBar.Top = label.Height;
            trackBar.Height = panel.Height - trackBar.Top - 4;
            trackBar.Parent = panel;
            trackBar.Scroll += tbTrackBar_Scroll;
            var margin = trackBar.Margin;
            margin.Top = 0;
            margin.Bottom = 0;
            trackBar.Margin = margin;
            trackBar.ValueChanged += TrackBar_ValueChanged;

            var setting = new TbSetting
            {
                TrackBar = trackBar,
                Name = settingName,
                EnumType = enumType,
                PropertyName = Utils.FirstCharUpperCase(settingName)
            };

            trackBar.Tag = setting;

            panel.Parent = flowPanel;

            _trackBars.Add(trackBar);

            return trackBar;
        }

        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            //var trackBar = sender as TrackBar;

            //if (_toolTip != null)
            //{
            //    _toolTip.SetToolTip(trackBar, trackBar.Value.ToString());
            //}
        }

        private TrackBar CreateTrackBar(int min, int max)
        {
            var trackBar = new TrackBar();
            trackBar.BackColor = Color.Black;
            trackBar.Orientation = Orientation.Vertical;
            trackBar.TickFrequency = (max >= 50) ? 5 : 1;
            trackBar.TickStyle = TickStyle.Both;
            trackBar.Minimum = min;
            trackBar.Maximum = max;

            return trackBar;
        }

        private void LgDevice_PictureSettingsChangedEvent(object sender, EventArgs e)
        {
            if (_settingChanged)
            {
                _settingChanged = false;
                return;
            }

            if (!Visible)
            {
                return;
            }

            FormUtils.BeginInvokeCheck(this, UpdateValues);
        }

        private void UpdateValues()
        {
            var dynType = _lgDevice.PictureSettings.GetType();
            var actions = _lgDevice.GetInvokableActions();
            foreach (var trackbar in _trackBars.Where(t => !t.Focused))
            {
                var setting = (TbSetting)trackbar.Tag;

                if (setting.PropertyName == null)
                {
                    continue;
                }

                var prop = dynType.GetProperty(setting.PropertyName);

                if (prop != null)
                {
                    var value = prop.GetValue(_lgDevice.PictureSettings);

                    if (value is int intValue)
                    {
                        trackbar.Value = intValue;
                    }
                }
                else
                {
                    var action = actions.FirstOrDefault(a => a.Name == setting.Name);
                    if (action != null)
                    {
                        trackbar.Value = action.CurrentValue;
                    }
                }
            }
        }

        private void RemoteControlForm_Load(object sender, EventArgs e)
        {
            Left = _lgService.Config.GameBarLeft;
            Top = _lgService.Config.GameBarTop;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs)
            {
                var preset = (LgPreset)(sender as Button).Tag;

                var _ = _lgService.ApplyPreset(preset);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void RemoteControlForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                var intPtr = new UIntPtr(2);
                NativeMethods.SendMessageW(Handle, NativeConstants.WM_NCLBUTTONDOWN, intPtr, IntPtr.Zero);
            }
        }

        private void RemoteControlForm_KeyDown(object sender, KeyEventArgs e)
        {
            //var key = e.KeyCode;
            //HandleKey(key);
        }

        private void flowPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //HandleKey(e.KeyCode);
        }

        private void HandleKey(Keys key)
        {
            var cvt = new KeysConverter();
            var shortcut = (string)cvt.ConvertTo(key, typeof(string));
        }

        private void RemoteControlForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //HandleKey(e.KeyCode);
        }

        private void RemoteControlForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                return;
            }

            HandleKey(e.KeyCode);
        }

        private void flowPanel_MouseMove(object sender, MouseEventArgs e)
        {
            ResetAutoHide();
        }

        private void RemoteControlForm_Shown(object sender, EventArgs e)
        {
            GenerateComponents();
            UpdateInfo();
            ResetAutoHide();
        }

        private void UpdateInfo()
        {
            lblTvName.Text = $"Device: {_lgDevice?.Name ?? "No tv selected"}";
            lblProcessName.Text = $"Current process: {_lgService.CurrentProcessEvent?.ForegroundProcess?.ProcessName ?? _lgService.CurrentProcessEvent?.LastFullScreenProcessName ?? "No full screen process"}";

            UpdateValues();
        }

        private void LgGameBar_Deactivate(object sender, EventArgs e)
        {
            _lgService.Config.GameBarLeft = Left;
            _lgService.Config.GameBarTop = Top;
            Hide();
        }

        private void tmrHide_Tick(object sender, EventArgs e)
        {
            _lgService.Config.GameBarLeft = Left;
            _lgService.Config.GameBarTop = Top;
            Hide();
        }

        private async void tbTrackBar_Scroll(object sender, EventArgs e)
        {
            _settingChanged = true;
            ResetAutoHide();

            var trackBar = sender as TrackBar;
            var setting = (TbSetting)trackBar.Tag;

            string value;
            if (setting.EnumType == null)
            {
                value = trackBar.Value.ToString();
            }
            else
            {
                var enumValue = Enum.Parse(setting.EnumType, trackBar.Value.ToString());
                value = enumValue.ToString();
            }

            await _lgDevice.SetSystemSettings(setting.Name, value);

            if (_toolTip != null)
            {
                _toolTip.Show(value, trackBar, 10, 50, 1500);
            }
        }

        private void LgGameBar_Activated(object sender, EventArgs e)
        {
            GenerateComponents();
            UpdateInfo();
            ResetAutoHide();
        }

        private void LgGameBar_Move(object sender, EventArgs e)
        {
            ResetAutoHide();
        }

        private void ResetAutoHide()
        {
            tmrHide.Enabled = false;
            tmrHide.Enabled = true;
        }
    }
}
