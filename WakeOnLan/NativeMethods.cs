using System.Runtime.InteropServices;
using System.Security;

namespace System.Net
{
    internal static class NativeMethods
    {
        private const string IphlpApi = "iphlpapi.dll";

        [DllImport(IphlpApi, ExactSpelling = true)]
        [SecurityCritical]
        internal static extern int SendARP(int destinationIp, int sourceIp, byte[] macAddress, ref int physicalAddrLength);
    }
}
