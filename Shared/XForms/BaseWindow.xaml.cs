using System.Windows;

namespace ColorControl.Shared.XForms
{
    public partial class BaseWindow : Window
    {
        public BaseWindow() : base()
        {
        }

        //protected void InitThemeAfterShow()
        //{
        //    var useDarkTheme = DarkModeUtils.UseDarkMode;

        //    var handle = new WindowInteropHelper(this).Handle;

        //    var value = useDarkTheme ? 1 : 0;

        //    // Takes care of title bar
        //    WinApi.DwmSetWindowAttribute(handle, DarkModeUtils.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, 4);
        //}

        //protected void InitTheme()
        //{
        //    if (DarkModeUtils.UseDarkMode)
        //    {
        //        var dict = new ResourceDictionary { Source = new Uri($"pack://application:,,,/Shared;component/Themes/DarkTheme.xaml", UriKind.Absolute) };

        //        if (Application.Current.Resources.MergedDictionaries.Any())
        //        {
        //            Application.Current.Resources.MergedDictionaries[0] = dict;
        //        }
        //        else
        //        {
        //            Application.Current.Resources.MergedDictionaries.Add(dict);
        //        }
        //    }
        //    else
        //    {
        //        Application.Current.Resources.MergedDictionaries.Clear();
        //    }
        //}

    }
}