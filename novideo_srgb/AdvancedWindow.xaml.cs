using ColorControl.Shared.XForms;
using System.Windows;

namespace novideo_srgb
{
    public partial class AdvancedWindow : BaseWindow
    {
        private AdvancedViewModel _viewModel;

        public AdvancedWindow(MonitorData monitor)
        {
            _viewModel = new AdvancedViewModel(monitor);
            DataContext = _viewModel;
            InitializeComponent();
        }

        private static string BrowseProfiles()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "ICC Profiles|*.icc;*.icm"
            };

            var result = dlg.ShowDialog();

            return result == true ? dlg.FileName : null;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var profilePath = BrowseProfiles();
            if (!string.IsNullOrEmpty(profilePath))
            {
                _viewModel.ProfilePath = profilePath;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ApplyChanges();
            DialogResult = true;
        }

        public bool ChangedCalibration => _viewModel.ChangedCalibration;
    }
}