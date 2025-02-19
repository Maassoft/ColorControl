using ColorControl.Shared.Common;
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
        private readonly GlobalContext _globalContext;

        public ColorProfileWindow(WinApiAdminService winApiAdminService, GlobalContext globalContext, bool isHDR)
        {
            _winApiAdminService = winApiAdminService;
            _globalContext = globalContext;

            _viewModel = new ColorProfileViewModel(isHDR);
            _viewModel.SetMinMaxTml = globalContext.Config.SetMinTmlAndMaxTml;

            DataContext = _viewModel;
            InitializeComponent();
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(processStartInfo);
        }

        public static void CreateAndShow(bool show = true, bool isHDR = true)
        {
            Utils.EnsureApplication();

            var winApiAdminService = Program.ServiceProvider.GetRequiredService<WinApiAdminService>();
            var globalContext = Program.ServiceProvider.GetRequiredService<GlobalContext>();
            var window = new ColorProfileWindow(winApiAdminService, globalContext, isHDR);

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
                var message = _viewModel.IsHDR ?
                    "There are no displays available that support HDR and have it enabled. Please activate HDR first." :
                    "There are no displays available are in SDR mode. Please deactivate HDR first.";

                MessageForms.WarningOk(message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var command = new GenerateProfileCommand
            {
                Description = _viewModel.Description,
                IsHDRProfile = _viewModel.IsHDR,
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
                ToneMappingFromLuminance = _viewModel.ToneMappingFromLuminance,
                ToneMappingToLuminance = _viewModel.ToneMappingToLuminance,
                HdrBrightnessMultiplier = _viewModel.HdrBrightnessMultiplier,
                HdrGammaMultiplier = _viewModel.HdrGammaMultiplier
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

            if (CCD.SetDisplayDefaultColorProfile(displayName, profileName, _viewModel.SetMinMaxTml, _viewModel.IsHDR))
            {
                if (!_viewModel.ExistingProfiles.Contains(profileName))
                {
                    _viewModel.ExistingProfiles.Add(profileName);
                }
                _viewModel.SelectedExistingProfile = profileName;
                _viewModel.Update();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var existingProfile = _viewModel.SelectedExistingProfile;

            if (existingProfile == ColorProfileViewModel.CreateANewProfile)
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = "Color profiles (*.icc;*.icm)|*.icc;*.icm"
                };
                if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                existingProfile = openFileDialog.FileName;
            }

            if (MessageForms.QuestionYesNo("This will discard all current changes. Do you want to continue?") != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            // Load profile
            var profileProperties = MHC2Wrapper.LoadProfile(existingProfile, _viewModel.IsHDR);

            _viewModel.RedPoint = new(profileProperties.DevicePrimaries.Red);
            _viewModel.GreenPoint = new(profileProperties.DevicePrimaries.Green);
            _viewModel.BluePoint = new(profileProperties.DevicePrimaries.Blue);
            _viewModel.WhitePoint = new(profileProperties.DevicePrimaries.White);
            _viewModel.ColorGamut = profileProperties.ColorGamut;
            _viewModel.BlackLuminance = profileProperties.BlackLuminance;
            _viewModel.WhiteLuminance = profileProperties.WhiteLuminance;
            _viewModel.SDRTransferFunction = profileProperties.SDRTransferFunction;
            _viewModel.CustomGamma = profileProperties.Gamma;
            _viewModel.SDRMinBrightness = profileProperties.SDRMinBrightness;
            _viewModel.SDRMaxBrightness = profileProperties.SDRMaxBrightness;
            _viewModel.SDRBrightnessBoost = profileProperties.SDRBrightnessBoost;
            _viewModel.MinCLL = profileProperties.MinCLL;
            _viewModel.MaxCLL = profileProperties.MaxCLL;

            _viewModel.Update();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var existingProfile = _viewModel.SelectedExistingProfile;

            if (existingProfile == ColorProfileViewModel.CreateANewProfile)
            {
                return;
            }

            if (MessageForms.QuestionYesNo($"Are you sure you want to remove the color profile '{existingProfile}'?") != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            if (_winApiAdminService.UninstallColorProfile(existingProfile))
            {
                _viewModel.SelectedExistingProfile = ColorProfileViewModel.CreateANewProfile;
                _viewModel.ExistingProfiles.Remove(existingProfile);
                _viewModel.Update();
            }
        }
    }
}