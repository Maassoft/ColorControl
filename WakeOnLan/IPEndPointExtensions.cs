using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace System.Net
{
    /// <summary>Provides extension methods for sending Wake On LAN signals (magic packets) to a specific <see cref="IPEndPoint"/>.</summary>
    public static class IPEndPointExtensions
    {
        #region Wol

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array macAddress is not 6.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        public static void SendWol(this IPEndPoint target, byte[] macAddress)
        {
            Net.MagicPacket.Send(target, macAddress);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        public static void SendWol(this IPEndPoint target, PhysicalAddress macAddress)
        {
            Net.MagicPacket.Send(target, macAddress);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array macAddress is not 6.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        public static void SendWol(this IPEndPoint target, byte[] macAddress, SecureOnPassword password)
        {
            Net.MagicPacket.Send(target, macAddress, password);
        }

        /// <summary>
        /// Sendet ein Wake-On-LAN-Signal an einen Client.
        /// </summary>
        /// <param name="target">Der Ziel-IPEndPoint.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        public static void SendWol(this IPEndPoint target, PhysicalAddress macAddress, SecureOnPassword password)
        {
            Net.MagicPacket.Send(target, macAddress, password);
        }

        #endregion
        #region TAP

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array macAddress is not 6.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this IPEndPoint target, byte[] macAddress)
        {
            return Net.MagicPacket.SendAsync(target, macAddress);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this IPEndPoint target, PhysicalAddress macAddress)
        {
            return Net.MagicPacket.SendAsync(target, macAddress);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array macAddress is not 6.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this IPEndPoint target, byte[] macAddress, SecureOnPassword password)
        {
            return Net.MagicPacket.SendAsync(target, macAddress, password);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a client.</summary>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="macAddress">The MAC address of the client.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="macAddress"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this IPEndPoint target, PhysicalAddress macAddress, SecureOnPassword password)
        {
            return Net.MagicPacket.SendAsync(target, macAddress, password);
        }
        #endregion
    }
}
