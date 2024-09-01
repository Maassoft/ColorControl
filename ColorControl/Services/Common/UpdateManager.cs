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
    private readonly GlobalContext _globalContext;
    private readonly ServiceManager _serviceManager;
    private readonly NotifyIconManager _notifyIconManager;
    private readonly WinApiService _winApiService;
    private readonly WinApiAdminService _winApiAdminService;
    private readonly Config _config;

    private string _downloadUrl;

    public UpdateManager(GlobalContext globalContext, ServiceManager serviceManager, NotifyIconManager notifyIconManager, WinApiService winApiService, WinApiAdminService winApiAdminService)
    {
        _globalContext = globalContext;
        _serviceManager = serviceManager;
        _notifyIconManager = notifyIconManager;
        _winApiService = winApiService;
        _winApiAdminService = winApiAdminService;
        _config = globalContext.Config;
    }

    private void trayIconBalloonTip_Clicked(object sender, EventArgs e)
    {
        if (_globalContext.UpdateInfo?.HtmlUrl != null)
        {
            _winApiAdminService.StartProcess(_globalContext.UpdateInfo?.HtmlUrl);
        }
    }

    public async Task CheckForUpdatesAndInstall()
    {
        var updateInfo = await CheckForUpdates();

        if (!updateInfo.UpdateAvailable)
        {
            return;
        }

        _notifyIconManager.NotifyIcon.BalloonTipClicked += trayIconBalloonTip_Clicked;
        var newVersion = updateInfo.NewVersionNumber;

        if (!string.IsNullOrEmpty(updateInfo.DownloadUrl) && _winApiService.IsServiceRunning())
        {
            if (_config.AutoInstallUpdates)
            {
                await InstallUpdate();

                return;
            }

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

    public async Task<UpdateInfoDto> CheckForUpdates()
    {
        if (_globalContext.UpdateInfo != null)
        {
            return _globalContext.UpdateInfo;
        }

        //if (Debugger.IsAttached)
        //{
        //    return _globalContext.UpdateInfo = new UpdateInfoDto
        //    {
        //        UpdateAvailable = true,
        //        NewVersionNumber = "10.0.0.2",
        //        DownloadUrl = "http://testurl"
        //    };
        //}

        if (Debugger.IsAttached)
        {
            return new UpdateInfoDto();
        }

        var json = await Utils.GetRestJsonAsync("https://api.github.com/repos/maassoft/colorcontrol/releases/latest");

        if (json == null)
        {
            return new UpdateInfoDto();
        }

        _globalContext.UpdateInfo = GetUpdateInfo(json);

        return _globalContext.UpdateInfo;
    }

    private static UpdateInfoDto GetUpdateInfo(dynamic json)
    {
        var updateInfo = new UpdateInfoDto();

        if (json?.tag_name == null)
        {
            return updateInfo;
        }

        var currentVersion = Application.ProductVersion;
        var cvParts = currentVersion.Split(".");

        var newVersion = (string)json.tag_name.Value.Substring(1);
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

        updateInfo.NewVersionNumber = newVersion;
        updateInfo.UpdateAvailable = nvParts.Length != cvParts.Length || CompareVersions();

        updateInfo.HtmlUrl = json.html_url.Value;

        if (json.assets != null)
        {
            var asset = json.assets[0];
            updateInfo.DownloadUrl = asset.browser_download_url.Value;
        }

        return updateInfo;
    }

    public async Task<bool> InstallUpdate()
    {
        if (_globalContext.UpdateInfo?.DownloadUrl == null)
        {
            return false;
        }

        if (Debugger.IsAttached)
        {
            await Task.Delay(5000);
            return true;
        }

        var message = new SvcInstallUpdateMessage
        {
            DownloadUrl = _globalContext.UpdateInfo?.DownloadUrl,
            ClientPath = new FileInfo(Application.ExecutablePath).Directory.FullName
        };

        var result = await PipeUtils.SendMessageAsync(message, 30000);

        if (result != null && !result.Result)
        {
            if (Program.IsMainFormOpened())
            {
                MessageForms.ErrorOk($"Error while updating: {result.ErrorMessage}");
            }

            return false;
        }

        await PipeUtils.SendMessageAsync(SvcMessageType.RestartAfterUpdate);

        if (Program.IsMainFormOpened() && MessageForms.QuestionYesNo("Update installed successfully. Do you want to restart the application?") == DialogResult.Yes)
        {
            Program.Restart();
        }

        return true;
    }

    public async Task<bool> RestartAfterUpdate()
    {
        Program.Restart();

        return true;
    }
}
