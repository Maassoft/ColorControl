using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ColorControl.UI.Services;

public class RpcUiClientOptions
{
	public int Timeout { get; set; } = PipeUtils.DefaultTimeout;
}

public class RpcUiClientService(NotificationService notificationService)
{
	public string? LastErrorMessage { get; private set; }

	public async Task<T> CallAsync<T>(string serviceName, string method, params object[] arguments)
	{
		var rpcMessage = new SvcRpcMessage
		{
			ServiceName = serviceName,
			MethodName = method,
			Arguments = arguments
		};

		var timeout = Debugger.IsAttached ? 60000 : PipeUtils.DefaultTimeout;

		return await SendRpcMessageAsync<T>(rpcMessage, timeout, pipeName: PipeUtils.MainPipe);
	}

	public async Task<T> CallWithOptionsAsync<T>(string serviceName, string method, RpcUiClientOptions options, params object[] arguments)
	{
		var rpcMessage = new SvcRpcMessage
		{
			ServiceName = serviceName,
			MethodName = method,
			Arguments = arguments
		};

		return await SendRpcMessageAsync<T>(rpcMessage, options.Timeout, pipeName: PipeUtils.MainPipe);
	}

	public T Call<T>(string serviceName, string method, params object[] arguments)
	{
		var rpcMessage = new SvcRpcMessage
		{
			ServiceName = serviceName,
			MethodName = method,
			Arguments = arguments
		};

		var timeout = Debugger.IsAttached ? 60000 : 2000;

		return PipeUtils.SendRpcMessage<T>(rpcMessage, timeout, pipeName: PipeUtils.MainPipe);
	}

	private async Task<T> SendRpcMessageAsync<T>(SvcRpcMessage message, int timeout = PipeUtils.DefaultTimeout, string pipeName = PipeUtils.ServicePipe)
	{
		var messageJson = JsonConvert.SerializeObject(message);

		var resultJson = await PipeUtils.SendMessageAsync(messageJson, timeout, pipeName);

		if (resultJson == null)
		{
			notificationService.SendNotification(new NotificationDto("Unknown communication error", Constants.Danger));

			return default;
		}

		var resultMessage = JsonConvert.DeserializeObject<SvcResultMessage>(resultJson);

		if (resultMessage?.Data == null)
		{
			LastErrorMessage = resultMessage?.ErrorMessage ?? "Unknown communication error";

			notificationService.SendNotification(new NotificationDto(LastErrorMessage, Constants.Danger));

			return default;
		}

		LastErrorMessage = null;

		var result = JsonConvert.DeserializeObject<T>(resultMessage.Data);

		return result;
	}


}
