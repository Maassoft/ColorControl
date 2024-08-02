using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using System.Diagnostics;

namespace ColorControl.Shared.Services;

public class RpcClientService
{
    private readonly WinElevatedProcessManager _elevatedProcessManager;
    private readonly WinApiService _winApiService;
    private readonly RpcServerService _rpcServerService;

    public string Name { get; set; }

    public RpcClientService(WinElevatedProcessManager elevatedProcessManager, WinApiService winApiService/*, RpcServerService rpcServerService*/)
    {
        _elevatedProcessManager = elevatedProcessManager;
        _winApiService = winApiService;
        //_rpcServerService = rpcServerService;
    }

    public T Call<T>(string method, params object[] arguments)
    {
        if (!_winApiService.IsServiceRunning())
        {
            // Last resort: try via elevated process
            return ExecuteViaElevatedProcess<T>(method, arguments);
        }

        var rpcMessage = new SvcRpcMessage
        {
            ServiceName = Name,
            MethodName = method,
            Arguments = arguments
        };

        //var json = JsonConvert.SerializeObject(rpcMessage);
        //var msg = JsonConvert.DeserializeObject<SvcRpcMessage>(json);
        //var _ = _rpcServerService.ExecuteRpcAsync(msg);
        //return default;

        return PipeUtils.SendRpcMessage<T>(rpcMessage);
    }

    public T Call<T>(SvcRpcMessage message)
    {
        message.ServiceName = Name;

        if (Debugger.IsAttached && !_winApiService.IsServiceRunning())
        {
            //return PipeUtils.SendRpcMessage<T>(message, pipeName: PipeUtils.ElevatedPipe);
            return ExecuteViaElevatedProcess<T>(message);
        }

        if (!_winApiService.IsServiceRunning())
        {
            // Last resort: try via elevated process
            return ExecuteViaElevatedProcess<T>(message);
        }

        return PipeUtils.SendRpcMessage<T>(message);
    }

    private T ExecuteViaElevatedProcess<T>(string method, params object[] arguments)
    {
        return _elevatedProcessManager.ExecuteElevated<T>(method, arguments);
    }

    private T ExecuteViaElevatedProcess<T>(SvcRpcMessage message)
    {
        return _elevatedProcessManager.ExecuteElevated<T>(message);
    }
}
