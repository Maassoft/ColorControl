using ColorControl.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;

namespace ColorControl.Shared.Services;

public class RpcServerService
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IServiceProvider _serviceProvider;

    public static List<Type> ServiceTypes = new List<Type>();

    public RpcServerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<SvcResultMessage> ExecuteRpcAsync(SvcRpcMessage message)
    {
        try
        {
            var service = GetServiceInstance(message.ServiceName);

            var method = service.GetType().GetMethod(message.MethodName);

            var convertedArgs = new List<object>();

            var methodParams = method.GetParameters();
            for (var i = 0; i < message.Arguments.Length; i++)
            {
                var arg = message.Arguments[i];
                var methodParam = methodParams[i];

                AddArgument(arg, methodParam.ParameterType, convertedArgs);
            }

            Logger.Debug($"Executing RPC: {message.ServiceName}.{message.MethodName}({string.Join(", ", convertedArgs)})");

            var result = method.Invoke(service, convertedArgs.ToArray());

            var data = JsonConvert.SerializeObject(result);

            return SvcResultMessage.FromResult(data);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"ExecuteRpcAsync: {ex.Message}");

            return new SvcResultMessage { ErrorMessage = ex.Message, Result = false };
        }
    }

    private void AddArgument(object arg, Type type, List<object> list)
    {
        if (arg.GetType() != type)
        {
            if (type.IsEnum)
            {
                // Add enum directly to list and not return as 'object'
                list.Add(Enum.ToObject(type, arg));
                return;
            }

            list.Add(Convert.ChangeType(arg, type));
            return;
        }

        list.Add(arg);
    }

    public async Task<SvcResultMessage> ExecuteRpcTypedAsync(SvcRpcMessageTyped message)
    {
        try
        {
            var service = GetServiceInstance(message.ServiceName);

            var method = service.GetType().GetMethod(message.MethodName);

            var props = message.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var methodParams = method.GetParameters();

            var convertedArgs = new List<object>();
            var paramIndex = 0;

            foreach (var prop in props)
            {
                Logger.Debug($"Prop: {prop.Name}");

                var arg = prop.GetValue(message);
                var methodParam = methodParams[paramIndex];

                AddArgument(arg, methodParam.ParameterType, convertedArgs);

                paramIndex++;
            }

            Logger.Debug($"Executing RPC-typed: {message.ServiceName}.{message.MethodName}({string.Join(", ", convertedArgs)})");

            var result = method.Invoke(service, convertedArgs.ToArray());

            var data = JsonConvert.SerializeObject(result);

            return SvcResultMessage.FromResult(data);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"ExecuteRpcTypedAsync: {ex.Message}");

            return new SvcResultMessage { ErrorMessage = ex.Message, Result = false };
        }
    }

    private object GetServiceInstance(string name)
    {
        var type = Type.GetType($"ColorControl.Shared.Services.{name}");

        type ??= Type.GetType($"ColorControl.Services.NVIDIA.{name}");

        type ??= ServiceTypes.FirstOrDefault(t => t.Name == name);

        if (type == null)
        {
            throw new InvalidOperationException($"Type not found: {name}");
        }

        var service = _serviceProvider.GetRequiredService(type);

        return service;
    }
}
