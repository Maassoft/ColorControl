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
            && !ni.Description.Contains("Hyper-V")
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
            var targetAddress = ipAddressString != null ? IPAddress.Parse(ipAddressString) : null;

            // Gather every usable IPv4 unicast address across the viable interfaces.
            var candidates = NetworkInterface.GetAllNetworkInterfaces()
                .Where(IsViableWOLInterface)
                .SelectMany(ni => ni.GetIPProperties().UnicastAddresses
                    .Where(uip => uip.Address.AddressFamily == AddressFamily.InterNetwork
                        && !uip.Address.ToString().StartsWith("169.254"))
                    .Select(uip => (ni, uip)))
                .ToList();

            // Preferred path: when the target IP is known, only broadcast on the interface that
            // shares the target's subnet, using that subnet's directed broadcast address. This
            // avoids spraying every NIC (e.g. a VPN adapter) which sources the packet from the
            // wrong local address and never reaches a directly-connected target.
            if (targetAddress != null)
            {
                var match = candidates.FirstOrDefault(c => IsInSameSubnet(c.uip.Address, targetAddress, c.uip.PrefixLength));

                if (match.uip != null)
                {
                    var localAddress = match.uip.Address;
                    var directedBroadcast = GetDirectedBroadcast(localAddress, match.uip.PrefixLength);
                    Logger.Debug($"Sending WOL on subnet-matched interface: {match.ni.Name} ({match.ni.Description}), local address: {localAddress}, directed broadcast: {directedBroadcast}, target: {targetAddress}");

                    BroadcastWol(localAddress, directedBroadcast, data);
                    BroadcastWol(localAddress, IPAddress.Broadcast, data);

                    return true;
                }

                Logger.Debug($"No local interface found on the target's subnet ({targetAddress}); falling back to broadcasting on all interfaces");
            }

            // Fallback: broadcast on every viable interface (target IP unknown, or no local
            // interface shares its subnet).
            foreach (var (ni, uip) in candidates)
            {
                try
                {
                    var localAddress = uip.Address;
                    Logger.Debug($"Broadcast WOL in network: {ni.Name} ({ni.Description}), local address: {localAddress}, destination IP-address: {ipAddressString ?? "broadcast"}");
                    BroadcastWol(localAddress, IPAddress.Broadcast, data);

                    if (uip.PrefixLength is > 0 and < 31)
                    {
                        BroadcastWol(localAddress, GetDirectedBroadcast(localAddress, uip.PrefixLength), data);
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"WakeFunctionToAllNics: while sending to specific network: {ni.Name} ({ni.Description}): {ex.ToLogString()}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"WakeFunctionToAllNics: {ex.ToLogString()}");
        }

        return result;
    }

    private static bool IsInSameSubnet(IPAddress local, IPAddress target, int prefixLength)
    {
        if (local.AddressFamily != AddressFamily.InterNetwork || target.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }
        if (prefixLength < 0 || prefixLength > 32)
        {
            return false;
        }

        var localBytes = local.GetAddressBytes();
        var targetBytes = target.GetAddressBytes();
        var maskBytes = GetMaskBytes(prefixLength);

        for (var i = 0; i < 4; i++)
        {
            if ((localBytes[i] & maskBytes[i]) != (targetBytes[i] & maskBytes[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static IPAddress GetDirectedBroadcast(IPAddress local, int prefixLength)
    {
        var localBytes = local.GetAddressBytes();
        var maskBytes = GetMaskBytes(prefixLength);
        var broadcastBytes = new byte[4];

        for (var i = 0; i < 4; i++)
        {
            broadcastBytes[i] = (byte)(localBytes[i] | (byte)~maskBytes[i]);
        }

        return new IPAddress(broadcastBytes);
    }

    private static byte[] GetMaskBytes(int prefixLength)
    {
        // Guard prefixLength 0 explicitly: shifting a uint by 32 is undefined in C#.
        var mask = prefixLength == 0 ? 0u : uint.MaxValue << (32 - prefixLength);

        return
        [
            (byte)(mask >> 24),
            (byte)(mask >> 16),
            (byte)(mask >> 8),
            (byte)mask
        ];
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
        using var udpc = new UdpClient(new IPEndPoint(localAddress, 0));
        udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        var target = new IPEndPoint(broadCastAddress, DefaultWolPort);
        udpc.Send(data, data.Length, target);
    }

    private static void SendWolToIpAddress(IPAddress localAddress, IPAddress remoteAddress, byte[] data, PhysicalAddress physicalAddress)
    {
        var netLuid = CreateTransientLocalNetEntry(remoteAddress, physicalAddress);
        try
        {
            using var udpc = new UdpClient(new IPEndPoint(localAddress, 0));
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