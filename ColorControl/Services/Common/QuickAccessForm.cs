using ColorControl.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Services.Common
{
    partial class QuickAccessForm<T> : Form where T : PresetBase, new()
    {
        private ServiceBase<T> _service;
        private List<Button> _buttons = new List<Button>();
        private bool _shown = false;

        private static Dictionary<ServiceBase<T>, QuickAccessForm<T>> QuickAccessForms = new();

        public static void ToggleQuickAccessForm(ServiceBase<T> service)
        {
            if (!QuickAccessForms.TryGetValue(service, out var form) || form.IsDisposed)
            {
                form = new QuickAccessForm<T>(service);
                QuickAccessForms.Add(service, form);
            }


            if (!form.Visible)
            {
                form.Show();
                form.Activate();
            }
            else
            {
                form.Hide();
            }
        }

        public QuickAccessForm(ServiceBase<T> service)
        {
            _service = service;

            InitializeComponent();

            lblTitle.Text = $"Quick Access - {service.ServiceName}";
        }

        private void GenerateComponents()
        {
            var presets = _service.GetPresets().Where(p => p.ShowInQuickAccess);

            flowPanel.Controls.Clear();
            _buttons.Clear();

            if (presets.Any())
            {
                foreach (var preset in presets)
                {
                    CreateGroup(preset);
                }
                Height = Height - flowPanel.Height + (_buttons.Count() * 46) + 8;

                return;
            }
            var panel = new Panel();
            panel.Width = flowPanel.Width - 4;
            panel.Height = 40;

            var label = new Label();
            label.Text = "No Quick Access presets configured";
            label.Parent = panel;
            label.AutoSize = true;
            label.ForeColor = Color.White;

            panel.Parent = flowPanel;

            Height = Height - flowPanel.Height + 36;
        }

        private Button CreateGroup(PresetBase preset)
        {
            var panel = new Panel();
            panel.Width = flowPanel.Width - 4;
            panel.Height = 40;

            var button = new Button();
            button.Text = preset.name;
            button.Tag = preset;
            button.Font = new Font(button.Font, FontStyle.Bold);
            button.Parent = panel;
            button.Width = panel.Width - 4;
            button.Height = panel.Height - 4;
            var margin = button.Margin;
            margin.Top = 0;
            margin.Bottom = 0;
            button.Margin = margin;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.DarkGray;
            button.FlatAppearance.MouseOverBackColor = Color.DimGray;
            button.Click += Button_Click;

            panel.Parent = flowPanel;

            _buttons.Add(button);

            return button;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var preset = (T)(sender as Button).Tag;

            if (NWin32.NativeMethods.GetAsyncKeyState(NWin32.NativeConstants.VK_SHIFT) < 0)
            {
                preset.ShowInQuickAccess = false;

                GenerateComponents();

                return;
            }

            Hide();

            var _ = _service.ApplyPreset(preset);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void HandleKey(Keys key)
        {
            if (key == Keys.Escape)
            {
                Hide();
                return;
            }

            ResetAutoHide();

            if (!_buttons.Any())
            {
                return;
            }

            if (!(ActiveControl is Button button))
            {
                _buttons.First().Focus();
                return;
            }
            var diff = 0;

            if (key == Keys.Up)
            {
                diff = -1;
            }
            else if (key == Keys.Down)
            {
                diff = 1;
            }

            if (diff == 0)
            {
                return;
            }

            var index = _buttons.IndexOf(button);
            index += diff;

            if (index == -1)
            {
                index = _buttons.Count - 1;
            }
            else if (index == _buttons.Count)
            {
                index = 0;
            }

            button = _buttons[index];

            button.Focus();
        }

        private void RemoteControlForm_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKey(e.KeyCode);
        }

        private void flowPanel_MouseMove(object sender, MouseEventArgs e)
        {
            ResetAutoHide();
        }

        private void RemoteControlForm_Shown(object sender, EventArgs e)
        {
            GenerateComponents();
            ResetAutoHide();

            _shown = true;
        }

        private void LgGameBar_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }

        private void tmrHide_Tick(object sender, EventArgs e)
        {
            Hide();
        }

        private void LgGameBar_Activated(object sender, EventArgs e)
        {
            if (_shown)
            {
                GenerateComponents();
            }
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

        private void QuickAccessForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            HandleKey(e.KeyCode);
        }
    }
}
