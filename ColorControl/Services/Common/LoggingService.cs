using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColorControl.Services.Common;

public class LoggingService
{
    private readonly GlobalContext _globalContext;
    private readonly ElevationService _elevationService;
    private readonly WinApiService _winApiService;

    public LoggingService(GlobalContext globalContext, ElevationService elevationService, WinApiService winApiService)
    {
        _globalContext = globalContext;
        _elevationService = elevationService;
        _winApiService = winApiService;
    }

    public bool SetLogLevel(string logLevelName)
    {
        var logLevel = LogLevel.FromString(logLevelName);
        _globalContext.SetLogLevel(logLevel);

        return true;
    }

    public async Task<List<string>> LoadLog(int type, int maxLines = 1000, int order = 0)
    {
        string logFile;

        var lines = default(List<string>);

        if (type == 0)
        {
            lines = Utils.ReadLines(Program.LogFilename);
        }
        else
        {
            var winApiService = Program.ServiceProvider.GetRequiredService<WinApiService>();

            if (winApiService.IsServiceRunning())
            {
                var result = await PipeUtils.SendMessageAsync(SvcMessageType.GetLog);

                logFile = result?.Data ?? "Cannot get log from service";
            }
            else
            {
                logFile = "The service is not installed or not running";
            }

            lines = logFile.Split("\r\n").ToList();
        }

        lines = lines.Where((s, index) => order == 1 ? index < maxLines : (order == 0 && index >= lines.Count - maxLines)).ToList();

        if (order == 0)
        {
            lines.Reverse();
        }

        return lines;
    }
}