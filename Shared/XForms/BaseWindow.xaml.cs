using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using System.Windows;
using System.Windows.Interop;

namespace ColorControl.Shared.XForms
{
    public partial class BaseWindow : Window
    {
        public BaseWindow() : base()
        {
            ContentRendered += ContenRenderedHandler;
        }

        private void ContenRenderedHandler(object sender, EventArgs e)
        {
            InitThemeAfterShow();
        }

        protected void InitThemeAfterShow()
        {
            var useDarkTheme = DarkModeUtils.UseDarkMode;

            var handle = new WindowInteropHelper(this).Handle;

            var value = useDarkTheme ? 1 : 0;

            // Takes care of title bar
            WinApi.DwmSetWindowAttribute(handle, DarkModeUtils.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, 4);
        }
    }
}