using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace System.Net
{
    /// <summary>Provides methods for sending Wake On LAN signals (magic packets).</summary>
    public static class MagicPacket
    {
        #region Wol

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <exception cref="ArgumentNullException">target is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array macAddress is not 6.</exception>
        /// <exception cref="SocketException">An error occurred when accessing the socket. See Remarks section of <see cref="UdpClient.Send(byte[], int, IPEndPoint)"/> for more information.</exception>
        public static void Send(IPEndPoint target, byte[] macAddress) => Send(target, macAddress, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="macAddress"/> is not 6.</exception>
        /// <exception cref="SocketException">An error occurred when accessing the socket. See Remarks section of <see cref="UdpClient.Send(byte[], int, IPEndPoint)"/> for more information.</exception>
        public static void Send(IPEndPoint target, byte[] macAddress, SecureOnPassword password)
        {
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));

            byte[] passwordBuffer = password?.GetPasswordBytes();
            byte[] packet = GetWolPacket(macAddress, passwordBuffer);
            SendPacket(target, packet);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="SocketException">An error occurred when accessing the socket. See Remarks section of <see cref="UdpClient.Send(byte[], int, IPEndPoint)"/> for more information.</exception>
        public static void Send(IPEndPoint target, PhysicalAddress macAddress) => Send(target, macAddress, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="SocketException">An error occurred when accessing the socket. See Remarks section of <see cref="UdpClient.Send(byte[], int, IPEndPoint)"/> for more information.</exception>
        public static void Send(IPEndPoint target, PhysicalAddress macAddress, SecureOnPassword password)
        {
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));

            byte[] passwordBuffer = password?.GetPasswordBytes();
            byte[] packet = GetWolPacket(macAddress.GetAddressBytes(), passwordBuffer);
            SendPacket(target, packet);
        }

        private static void SendPacket(IPEndPoint target, byte[] packet)
        {
            using (var cl = new UdpClient())
                cl.Send(packet, packet.Length, target);
        }

        #endregion
        #region TAP

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="Byte" /> array <paramref name="macAddress"/> is not 6.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendAsync(IPEndPoint target, byte[] macAddress) => SendAsync(target, macAddress);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendAsync(IPEndPoint target, byte[] macAddress, SecureOnPassword password)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));

            var passwordBuffer = password?.GetPasswordBytes();
            var packet = GetWolPacket(macAddress, passwordBuffer);
            return SendPacketAsync(target, packet);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendAsync(IPEndPoint target, PhysicalAddress macAddress) => SendAsync(target, macAddress, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the designated client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendAsync(IPEndPoint target, PhysicalAddress macAddress, SecureOnPassword password)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));

            var passwordBuffer = password?.GetPasswordBytes();
            var p = GetWolPacket(macAddress.GetAddressBytes(), passwordBuffer);
            return SendPacketAsync(target, p);
        }

        private static Task SendPacketAsync(IPEndPoint target, byte[] packet)
        {
            var cl = new UdpClient();
            return cl.SendAsync(packet, packet.Length, target).ContinueWith((Task t) => cl.Close());
        }

        #endregion
        #region internal

        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="macAddress"/> is not 6.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array <paramref name="password"/> is not 0 or 6.</exception>
        private static byte[] GetWolPacket(byte[] macAddress, byte[] password)
        {
            if (macAddress == null)
                throw new ArgumentNullException(nameof(macAddress));
            if (macAddress.Length != 6)
                throw new ArgumentException(Localization.ArgumentExceptionInvalidMacAddressLength);

            password = password ?? new byte[0];
            if (password.Length != 0 && password.Length != 6)
                throw new ArgumentException(Localization.ArgumentExceptionInvalidPasswordLength);

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

        #endregion
    }
}
