using NLog;
using System;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace ColorControl
{
    class WOL
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private const string ArgumentExceptionInvalidMacAddressLength = "Invalid MAC address length.";
        private const string ArgumentExceptionInvalidPasswordLength = "Invalid password length.";
        private const int DefaultWolPort = 9;

        public static bool WakeFunction(string macAddress)
        {
            return WakeFunctionToAllNics(macAddress);
        }

        public static bool WakeFunctionToAllNics(string macAddress)
        {
            var result = false;
            Logger.Debug($"Sending WOL packet to MAC-address {macAddress}");

            try
            {
                var address = PhysicalAddress.Parse(macAddress);
                var addressBytes = address.GetAddressBytes();
                var data = GetWolPacket(addressBytes);

                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var ni in interfaces)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback || ni.Name.Contains("VirtualBox"))
                    {
                        continue;
                    }
                    Logger.Debug($"Found network: {ni.Name} ({ni.Description}), type: {ni.NetworkInterfaceType}");
                    try
                    {
                        if (/*ni.OperationalStatus == OperationalStatus.Up &&*/ ni.SupportsMulticast && ni.GetIPProperties().GetIPv4Properties() != null)
                        {
                            foreach (var uip in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (uip.Address.ToString().StartsWith("169.254"))
                                {
                                    continue;
                                }
                                if (uip.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    Logger.Debug($"Broadcast WOL in network: {ni.Name} ({ni.Description}), local address: {uip.Address}");
                                    var local = new IPEndPoint(uip.Address, 0);
                                    var udpc = new UdpClient(local);
                                    udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                                    udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                                    var target = new IPEndPoint(IPAddress.Broadcast, DefaultWolPort);
                                    udpc.Send(data, data.Length, target);

                                    result = true;
                                }
                            }
                        }
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

        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="macAddress"/> is not 6.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="password"/> is not 0 or 6.</exception>
        private static byte[] GetWolPacket(byte[] macAddress, byte[] password = null)
        {
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));
            if (macAddress.Length != 6)
                throw new ArgumentException(ArgumentExceptionInvalidMacAddressLength);

            password = password ?? new byte[0];
            if (password.Length != 0 && password.Length != 6)
                throw new ArgumentException(ArgumentExceptionInvalidPasswordLength);

            var packet = new byte[17 * 6 + password.Length];

            int offset, i;
            for (offset = 0; offset < 6; ++offset)
                packet[offset] = 0xFF;

            for (offset = 6; offset < 17 * 6; offset += 6)
                for (i = 0; i < 6; ++i)
                    packet[i + offset] = macAddress[i];

            if (password.Length > 0)
            {
                for (offset = 16 * 6 + 6; offset < (17 * 6 + password.Length); offset += 6)
                    for (i = 0; i < 6; ++i)
                        packet[i + offset] = password[i];
            }
            return packet;
        }
   }
}