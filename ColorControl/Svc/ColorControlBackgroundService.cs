using ColorControl.Services.LG;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Services;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Svc
{
    public sealed class ColorControlBackgroundService : BackgroundService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SessionSwitchDispatcher _sessionSwitchDispatcher;
        private readonly RpcServerService _rpcServerService;
        private readonly WinApiAdminService _winApiAdminService;
        private readonly WolService _wolService;
        private static string LgConfigFile = "LgConfig.json";

        public string PipeName { get; set; } = PipeUtils.ServicePipe;

        public ColorControlBackgroundService(SessionSwitchDispatcher sessionSwitchDispatcher, RpcServerService rpcServerService, WinApiAdminService winApiAdminService, WolService wolService)
        {
            _sessionSwitchDispatcher = sessionSwitchDispatcher;
            _rpcServerService = rpcServerService;
            _winApiAdminService = winApiAdminService;
            _wolService = wolService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                LgConfigFile = Path.Combine(Program.DataDir, LgConfigFile);

                //var lgService = new LgService(Program.DataDir, true);
                //lgService.InstallEventHandlers(true);

                if (PipeName == PipeUtils.ServicePipe)
                {
                    await WakeDevicesAsync();
                }

                var securityIdentifier = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);

                var ps = new PipeSecurity();
                ps.AddAccessRule(new PipeAccessRule(securityIdentifier, PipeAccessRights.ReadWrite, AccessControlType.Allow));

                Logger.Debug("EXECUTING BACKGROUND");

                while (!stoppingToken.IsCancellationRequested)
                {
                    Logger.Debug("WAITING FOR MESSAGE...");

                    var pipeServer = NamedPipeServerStreamAcl.Create(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, ps);

                    await pipeServer.WaitForConnectionAsync(stoppingToken);

                    var ss = new StreamString(pipeServer);

                    var message = ss.ReadString();

                    //Logger.Debug("MESSAGE RECEIVED: " + message);

                    try
                    {
                        var result = await HandleMessageAsync(message);

                        var resultJson = JsonConvert.SerializeObject(result);

                        //Logger.Debug("SENDING RESULT: " + resultJson);

                        ss.WriteString(resultJson);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Error while handling message: {message}");

                        var result = SvcResultMessage.FromResult(false, ex.Message);
                        var resultJson = JsonConvert.SerializeObject(result);
                        ss.WriteString(resultJson);
                    }

                    pipeServer.Close();
                    //pipeServer.Disconnect();
                }
                Logger.Debug("STOPPING...");
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("Service stopping");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                //Environment.Exit(1);
            }
        }

        private async Task<SvcResultMessage> HandleMessageAsync(string json)
        {
            var message = JsonConvert.DeserializeObject<SvcMessage>(json);

            var result = message.MessageType switch
            {
                SvcMessageType.SetLgConfig => HandleSetLgConfigMessage(message),
                SvcMessageType.GetLog => HandleGetLogMessage(message),
                SvcMessageType.ClearLog => HandleClearLogMessage(message),
                SvcMessageType.ExecuteRpcGeneric => await _rpcServerService.ExecuteRpcAsync(JsonConvert.DeserializeObject<SvcRpcMessage>(json)),
                SvcMessageType.ExecuteRpcGenericTyped => await HandleRpcTypedAsync(JsonConvert.DeserializeObject<SvcRpcMessageTyped>(json), json),
                SvcMessageType.ExecuteUpdate => await HandleExecuteUpdateCommandAsync(JsonConvert.DeserializeObject<SvcInstallUpdateMessage>(json)),
                SvcMessageType.RestartAfterUpdate => HandleRestartAfterUpdateMessage(),
                _ => SvcResultMessage.FromResult(false, "Unexpected message type")
            };

            return result;
        }

        private async Task<SvcResultMessage> HandleRpcTypedAsync(SvcRpcMessageTyped message, string json)
        {
            var typeName = message.TypeName;

            Logger.Debug($"TypeName: {typeName}");

            var type = Type.GetType($"ColorControl.Shared.Contracts.{typeName}, Shared");

            Logger.Debug($"Type: {type?.Name}");

            var typedMessage = JsonConvert.DeserializeObject(json, type) as SvcRpcMessageTyped;

            Logger.Debug($"Msg: {typedMessage?.MethodName}");

            return await _rpcServerService.ExecuteRpcTypedAsync(typedMessage);
        }

        private SvcResultMessage HandleSetLgConfigMessage(SvcMessage message)
        {
            var data = message.Data;

            Utils.WriteText(LgConfigFile, data);

            return SvcResultMessage.FromResult(true);
        }

        private SvcResultMessage HandleGetLogMessage(SvcMessage _)
        {
            var logFile = Program.LogFilename;

            var data = Utils.ReadText(logFile);

            return SvcResultMessage.FromResult(data);
        }

        private SvcResultMessage HandleClearLogMessage(SvcMessage _)
        {
            var logFile = Program.LogFilename;

            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            return SvcResultMessage.FromResult(true);
        }

        private async Task<SvcResultMessage> HandleExecuteUpdateCommandAsync(SvcInstallUpdateMessage message)
        {
            var localFile = Path.Combine(Program.DataDir, "update.zip");

            await Utils.DownloadFileAsync(message.DownloadUrl, localFile);

            var updatePath = Path.Combine(Program.DataDir, "ColorControl");

            var directory = new DirectoryInfo(updatePath);
            if (directory.Exists)
            {
                directory.Delete(true);
            }

            Utils.UnZipFile(localFile, Program.DataDir);

            Utils.UpdateFiles(message.ClientPath, updatePath);

            return SvcResultMessage.FromResult(true);
        }

        private SvcResultMessage HandleRestartAfterUpdateMessage()
        {
            _winApiAdminService.StartProcess("cmd.exe", $@"/C net stop ""{Utils.SERVICE_NAME}"" && net start ""{Utils.SERVICE_NAME}""", true);

            return SvcResultMessage.FromResult(true);
        }

        private async Task WakeDevicesAsync()
        {
            if (!_sessionSwitchDispatcher.UserLocalSession)
            {
                return;
            }

            if (!File.Exists(LgConfigFile))
            {
                return;
            }

            var config = Utils.DeserializeJson<LgServiceConfig>(LgConfigFile);

            if (config == null)
            {
                return;
            }

            var wakeDevices = config.Devices.Where(d => d.PowerOnAfterStartup).ToList();

            foreach (var wakeDevice in wakeDevices)
            {
                for (var i = 0; i < config.PowerOnRetries; i++)
                {
                    _wolService.SendWol(wakeDevice.MacAddress, wakeDevice.IpAddress);

                    await Task.Delay(1000);

                    var result = await Utils.PingHost(wakeDevice.IpAddress);

                    if (result)
                    {
                        Logger.Debug($"Device {wakeDevice.Name} is pingable, stopping WOL");

                        break;
                    }
                }
            }
        }

        internal async Task StartAndStopWithMutex()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var _ = Task.Run(() => CheckMutexAsync(cancellationTokenSource));

            await StartAsync(cancellationToken);
        }

        private void CheckMutexAsync(CancellationTokenSource cancellationTokenSource)
        {
            var mutex = new Mutex(false, GlobalContext.CurrentContext.MutexId, out var _);
            try
            {
                mutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }

            cancellationTokenSource.Cancel();

            Environment.Exit(0);
        }
    }
}
