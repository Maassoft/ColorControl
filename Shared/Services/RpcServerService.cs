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

			var method = GetSuitableMethod(service, message.MethodName, message.Arguments);

			if (method == null)
			{
				throw new InvalidOperationException($"No suitable method {message.MethodName} found in service {service.GetType().Name} with {message.Arguments.Length} arguments");
			}

			var convertedArgs = new List<object>();

			var methodParams = method.GetParameters();
			for (var i = 0; i < message.Arguments.Length; i++)
			{
				var arg = message.Arguments[i];
				var methodParam = methodParams[i];

				AddArgument(arg, methodParam.ParameterType, convertedArgs);
			}

			while (convertedArgs.Count < methodParams.Length)
			{
				convertedArgs.Add(Type.Missing);
			}

			Logger.Debug($"Executing RPC: {message.ServiceName}.{message.MethodName}({string.Join(", ", convertedArgs)})");

			var result = method.Invoke(service, convertedArgs.ToArray());

			if (result is Task task)
			{
				await task;

				result = task.GetType().GetProperty("Result").GetValue(task);
			}

			var data = JsonConvert.SerializeObject(result);

			return SvcResultMessage.FromResult(data);
		}
		catch (Exception ex)
		{
			Logger.Error(ex, $"ExecuteRpcAsync: {ex.Message}");

			return new SvcResultMessage { ErrorMessage = ex.InnerException?.Message ?? ex.Message, Result = false };
		}
	}

	private void AddArgument(object arg, Type type, List<object> list)
	{
		if (arg != null && arg.GetType() != type)
		{
			if (arg.GetType().Name == "JObject" || arg.GetType().Name == "JArray")
			{
				var jsonString = arg.ToString();
				var obj = jsonString == "{}" ? Type.Missing : JsonConvert.DeserializeObject(jsonString, type);
				list.Add(obj);
				return;
			}
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

			while (convertedArgs.Count < methodParams.Length)
			{
				convertedArgs.Add(Type.Missing);
			}

			Logger.Debug($"Executing RPC-typed: {message.ServiceName}.{message.MethodName}({string.Join(", ", convertedArgs)})");

			var result = method.Invoke(service, convertedArgs.ToArray());

			if (result is Task)
			{
				await (result as Task);
			}

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

	private MethodInfo GetSuitableMethod(object service, string name, object[] arguments)
	{
		var methods = service.GetType().GetMethods().Where(m => m.Name == name).ToArray();

		if (methods.Length == 1)
		{
			return methods[0];
		}

		foreach (var method in methods)
		{
			var convertedArgs = new List<object>();
			var methodParams = method.GetParameters();

			if (methodParams.Length != arguments.Length)
			{
				continue;
			}

			try
			{
				for (var i = 0; i < arguments.Length; i++)
				{
					var arg = arguments[i];
					var methodParam = methodParams[i];

					AddArgument(arg, methodParam.ParameterType, convertedArgs);
				}

				return method;
			}
			catch (Exception)
			{
				continue;
			}
		}

		return null;
	}
}
