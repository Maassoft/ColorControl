using ColorControl.Shared.Contracts;
using ColorControl.UI;
using ColorControl.UI.Services;
using System.Diagnostics;

var arguments = args;

if (Debugger.IsAttached)
{
    var rpcServer = new RpcUiClientService(null);
    var options = new RpcUiClientOptions
    {
        Timeout = 10000
    };

    var config = await rpcServer.CallWithOptionsAsync<Config>("OptionsService", "GetConfig", options);
    arguments = ["", config.UseDarkMode.ToString(), config.UiPort.ToString(), config.UiAllowRemoteConnections.ToString()];
}

await Blazor.Start(arguments);
