using NWin32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    partial class RemoteControlForm : Form
    {
        private LgService _lgService;
        private List<LgPreset> _buttons;

        public RemoteControlForm(LgService lgService, List<LgPreset> buttons)
        {
            _lgService = lgService;
            _buttons = buttons;

            InitializeComponent();
        }

        private void RemoteControlForm_Load(object sender, EventArgs e)
        {
            foreach (var preset in _buttons)
            {
                var button = new Button();
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.DarkGray;
                button.FlatAppearance.MouseOverBackColor = Color.DarkGray;
                button.Text = preset.name;
                button.Click += ButtonClick;
                button.Tag = preset;

                button.Parent = flowPanel;
            }
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var preset = (LgPreset)(sender as Button).Tag;

            _lgService.ApplyPreset(preset);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}
