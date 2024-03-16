using ColorControl.Shared.Common;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;

namespace ColorControl.Shared.Contracts;

public class Module
{
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

    public T CreateService(IServiceProvider serviceProvider)
    {
        try
        {
            Logger.Debug($"Initializing {typeof(T).Name}...");

            var service = serviceProvider.GetRequiredService<T>();

            Logger.Debug($"Initializing {typeof(T).Name}...Done");

            return service;
        }
        catch (Exception ex)
        {
            Logger.Debug($"Error initializing {typeof(T).Name}. No device(s) or drivers are available: {ex.ToLogString()}");
            return null;
        }
    }
}

