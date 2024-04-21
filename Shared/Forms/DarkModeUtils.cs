using ColorControl.Shared.Native;
using NWin32;

namespace ColorControl.Shared.Forms
{
    public static class DarkModeUtils
    {
        public static bool UseDarkMode = false;

        //private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static Dictionary<string, Tuple<Color, Color, Color, Color>> _defaultColors = new Dictionary<string, Tuple<Color, Color, Color, Color>>();
        private static ToolStripRenderer _defaultRenderer;
        private static Type[] _windowColorTypes = new[] { typeof(GroupBox), typeof(TabPage), typeof(UserControl) };
        public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        public static readonly Color ListViewDarkModeBackColor = Color.FromArgb(80, 80, 80);

        public class DarkModeToolStripRenderer : ToolStripProfessionalRenderer
        {
            public DarkModeToolStripRenderer() : base(new DarkModeProfessionalColors())
            {

            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                var tsMenuItem = e.Item as ToolStripMenuItem;
                if (tsMenuItem != null)
                    e.ArrowColor = Color.White;
                base.OnRenderArrow(e);
            }
        }

        public class DarkModeProfessionalColors : ProfessionalColorTable
        {
            public override Color CheckBackground
            { get { return FormUtils.DarkModeBackColor; } }
            public override Color CheckSelectedBackground
            { get { return Color.FromArgb(16, 32, 49); } }
            public override Color MenuItemSelectedGradientBegin
            { get { return Color.FromArgb(16, 32, 49); } }
            public override Color MenuItemSelectedGradientEnd
            { get { return Color.FromArgb(16, 32, 49); } }
            public override Color SeparatorDark
            { get { return Color.DimGray; } }
            public override Color SeparatorLight
            { get { return Color.DarkGray; } }
            public override Color ToolStripDropDownBackground
            { get { return FormUtils.DarkModeBackColor; } }
            public override Color ImageMarginGradientBegin
            { get { return Color.FromArgb(80, 80, 80); } }
            public override Color ImageMarginGradientMiddle
            { get { return Color.FromArgb(40, 40, 40); } }
            public override Color ImageMarginGradientEnd
            { get { return FormUtils.DarkModeBackColor; } }
        }

        public static void InitWpfTheme()
        {
            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application();
            }

            var resources = System.Windows.Application.Current.Resources;

            if (DarkModeUtils.UseDarkMode)
            {
                var dict = new System.Windows.ResourceDictionary { Source = new Uri($"pack://application:,,,/Shared;component/Themes/DarkTheme.xaml", UriKind.Absolute) };

                if (resources.MergedDictionaries.Any())
                {
                    resources.MergedDictionaries[0] = dict;
                }
                else
                {
                    resources.MergedDictionaries.Add(dict);
                }
            }
            else
            {
                resources.MergedDictionaries.Clear();
            }
        }

        public static void SetDarkMode(bool darkMode)
        {
            UseDarkMode = darkMode;

            // Takes care of system context menu
            WinApi.SetPreferredAppMode(darkMode ? 1 : 0);

            var backColor = darkMode ? FormUtils.DarkModeBackColor : SystemColors.Control;
            var foreColor = darkMode ? FormUtils.DarkModeForeColor : SystemColors.ControlText;

            FormUtils.CurrentBackColor = backColor;
            FormUtils.CurrentForeColor = foreColor;

            if (darkMode)
            {
                _defaultRenderer ??= ToolStripManager.Renderer;

                ToolStripManager.Renderer = new DarkModeToolStripRenderer();
            }
            else if (_defaultRenderer != null)
            {
                ToolStripManager.Renderer = _defaultRenderer;
            }

            FormUtils.MenuItemForeColor = FormUtils.CurrentForeColor;

            WinApi.FlushMenuThemes();
        }

        public static void UpdateTheme(this Form form, bool? useDarkMode = null, bool onlyIfDark = false)
        {
            var toDark = useDarkMode ?? UseDarkMode;

            if (onlyIfDark && !toDark)
            {
                return;
            }

            SetDarkMode(toDark);

            form.SuspendLayout();

            try
            {
                //var settings = new UISettings();

                //var foregroundColorValue = settings.GetColorValue(UIColorType.Foreground);
                //var backroundColorValue = settings.GetColorValue(UIColorType.Background);
                //var isDarkMode = FormUtils.IsColorLight(foregroundColorValue);

                SetControlTheme(form, FormUtils.CurrentBackColor, FormUtils.CurrentForeColor);

                var value = toDark ? 1 : 0;

                // Takes care of title bar
                WinApi.DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, 4);
            }
            finally
            {
                form.ResumeLayout();
                form.Refresh();
            }
        }

        public static void SetControlTheme(Control control)
        {
            SetControlTheme(control, FormUtils.CurrentBackColor, FormUtils.CurrentForeColor);
        }

        public static void SetControlTheme(Control control, Color backColor, Color foreColor)
        {
            var controlName = control.GetHashCode().ToString();

            var controlBackColor = backColor;
            var controlForeColor = foreColor;

            var toDark = backColor != SystemColors.Control;

            if (!toDark)
            {
                if (_defaultColors.TryGetValue(controlName, out var newBackColors))
                {
                    controlBackColor = control.Enabled ? newBackColors.Item1 : newBackColors.Item2;
                    controlForeColor = control.Enabled ? newBackColors.Item3 : newBackColors.Item4;
                }
                else
                {
                    controlBackColor = SystemColors.Window;
                }
            }
            else
            {
                if (!_defaultColors.ContainsKey(controlName))
                {
                    var isEnabled = control.Enabled;
                    if (!isEnabled)
                    {
                        control.Enabled = true;
                    }

                    var enabledBackColor = _windowColorTypes.Contains(control.GetType()) ? SystemColors.Window : control.BackColor;
                    var enabledForeColor = control.ForeColor;

                    control.Enabled = false;

                    var disabledBackColor = control.BackColor;
                    var disabledForeColor = control.ForeColor;

                    if (isEnabled)
                    {
                        control.Enabled = true;
                    }

                    var tuple = new Tuple<Color, Color, Color, Color>(enabledBackColor, disabledBackColor, enabledForeColor, disabledForeColor);

                    _defaultColors.Add(controlName, tuple);
                }
            }

            for (var i = 0; i < control.Controls.Count; i++)
            {
                var childControl = control.Controls[i];

                SetControlTheme(childControl, backColor, foreColor);
            }

            WinApi.AllowDarkModeForWindow(control.Handle, toDark);
            WinApi.SetWindowTheme(control.Handle, toDark ? "DarkMode_Explorer" : "Explorer", null);
            NativeMethods.SendMessageW(control.Handle, NativeConstants.WM_THEMECHANGED, 0, 0);

            control.BackColor = controlBackColor;
            control.ForeColor = controlForeColor;

            SetContextMenuForeColor(control.ContextMenuStrip, controlForeColor);
        }

        public static void SetContextMenuForeColor(ContextMenuStrip menu, Color color)
        {
            if (menu == null)
            {
                return;
            }

            menu.ForeColor = color;

            foreach (var item in menu.Items.OfType<ToolStripMenuItem>())
            {
                SetMenuForeColor(item, color);
            }
        }

        public static void SetMenuForeColor(ToolStripMenuItem menu, Color color)
        {
            menu.ForeColor = color;

            foreach (var item in menu.DropDownItems.OfType<ToolStripMenuItem>())
            {
                SetMenuForeColor(item, color);
            }
        }

        public static bool IsColorLight(Windows.UI.Color color)
        {
            return ((5 * color.G) + (2 * color.R) + color.B) > (8 * 128);
        }
    }
}
