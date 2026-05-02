using ColorControl.Shared.Common;
using ColorControl.Shared.XForms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;

namespace ColorControl.XForms;

public partial class BrowserWindow : BaseWindow
{
    private static BrowserWindow _window;
    private readonly GlobalContext _globalContext;

    private string _currentUrl;
    public string Url { get; private set; }

    public BrowserWindow(GlobalContext globalContext)
    {
        InitializeComponent();

        DataContext = this;

        _globalContext = globalContext;

        Title = _globalContext.ApplicationTitleAdmin;

        Width = _globalContext.Config.XFormWidth;
        Height = _globalContext.Config.XFormHeight;
    }

    private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        var processStartInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true,
        };
        System.Diagnostics.Process.Start(processStartInfo);
    }

    public static void CreateAndShow(string url, bool show = true)
    {
        Utils.EnsureApplication();

        var reload = _window != null && _window._currentUrl != url;

        //if (reload && _window != null)
        //{
        //    _window.Close();
        //    _window = null;
        //}

        _window ??= Program.ServiceProvider.GetRequiredService<BrowserWindow>();
        _window.Url = url;
        _window._currentUrl = url;
        if (reload)
        {
            _window.webView.Source = new Uri(url);
        }

        if (show)
        {
            _window.WindowState = WindowState.Normal;
            _window.Show();
            _window.Topmost = true;
            _window.Topmost = false;
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;

        _globalContext.Config.XFormWidth = Width;
        _globalContext.Config.XFormHeight = Height;

        if (_globalContext.Config.MinimizeOnClose)
        {
            Hide();

            return;
        }

        Program.Exit();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }

        base.OnStateChanged(e);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
        var core = _window.webView.CoreWebView2;

        if (core != null && !core.Settings.UserAgent.Contains("ColorControlEmbedded"))
        {
            core.Settings.UserAgent = core.Settings.UserAgent + " (ColorControlEmbedded)";
        }

        if (e.Uri.ToString().Contains(":0/"))
        {
            e.Cancel = true;

            var port = BlazorUiManager.GetCurrentPort(Program.Config);

            var newUrl = e.Uri.Replace(":0/", $":{port}/");

            CreateAndShow(newUrl);
        }
    }

    private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
        if (e.WebErrorStatus > 0)
        {
            var port = BlazorUiManager.GetCurrentPort(Program.Config);

            var newUrl = $"http://localhost:{port}";

            CreateAndShow(newUrl, false);
        }
    }
}