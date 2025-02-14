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

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var command = new GenerateProfileCommand
        //    {
        //        Description = _viewModel.Description,
        //        IsHDRProfile = _viewModel.IsHDR,
        //        MinCLL = _viewModel.MinCLL,
        //        MaxCLL = _viewModel.MaxCLL,
        //        BlackLuminance = _viewModel.BlackLuminance,
        //        WhiteLuminance = _viewModel.WhiteLuminance,
        //        SDRMinBrightness = _viewModel.SDRMinBrightness,
        //        SDRMaxBrightness = _viewModel.SDRMaxBrightness,
        //        SDRTransferFunction = _viewModel.SDRTransferFunction,
        //        SDRBrightnessBoost = _viewModel.SDRBrightnessBoost,
        //        ColorGamut = _viewModel.ColorGamut,
        //        Gamma = _viewModel.CustomGamma,
        //        DevicePrimaries = _viewModel.GetDevicePrimaries(),
        //        ToneMappingFromLuminance = _viewModel.ToneMappingFromLuminance,
        //        ToneMappingToLuminance = _viewModel.ToneMappingToLuminance,
        //        CurveLikeLuminance = _viewModel.CurveLikeLuminance
        //    };

        //    var bytes = MHC2Wrapper.GenerateSdrAcmProfile(command);

        //    if (_viewModel.SaveOption == SaveOption.SaveToFile)
        //    {
        //        var saveFileDialog = new System.Windows.Forms.SaveFileDialog
        //        {
        //            FileName = _viewModel.NewProfileName
        //        };
        //        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            File.WriteAllBytes(saveFileDialog.FileName, bytes);
        //        }
        //        return;
        //    }

        //    var tempFilename = Path.GetFullPath(_viewModel.ProfileName, Path.GetTempPath());

        //    if (File.Exists(tempFilename))
        //    {
        //        File.Delete(tempFilename);
        //    }

        //    File.WriteAllBytes(tempFilename, bytes);

        //    _winApiAdminService.UninstallColorProfile(tempFilename);

        //    CCD.InstallColorProfile(tempFilename);

        //    if (_viewModel.SaveOption != SaveOption.InstallAndSetAsDefault)
        //    {
        //        return;
        //    }

        //    var displayName = _viewModel.SelectedDisplayName;

        //    var profileName = Path.GetFileName(tempFilename);

        //    if (CCD.SetDisplayDefaultColorProfile(displayName, profileName, _viewModel.SetMinMaxTml, _viewModel.IsHDR))
        //    {
        //        if (!_viewModel.ExistingProfiles.Contains(profileName))
        //        {
        //            _viewModel.ExistingProfiles.Add(profileName);
        //        }
        //        _viewModel.SelectedExistingProfile = profileName;
        //        _viewModel.Update();
        //    }
        //}


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var variants = new[] { 400, 500, 600, 700, 800, 900, 1000, 1200, 1500, 2000, 4000, 10000 };

            //var variants = new[] { (1500, 10000),(1400, 10000),(1300, 10000), (1200, 10000),(1100, 10000),(1000, 10000), (900, 10000), (800, 10000), (700, 10000), (600, 10000), (500, 10000), (10000, 10000), (10000, 400),
            //    (1500, 1500),(1400, 1500),(1300, 1500), (1200, 1500),(1100, 1500),(1000, 1500), (900, 1500), (800, 1500), (700, 1500), (600, 1500), (500, 1500), (400, 1500),
            //(1500, 1000),(1400, 1000),(1300, 1000), (1200, 1000),(1100, 1000),(1000, 1000), (900, 1000), (800, 1000), (700, 1000), (600, 1000), (500, 1000), (400, 1000),
            //(1500, 800),(1400, 800),(1300, 800), (1200, 800),(1100, 800),(1000, 800), (900, 800), (800, 800), (700, 800), (600, 800), (500, 800), (400, 800),
            //(1500, 600),(1400, 600),(1300, 600), (1200, 600),(1100, 600),(1000, 600), (900, 600), (800, 600), (700, 600), (600, 600), (500, 600), (400, 600),
            //(1500, 500),(1400, 500),(1300, 500), (1200, 500),(1100, 500),(1000, 500), (900, 500), (800, 500), (700, 500), (600, 500), (500, 500), (400, 500),
            //(1500, 400),(1400, 400),(1300, 400), (1200, 400),(1100, 400),(1000, 400), (900, 400), (800, 400), (700, 400), (600, 400), (500, 400), (400, 400)};

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < variants.Length; j++)
                {

                    var curveLike = variants[j] * .45 > 400 ? variants[j] * .45 : 400; // variants[i];
                    var output = variants[j];
                    try
                    {



                        var command = new GenerateProfileCommand
                        {
                            Description = "Gen HDR correct " + output + " Like " + curveLike,
                            IsHDRProfile = _viewModel.IsHDR,
                            MinCLL = _viewModel.MinCLL,
                            MaxCLL = (double)output,
                            BlackLuminance = _viewModel.BlackLuminance,
                            WhiteLuminance = (double)output,
                            SDRMinBrightness = _viewModel.SDRMinBrightness,
                            SDRMaxBrightness = (double)output,
                            SDRTransferFunction = SDRTransferFunction.ToneMappedPiecewise,
                            SDRBrightnessBoost = _viewModel.SDRBrightnessBoost,
                            ColorGamut = ColorGamut.Rec2020,
                            Gamma = _viewModel.CustomGamma,
                            DevicePrimaries = _viewModel.GetDevicePrimaries(),
                            ToneMappingFromLuminance = 1010,
                            ToneMappingToLuminance = (double)output,
                            CurveLikeLuminance = (double)curveLike,
                        };

                        var bytes = MHC2Wrapper.GenerateSdrAcmProfile(command);

                        if (_viewModel.SaveOption == SaveOption.SaveToFile)
                        {
                            var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                            {
                                FileName = "Gen HDR correct " + output + " Like " + curveLike + ".icm"
                            };
                            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                File.WriteAllBytes(saveFileDialog.FileName, bytes);
                            }
                            return;
                        }

                        var tempFilename = Path.GetFullPath("Gen HDR correct " + output + " Like " + curveLike + ".icm", Path.GetTempPath());

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

                        //var displayName = _viewModel.SelectedDisplayName;

                        //var profileName = Path.GetFileName(tempFilename);

                        //if (CCD.SetDisplayDefaultColorProfile(displayName, profileName, _viewModel.SetMinMaxTml, _viewModel.IsHDR))
                        //{
                        //    if (!_viewModel.ExistingProfiles.Contains(profileName))
                        //    {
                        //        _viewModel.ExistingProfiles.Add(profileName);
                        //    }
                        //    _viewModel.SelectedExistingProfile = profileName;
                        //    _viewModel.Update();
                        //}
                    }
                    catch (Exception ex)
                    {
                        double nn = 12;
                    }
                    ;
                }

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