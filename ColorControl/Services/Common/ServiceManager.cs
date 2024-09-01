using ColorControl.Services.AMD;
using ColorControl.Services.GameLauncher;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using ColorControl.Shared.Common;
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
    public Dictionary<string, IServiceBase> Services { get; } = new();

    internal NvService NvService { get; set; }
    internal LgService LgService { get; set; }
    internal AmdService AmdService { get; set; }
    internal GameService GameService { get; set; }
    internal SamsungService SamsungService { get; set; }

    public ServiceManager(WinApiAdminService winApiAdminService, IServiceProvider serviceProvider, GlobalContext globalContext)
    {
        _winApiAdminService = winApiAdminService;
        _serviceProvider = serviceProvider;

        _config = globalContext.Config;
    }

    public void LoadModules(string displayName = null)
    {
        if (displayName == null || displayName == Module.NvModule)
        {
            NvService = AddModule<NvService>(Module.NvModule);
        }

        if (displayName == null || displayName == Module.AmdModule)
        {
            AmdService = AddModule<AmdService>(Module.AmdModule);
        }

        if (displayName == null || displayName == Module.LgModule)
        {
            LgService = AddModule<LgService>(Module.LgModule);
        }

        if (displayName == null || displayName == Module.SamsungModule)
        {
            SamsungService = AddModule<SamsungService>(Module.SamsungModule);
        }

        if (displayName == null || displayName == Module.GameModule)
        {
            GameService = AddModule<GameService>(Module.GameModule);
        }

        if (displayName == null)
        {
            var names = Modules.Select(m => m.Key).ToList();
            _config.Modules = _config.Modules.OrderBy(m => names.IndexOf(m.DisplayName)).ToList();
        }
    }

    private T AddModule<T>(string displayName) where T : class, IServiceBase
    {
        ModuleEx<T> moduleEx;

        if (Modules.TryGetValue(displayName, out var moduleObject))
        {
            moduleEx = moduleObject as ModuleEx<T>;
        }
        else
        {
            moduleEx = new ModuleEx<T> { DisplayName = displayName };
            Modules.Add(displayName, moduleEx);
        }

        var existingModule = _config.Modules.FirstOrDefault(m => m.DisplayName == displayName);

        if (existingModule == null)
        {
            existingModule = new Module { DisplayName = displayName, IsActive = true };
            _config.Modules.Add(existingModule);
        }

        if (existingModule.IsActive)
        {
            var service = moduleEx.CreateService(_serviceProvider);

            if (service != null)
            {
                service.InstallEventHandlers();
                Services.Add(displayName, service);
            }

            return service;
        }

        return default;
    }

    public bool ActivateModule(Module module)
    {
        if (!module.IsActive)
        {
            return true;
        }

        LoadModules(module.DisplayName);

        return Services.ContainsKey(module.DisplayName);
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
        else if (serviceName.Equals("StartCmd", StringComparison.OrdinalIgnoreCase))
        {
            _winApiAdminService.StartProcess(parameters[0], parameters.Length > 1 ? string.Join(" ", parameters.Skip(1)) : null, wait: true, useShellExecute: false);

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

        Utils.WriteObject(Program.ConfigFilename, _config);
    }

    internal List<string> GetServiceInfo(string displayName)
    {
        if (Services.TryGetValue(displayName, out var service))
        {
            return service.GetInfo();
        }

        return new List<string>();
    }
}
