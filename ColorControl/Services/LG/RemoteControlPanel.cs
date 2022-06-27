using ColorControl.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Services.LG
{
    partial class RemoteControlPanel : UserControl
    {
        private static int Range = 35;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private LgService _lgService;
        private List<LgPreset> _buttons;
        private RemoteControlForm _remoteControlForm;
        private Bitmap _backupImage;
        private LgPreset _lastHoveredPreset;

        public RemoteControlPanel(LgService lgService, List<LgPreset> buttons)
        {
            _lgService = lgService;
            _buttons = buttons;

            InitializeComponent();

            _backupImage = new Bitmap(pbRemote.Image);

            var txtCoords = Utils.GetResourceFile("RC_coords.txt");

            foreach (var line in txtCoords.Split('\n'))
            {
                var columns = line.Split(';');

                var name = columns[0];
                var preset = _buttons.FirstOrDefault(p => p.name.Equals(name));
                if (preset != null)
                {
                    preset.X = int.Parse(columns[1]);
                    preset.Y = int.Parse(columns[2]);
                }
            }
        }

        private void RemoteControlPanel_Load(object sender, EventArgs e)
        {
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs)
            {
                var preset = (LgPreset)(sender as Button).Tag;

                var _ = _lgService.ApplyPreset(preset);
            }
        }

        private void pbRemote_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs mouseArgs)
            {
                var (posX, posY) = GetCoordsInImage(mouseArgs.X, mouseArgs.Y);

                var preset = _buttons.FirstOrDefault(p => posX >= (p.X - Range) && posX <= (p.X + Range) && posY >= (p.Y - Range) && posY <= (p.Y + Range));

                if (preset != null)
                {
                    if (preset.steps.Contains("POWER") && _lgService.SelectedDevice?.CurrentState != LgDevice.PowerState.Active)
                    {
                        Logger.Debug("Executing WOL instead of POWER");

                        var wolPreset = _buttons.FirstOrDefault(p => p.name.Equals("Wol"));
                        if (wolPreset != null)
                        {
                            preset = wolPreset;
                        }
                    }

                    var _ = _lgService.ApplyPreset(preset);
                }
            }
        }

        private (int, int) GetCoordsInImage(int curX, int curY)
        {
            var width = pbRemote.Width;
            var aspect = 0.27D;
            var currentAspect = (double)pbRemote.Width / pbRemote.Height;
            if (currentAspect > aspect)
            {
                width = (int)(aspect * pbRemote.Height);
                curX = curX - ((pbRemote.Width - width) / 2);
            }
            else
            {
                var height = (int)(pbRemote.Width / aspect);
                curY = curY - ((pbRemote.Height - height) / 2);
            }

            var factor = 405D / width;

            var posX = (int)(curX * factor);
            var posY = (int)(curY * factor);

            return (posX, posY);
        }

        private void btnRcDirect_Click(object sender, EventArgs e)
        {
        }

        private void pbRemote_MouseMove(object sender, MouseEventArgs mouseArgs)
        {
            var (posX, posY) = GetCoordsInImage(mouseArgs.X, mouseArgs.Y);

            var preset = _buttons.FirstOrDefault(p => posX >= (p.X - Range) && posX <= (p.X + Range) && posY >= (p.Y - Range) && posY <= (p.Y + Range));

            var refresh = false;

            if (_lastHoveredPreset != null && (preset == null || preset != _lastHoveredPreset))
            {
                var image = (Bitmap)pbRemote.Image;

                for (var y = Math.Max(0, _lastHoveredPreset.Y - Range); y < _lastHoveredPreset.Y + Range; y++)
                {
                    for (var x = Math.Max(0, _lastHoveredPreset.X - Range); x < _lastHoveredPreset.X + Range; x++)
                    {
                        image.SetPixel(x, y, _backupImage.GetPixel(x, y));

                    }
                }

                refresh = true;
            }

            if (preset != null && preset != _lastHoveredPreset)
            {
                var image = (Bitmap)pbRemote.Image;
                var rec = new Rectangle(Math.Max(0, preset.X - Range), Math.Max(0, preset.Y - Range), Range * 2, Range * 2);

                using (var g = Graphics.FromImage(image))
                {
                    using (var cloud_brush = new SolidBrush(Color.FromArgb(128, Color.White)))
                    {
                        g.FillEllipse(cloud_brush, rec);
                    }
                }

                //for (var y = preset.Y - 30; y < preset.Y + 30; y++)
                //{
                //    for (var x = preset.X - 30; x < preset.X + 30; x++)
                //    {
                //        var pixel = image.GetPixel(x, y);

                //        image.SetPixel(x, y, Color.FromArgb(150, 0, 0));

                //    }
                //}

                refresh = true;
            }

            if (refresh)
            {
                pbRemote.Refresh();
            }

            _lastHoveredPreset = preset;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_remoteControlForm == null || _remoteControlForm.IsDisposed)
            {
                var buttons = _lgService.GetRemoteControlButtons();
                _remoteControlForm = new RemoteControlForm(_lgService, buttons);
                _remoteControlForm.Show();
            }
            else
            {
                _remoteControlForm.Show();
            }
        }
    }
}
