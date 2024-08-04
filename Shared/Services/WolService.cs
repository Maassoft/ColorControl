using ColorControl.Shared.Common;
using NLog;
using NStandard;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Vanara.PInvoke;
using static Vanara.PInvoke.IpHlpApi;

namespace ColorControl.Shared.Services;

public class WolService
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private const string ArgumentExceptionInvalidMacAddressLength = "Invalid MAC address length.";
    private const string ArgumentExceptionInvalidPasswordLength = "Invalid password length.";
    private const int DefaultWolPort = 0;

    private readonly WinApiService _winApiService;
    private readonly RpcClientService _rpcService;

    public WolService(WinApiService winApiService, RpcClientService rpcService)
    {
        _winApiService = winApiService;
        _rpcService = rpcService;
        _rpcService.Name = nameof(WolService);
    }

    public bool SendWol(string macAddress, string ipAddress = null)
    {
        if (!_winApiService.IsAdministrator())
        {
            if (_rpcService.Call<bool>("SendWol", macAddress, ipAddress))
            {
                return true;
            }

            // If RPC failed, execute local WOL anyway
        }

        return WakeFunctionToAllNics(macAddress, ipAddress);
    }

    private bool IsViableWOLInterface(NetworkInterface ni) {
        return ni.NetworkInterfaceType != NetworkInterfaceType.Loopback
            && !ni.Name.Contains("Hyper-V")
            && ni.SupportsMulticast
            && ni.GetIPProperties().GetIPv4Properties != null;
    } 

    private bool WakeFunctionToAllNics(string macAddress, string ipAddressString)
    {
        var result = false;
        Logger.Debug($"Sending WOL packet to MAC-address {macAddress}");

        try
        {
            var physicalAddress = PhysicalAddress.Parse(macAddress);
            var addressBytes = physicalAddress.GetAddressBytes();
            var data = GetWolPacket(addressBytes);
            var ipAddress = ipAddressString != null ? IPAddress.Parse(ipAddressString) : null;

            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            interfaces
                .Where(IsViableWOLInterface)
                .Each(ni =>
                {
                    foreach (var uip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (uip.Address.ToString().StartsWith("169.254") || uip.Address.AddressFamily != AddressFamily.InterNetwork)
                        {
                            continue;
                        }
                        try
                        {
                            Logger.Debug($"Broadcast WOL in network: {ni.Name} ({ni.Description}), local address: {uip.Address}, destination IP-address: {ipAddressString ?? "broadcast"}");
                            BroadcastWol(uip.Address, IPAddress.Broadcast, data);

                            var parts = uip.Address.ToString().Split(".").ToList();
                            parts[3] = "0";
                            var broadcastAddress = IPAddress.Parse(string.Join(".", parts));
                            BroadcastWol(uip.Address, broadcastAddress, data);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"WakeFunctionToAllNics: while sending to specific network: {ni.Name} ({ni.Description}): {ex.ToLogString()}");
                        }
                    }
                });
        }
        catch (Exception ex)
        {
            Logger.Error($"WakeFunctionToAllNics: {ex.ToLogString()}");
        }

        return result;
    }

    /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
    /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="macAddress"/> is not 6.</exception>
    /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="password"/> is not 0 or 6.</exception>
    private static byte[] GetWolPacket(byte[] macAddress)
    {
        if (macAddress == null)
            throw new ArgumentNullException(nameof(macAddress));
        if (macAddress.Length != 6)
            throw new ArgumentException(ArgumentExceptionInvalidMacAddressLength);

        byte[] packet = new byte[102];

        foreach (var i in Enumerable.Range(0, 6)) {
            packet[i] = 0xFF;
        }

        foreach (var i in Enumerable.Range(0, 16)) {
            macAddress.CopyTo(packet, 6 + i * 6);
        }

        return packet;
    }

    private static void BroadcastWol(IPAddress localAddress, IPAddress broadCastAddress, byte[] data)
    {
        //var localEP = new IPEndPoint(localAddress, 0);
        var udpc = new UdpClient();

        udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);

        var target = new IPEndPoint(broadCastAddress, DefaultWolPort);

        udpc.Send(data, data.Length, target);
    }

    private static void SendWolToIpAddress(IPAddress localAddress, IPAddress remoteAddress, byte[] data, PhysicalAddress physicalAddress)
    {
        //var localEP = new IPEndPoint(localAddress, 0);

        var netLuid = CreateTransientLocalNetEntry(remoteAddress, physicalAddress);
        try
        {
            var udpc = new UdpClient();
            udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            var target = new IPEndPoint(remoteAddress, DefaultWolPort);

            udpc.Send(data, data.Length, target);
        }
        finally
        {
            DeleteNetEntry(netLuid, remoteAddress);
        }
    }

    private static NET_LUID CreateTransientLocalNetEntry(IPAddress ipAddress, PhysicalAddress macAddress)
    {
        var ipBytes = ipAddress.GetAddressBytes();

        var address = new Ws2_32.SOCKADDR_INET
        {
            si_family = Ws2_32.ADDRESS_FAMILY.AF_INET,
            Ipv4 = new Ws2_32.SOCKADDR_IN(new Ws2_32.IN_ADDR(ipBytes), DefaultWolPort),
        };

        var net = new NET_LUID();
        var addr = new Ws2_32.SOCKADDR_INET();

        MIB_IPFORWARD_ROW2 forwardRow;
        var result = GetBestRoute2(net, 0, addr, address, 0, out forwardRow, out _);

        if (result != Win32Error.ERROR_SUCCESS)
        {
            Logger.Debug($"Cannot find best route: {result}");

            var luidDummy = new NET_LUID();

            return luidDummy;
        }
        var luid = forwardRow.InterfaceLuid;

        var macAddressBytes = macAddress.GetAddressBytes();
        var bytes = new byte[32];

        macAddressBytes.CopyTo(bytes, 0);

        var row = new MIB_IPNET_ROW2
        {
            Address = address,
            InterfaceLuid = luid,
            PhysicalAddress = bytes,
            PhysicalAddressLength = 6
        };

        result = CreateIpNetEntry2(ref row);

        if (result != Win32Error.ERROR_SUCCESS)
        {
            Logger.Debug($"Cannot create ip net entry: {result}");
        }

        return luid;
    }

    private static void DeleteNetEntry(NET_LUID luid, IPAddress ipAddress)
    {
        if (luid.Value == 0)
        {
            return;
        }

        var ipBytes = ipAddress.GetAddressBytes();

        var address = new Ws2_32.SOCKADDR_INET
        {
            si_family = Ws2_32.ADDRESS_FAMILY.AF_INET,
            Ipv4 = new Ws2_32.SOCKADDR_IN(new Ws2_32.IN_ADDR(ipBytes), DefaultWolPort)

        };

        var row = new MIB_IPNET_ROW2
        {
            Address = address,
            InterfaceLuid = luid,
        };

        var result = DeleteIpNetEntry2(ref row);

        if (result != Win32Error.ERROR_SUCCESS)
        {
            Logger.Debug($"Cannot delete ip net entry: {result}");
        }
    }
}