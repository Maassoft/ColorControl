using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;

namespace ColorControl.Shared.Services
{
    public class RpcService
    {
        public string Name { get; set; }

        public T Call<T>(string method, params object[] arguments) where T : class
        {
            var rpcMessage = new SvcRpcMessage
            {
                ServiceName = Name,
                MethodName = method,
                Arguments = arguments
            };

            return PipeUtils.SendRpcMessage<T>(rpcMessage);
        }

    }
}
