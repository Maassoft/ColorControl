using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using ColorControl.Shared.XForms;
using MHC2Gen;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Navigation;

namespace ColorControl.XForms
{
    public partial class ColorProfileWindow : BaseWindow
    {
        private ColorProfileViewModel _viewModel;
        private readonly WinApiAdminService _winApiAdminService;

        public ColorProfileWindow(WinApiAdminService winApiAdminService)
        {
            _viewModel = new ColorProfileViewModel();

            DataContext = _viewModel;
            InitializeComponent();
            _winApiAdminService = winApiAdminService;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(processStartInfo);
        }

        public static void CreateAndShow(bool show = true)
        {
            if (Application.Current == null)
            {
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            }

            var window = Program.ServiceProvider.GetRequiredService<ColorProfileWindow>();

            if (show)
            {
                ElementHost.EnableModelessKeyboardInterop(window);
                window.WindowState = WindowState.Normal;
                window.Show();
                window.Topmost = true;
                window.Topmost = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //e.Cancel = true;

            //Hide();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Close();
            }

            base.OnStateChanged(e);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.Displays.Any())
            {
                MessageForms.WarningOk("There are no displays available that support HDR and have it enabled. Please activate HDR first.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var command = new GenerateProfileCommand
            {
                Description = _viewModel.Description,
                MinCLL = _viewModel.MinCLL,
                MaxCLL = _viewModel.MaxCLL,
                BlackLuminance = _viewModel.BlackLuminance,
                WhiteLuminance = _viewModel.WhiteLuminance,
                SDRMinBrightness = _viewModel.SDRMinBrightness,
                SDRMaxBrightness = _viewModel.SDRMaxBrightness,
                SDRTransferFunction = _viewModel.SDRTransferFunction,
                SDRBrightnessBoost = _viewModel.SDRBrightnessBoost,
                ColorGamut = _viewModel.ColorGamut,
                Gamma = _viewModel.CustomGamma,
                DevicePrimaries = _viewModel.GetDevicePrimaries(),
            };

            var bytes = MHC2Wrapper.GenerateSdrAcmProfile(command);

            if (_viewModel.SaveOption == SaveOption.SaveToFile)
            {
                var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                {
                    FileName = _viewModel.NewProfileName
                };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, bytes);
                }
                return;
            }

            var tempFilename = Path.GetFullPath(_viewModel.ProfileName, Path.GetTempPath());

            if (File.Exists(tempFilename))
            {
                File.Delete(tempFilename);
            }

            File.WriteAllBytes(tempFilename, bytes);

            _winApiAdminService.UninstallColorProfile(tempFilename);

            CCD.InstallColorProfile(tempFilename);

            if (_viewModel.SaveOption != SaveOption.InstallAndSetAsDefault)
            {
                return;
            }

            var displayName = _viewModel.SelectedDisplayName;

            var profileName = Path.GetFileName(tempFilename);

            CCD.SetDisplayDefaultColorProfile(displayName, profileName);
        }
    }
}