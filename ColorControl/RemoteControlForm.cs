using LgTv;
using NWin32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    partial class RemoteControlForm : Form
    {
        private LgService _lgService;
        private List<LgPreset> _buttons;
        private LgWebOsMouseService _mouseService;
        private int _maxX, _maxY;
        private int _posX, _posY;

        public RemoteControlForm(LgService lgService, List<LgPreset> buttons)
        {
            _lgService = lgService;
            _buttons = buttons;

            InitializeComponent();

            pnlMouse.MouseWheel += new MouseEventHandler(pnlMouse_MouseWheel);
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
            if (e is MouseEventArgs)
            {
                var preset = (LgPreset)(sender as Button).Tag;

                var _ = _lgService.ApplyPreset(preset);
            }
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
            if (!chkDirectMode.Checked)
            {
                return;
            }
            var cvt = new KeysConverter();
            var shortcut = (string)cvt.ConvertTo(key, typeof(string));

            var preset = _buttons.FirstOrDefault(p => p.shortcut != null && p.shortcut.Equals(shortcut));
            if (preset != null)
            {
                var _ = _lgService.ApplyPreset(preset);
            }
        }

        private void RemoteControlForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //HandleKey(e.KeyCode);
        }

        private void RemoteControlForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                chkDirectMode.Checked = !chkDirectMode.Checked;
                return;
            }

            HandleKey(e.KeyCode);
        }

        private void chkDirectMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDirectMode.Checked)
            {
                MessageForms.InfoOk(
@"If you enable Direct Mode you will be able to control the TV directly via your keyboard and mouse.
Note that your mouse will be captured in a box and that due to pointer accelaration the control will not be perfect.

Keyboard keys:
- arrow keys: navigate the menu
- backspace: go back
- escape: exit
- S: open picture settings
- media keys: volume up/down/mute

Mouse:
- left button: activate/open item
- right button: go back
- wheel: scroll through menus (only vertically)

Press F1 to stop Direct Mode."
                );

                if (_mouseService == null)
                {
                    _lgService.GetMouseAsync().ContinueWith((task) => BeginInvoke(new Action<Task<LgWebOsMouseService>>(SetMouseService), new[] { task }));
                }
            }
            else
            {
                Cursor.Clip = Rectangle.Empty;
            }
        }

        private void SetMouseService(Task<LgWebOsMouseService> mouseServiceTask)
        {
            _mouseService = mouseServiceTask.Result;

            _maxX = 1920;
            _maxY = 1080;
            _posX = _maxX / 2;
            _posY = _maxY / 2;
        }

        private void flowPanel_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void pnlMouse_MouseEnter(object sender, EventArgs e)
        {
            if (chkDirectMode.Checked)
            {
                Cursor.Clip = RectangleToScreen(pnlMouse.Bounds);
            }
        }

        private void RemoteControlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cursor.Clip = Rectangle.Empty;
        }

        private void pnlMouse_MouseUp(object sender, MouseEventArgs e)
        {
            if (chkDirectMode.Checked && _mouseService != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _mouseService.Click();
                }
                else
                {
                    HandleKey(Keys.Back);
                }
            }
        }

        private void HandleMouse(MouseEventArgs e)
        {
            if (chkDirectMode.Checked && _mouseService != null)
            {
                var scaleX = (double)e.X / pnlMouse.Width;
                var scaleY = (double)e.Y / pnlMouse.Height;

                var relX = scaleX * _maxX;
                var relY = scaleY * _maxY;

                var dx = (int)relX - _posX;
                var dy = (int)relY - _posY;

                _posX = Math.Min(_posX + dx, _maxX);
                _posY = Math.Min(_posY + dy, _maxY);

                lblPos.Text = $"Pos: {_posX}, {_posY}";

                //var cx = flowPanel.Width / 2;
                //var cy = flowPanel.Height / 2;

                //var dx = (e.X - cx) / 10;
                //var dy = (e.Y - cy) / 10;

                _mouseService.Move(dx, dy);
            }
        }

        private void pnlMouse_MouseMove(object sender, MouseEventArgs e)
        {
            HandleMouse(e);
        }

        private void pnlMouse_MouseWheel(object sender, MouseEventArgs e)
        {
            if (chkDirectMode.Checked && _mouseService != null)
            {
                var dy = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                if (dy != 0)
                {
                    _mouseService.Scroll(0, dy);
                }
            }
        }
    }
}
