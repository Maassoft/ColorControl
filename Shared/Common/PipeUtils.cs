using ColorControl.Shared.Contracts;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Security.Principal;

namespace ColorControl.Shared.Common;

public static class PipeUtils
{
	public const string ServicePipe = "servicepipe";
	public const string ElevatedPipe = "elevatedpipe";
	public const string MainPipe = "mainpipe";

	public const int DefaultTimeout = 5000;

	private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

	public static async Task<string> SendMessageAsync(string message, int timeout = DefaultTimeout, string pipeName = ServicePipe)
	{
		var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
		try
		{
			await pipeClient.ConnectAsync(timeout);
			try
			{
				var ss = new StreamString(pipeClient);

				await ss.WriteStringAsync(message);

				var result = await ss.ReadStringAsync();

				return result;
			}
			finally
			{
				pipeClient.Close();
			}
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "PipeUtils.SendMessage");

			return null;
		}
	}

	public static string SendMessage(string message, int timeout = 5000, string pipeName = ServicePipe)
	{
		var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
		try
		{
			pipeClient.Connect(timeout);
			try
			{
				var ss = new StreamString(pipeClient);

				ss.WriteString(message);

				var result = ss.ReadString();

				return result;
			}
			finally
			{
				pipeClient.Close();
			}
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "PipeUtils.SendMessage");

			return null;
		}
	}

	public static async Task<SvcResultMessage> SendMessageAsync(SvcMessage message, int timeout = DefaultTimeout)
	{
		var messageJson = JsonConvert.SerializeObject(message);

		var resultJson = await SendMessageAsync(messageJson, timeout);

		if (resultJson == null)
		{
			return null;
		}

		var resultMessage = JsonConvert.DeserializeObject<SvcResultMessage>(resultJson);

		return resultMessage;
	}

	public static SvcResultMessage SendMessage(SvcMessage message, int timeout = DefaultTimeout)
	{
		var messageJson = JsonConvert.SerializeObject(message);

		var resultJson = SendMessage(messageJson, timeout);

		if (resultJson == null)
		{
			return null;
		}

		var resultMessage = JsonConvert.DeserializeObject<SvcResultMessage>(resultJson);

		return resultMessage;
	}

	public static async Task<SvcResultMessage> SendMessageAsync(SvcMessageType messageType)
	{
		var message = new SvcMessage { MessageType = messageType };

		return await SendMessageAsync(message);
	}

	public static T SendRpcMessage<T>(SvcRpcMessage message, int timeout = DefaultTimeout, string pipeName = ServicePipe)
	{
		var messageJson = JsonConvert.SerializeObject(message);

		var resultJson = SendMessage(messageJson, timeout, pipeName);

		if (resultJson == null)
		{
			return default;
		}

		var resultMessage = JsonConvert.DeserializeObject<SvcResultMessage>(resultJson);

		if (resultMessage?.Data == null)
		{
			return default;
		}

		var result = JsonConvert.DeserializeObject<T>(resultMessage.Data);

		return result;
	}
}
