using ColorControl.Services.AMD;
using ColorControl.Services.GameLauncher;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColorControl.Services.Common;

public class ServiceManager
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly WinApiAdminService _winApiAdminService;
    private readonly IServiceProvider _serviceProvider;
    private readonly Config _config;
    public Dictionary<string, object> Modules { get; } = new();

    internal NvService NvService { get; set; }
    internal LgService LgService { get; set; }
    internal AmdService AmdService { get; set; }
    internal GameService GameService { get; set; }
    internal SamsungService SamsungService { get; set; }

    public ServiceManager(WinApiAdminService winApiAdminService, IServiceProvider serviceProvider, AppContextProvider appContextProvider)
    {
        _winApiAdminService = winApiAdminService;
        _serviceProvider = serviceProvider;

        _config = appContextProvider.GetAppContext().Config;
    }

    public void LoadModules()
    {
        NvService = AddModule<NvService>("NVIDIA controller");
        AmdService = AddModule<AmdService>("AMD controller");
        LgService = AddModule<LgService>("LG controller");
        SamsungService = AddModule<SamsungService>("Samsung controller");
        GameService = AddModule<GameService>("Game launcher");

        var names = Modules.Select(m => m.Key).ToList();
        _config.Modules = _config.Modules.OrderBy(m => names.IndexOf(m.DisplayName)).ToList();
    }

    private T AddModule<T>(string displayName) where T : class, IServiceBase
    {
        var moduleEx = new ModuleEx<T> { DisplayName = displayName };
        Modules.Add(displayName, moduleEx);

        var existingModule = _config.Modules.FirstOrDefault(m => m.DisplayName == displayName);

        if (existingModule == null)
        {
            existingModule = new Module { DisplayName = displayName, IsActive = true };
            _config.Modules.Add(existingModule);
        }

        if (existingModule.IsActive)
        {
            var service = moduleEx.CreateService(_serviceProvider);

            service?.InstallEventHandlers();

            return service;
        }

        return default;
    }

    public async Task<bool> HandleExternalServiceAsync(string serviceName, string[] parameters)
    {
        if (string.IsNullOrEmpty(serviceName) || parameters.Length == 0)
        {
            return false;
        }

        if (NvService != null && serviceName.Equals("NvPreset", StringComparison.OrdinalIgnoreCase))
        {
            return await NvService.ApplyPreset(parameters[0]);
        }
        if (NvService != null && serviceName.Equals("GsyncEnabled", StringComparison.OrdinalIgnoreCase))
        {
            return NvService.IsGsyncEnabled();
        }
        if (AmdService != null && serviceName.Equals("AmdPreset", StringComparison.OrdinalIgnoreCase))
        {
            return await AmdService.ApplyPreset(parameters[0]);
        }
        if (LgService != null && serviceName.Equals("LgPreset", StringComparison.OrdinalIgnoreCase))
        {
            await LgService.ApplyPreset(parameters[0]);

            return true;
        }
        if (SamsungService != null && serviceName.Equals("SamsungPreset", StringComparison.OrdinalIgnoreCase))
        {
            await SamsungService.ApplyPreset(parameters[0]);

            return true;
        }

        if (serviceName.Equals("StartProgram", StringComparison.OrdinalIgnoreCase))
        {
            _winApiAdminService.StartProcess(parameters[0], parameters.Length > 1 ? string.Join(" ", parameters.Skip(1)) : null, setWorkingDir: true);

            return true;
        }

        return false;
    }

    public void Save()
    {
        NvService?.GlobalSave();
        AmdService?.GlobalSave();
        LgService?.GlobalSave();
        GameService?.GlobalSave();
        SamsungService?.GlobalSave();
    }
}
