using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using System.Diagnostics;

namespace ColorControl.Shared.Services;

public class RpcUiClientOptions
{
    public int Timeout { get; set; } = 2000;
}

public class RpcUiClientService
{
    public RpcUiClientService()
    {
    }

    public async Task<T> CallAsync<T>(string serviceName, string method, params object[] arguments)
    {
        var rpcMessage = new SvcRpcMessage
        {
            ServiceName = serviceName,
            MethodName = method,
            Arguments = arguments
        };

        var timeout = Debugger.IsAttached ? 60000 : 2000;

        return await PipeUtils.SendRpcMessageAsync<T>(rpcMessage, timeout, pipeName: PipeUtils.MainPipe);
    }

    public async Task<T> CallWithOptionsAsync<T>(string serviceName, string method, RpcUiClientOptions options, params object[] arguments)
    {
        var rpcMessage = new SvcRpcMessage
        {
            ServiceName = serviceName,
            MethodName = method,
            Arguments = arguments
        };

        return await PipeUtils.SendRpcMessageAsync<T>(rpcMessage, options.Timeout, pipeName: PipeUtils.MainPipe);
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
}
