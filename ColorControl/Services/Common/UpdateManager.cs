using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.Common;

public class UpdateManager
{
    private readonly AppContextProvider _appContextProvider;
    private readonly ServiceManager _serviceManager;
    private readonly NotifyIconManager _notifyIconManager;
    private readonly WinApiService _winApiService;
    private readonly WinApiAdminService _winApiAdminService;
    private readonly Config _config;

    private bool _checkedForUpdates;
    private string _updateHtmlUrl;
    private string _downloadUrl;

    public UpdateManager(AppContextProvider appContextProvider, ServiceManager serviceManager, NotifyIconManager notifyIconManager, WinApiService winApiService, WinApiAdminService winApiAdminService)
    {
        _appContextProvider = appContextProvider;
        _serviceManager = serviceManager;
        _notifyIconManager = notifyIconManager;
        _winApiService = winApiService;
        _winApiAdminService = winApiAdminService;
        _config = appContextProvider.GetAppContext().Config;
    }

    private void trayIconBalloonTip_Clicked(object sender, EventArgs e)
    {
        if (_updateHtmlUrl != null)
        {
            _winApiAdminService.StartProcess(_updateHtmlUrl);
        }
    }

    public async Task CheckForUpdates()
    {
        if (!_config.CheckForUpdates || _checkedForUpdates || Debugger.IsAttached)
        {
            return;
        }

        _checkedForUpdates = true;

        await Utils.GetRestJsonAsync("https://api.github.com/repos/maassoft/colorcontrol/releases/latest", InitHandleCheckForUpdates);
    }

    private void InitHandleCheckForUpdates(dynamic latest)
    {
        HandleCheckForUpdates(latest);

        //FormUtils.BeginInvokeCheck(() => HandleCheckForUpdates(latest));
    }

    private void HandleCheckForUpdates(dynamic latest)
    {
        if (latest?.tag_name == null)
        {
            return;
        }

        var currentVersion = Application.ProductVersion;
        var cvParts = currentVersion.Split(".");

        var newVersion = (string)latest.tag_name.Value.Substring(1);
        var nvParts = newVersion.Split(".");

        bool CompareVersions()
        {
            var result = true;
            var aNumberIsLarger = false;

            for (var i = 0; i < nvParts.Length; i++)
            {
                var part = Utils.ParseInt(nvParts[i]);
                var cvPart = Utils.ParseInt(cvParts[i]);

                if (part > cvPart)
                {
                    aNumberIsLarger = true;
                    break;
                }

                if (part == cvPart)
                {
                    continue;
                }

                result = false;
                break;
            }

            return result && aNumberIsLarger;
        }

        if (nvParts.Length != cvParts.Length || CompareVersions())
        {
            _notifyIconManager.NotifyIcon.BalloonTipClicked += trayIconBalloonTip_Clicked;

            _updateHtmlUrl = latest.html_url.Value;

            if (latest.assets != null && _winApiService.IsServiceRunning())
            {
                var asset = latest.assets[0];
                _downloadUrl = asset.browser_download_url.Value;

                if (_config.AutoInstallUpdates)
                {
                    InstallUpdate();

                    return;
                }

                _appContextProvider.GetAppContext().UpdateAvailable = true;

                if (_notifyIconManager.NotifyIcon.Visible)
                {
                    _notifyIconManager.NotifyIcon.ShowBalloonTip(5000, "Update available", $"Version {newVersion} is available. Click on the Update-button to update", ToolTipIcon.Info);
                }
                else
                {
                    MessageForms.InfoOk($"New version {newVersion} is available. Click on the Update-button to update", "Update available", $"https://github.com/Maassoft/ColorControl/releases/tag/v{newVersion}");
                }

                return;
            }

            if (_notifyIconManager.NotifyIcon.Visible)
            {
                _notifyIconManager.NotifyIcon.ShowBalloonTip(5000, "Update available", $"Version {newVersion} is available. Click to open the GitHub page", ToolTipIcon.Info);
            }
            else
            {
                MessageForms.InfoOk($"New version {newVersion} is available. Click on the Help-button to open the GitHub page.", "Update available", $"https://github.com/Maassoft/ColorControl/releases/tag/v{newVersion}");
            }
        }
    }

    public void InstallUpdate()
    {
        if (_downloadUrl == null) { return; }

        var message = new SvcInstallUpdateMessage
        {
            DownloadUrl = _downloadUrl,
            ClientPath = new FileInfo(Application.ExecutablePath).Directory.FullName
        };

        var result = PipeUtils.SendMessage(message, 30000);

        if (result != null && !result.Result)
        {
            MessageForms.ErrorOk($"Error while updating: {result.ErrorMessage}");

            return;
        }

        PipeUtils.SendMessage(SvcMessageType.RestartAfterUpdate);

        if (MessageForms.QuestionYesNo("Update installed successfully. Do you want to restart the application?") == DialogResult.Yes)
        {
            Program.Restart();
        }
        else
        {
            _downloadUrl = null;
            _appContextProvider.GetAppContext().UpdateAvailable = true;
        }
    }
}
