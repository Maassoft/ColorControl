using ColorControl.Shared.Common;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;

namespace ColorControl.Shared.Contracts;

public class Module
{
    public const string NvModule = "NVIDIA controller";
    public const string AmdModule = "AMD controller";
    public const string LgModule = "LG controller";
    public const string SamsungModule = "Samsung controller";
    public const string GameModule = "Game launcher";

    public bool IsActive { get; set; }
    public string DisplayName { get; set; }
    [JsonIgnore]
    public Func<UserControl> InitAction { get; set; }
}

public class ModuleEx<T> where T : class
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public bool IsActive { get; set; }
    public string DisplayName { get; set; }
    public T ServiceInstance { get; set; }

    public T CreateService(IServiceProvider serviceProvider)
    {
        try
        {
            Logger.Debug($"Initializing {typeof(T).Name}...");

            ServiceInstance = serviceProvider.GetRequiredService<T>();

            Logger.Debug($"Initializing {typeof(T).Name}...Done");

            return ServiceInstance;
        }
        catch (Exception ex)
        {
            Logger.Debug($"Error initializing {typeof(T).Name}. No device(s) or drivers are available: {ex.ToLogString()}");
            return null;
        }
    }
}

