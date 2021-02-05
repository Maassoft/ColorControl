using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace System.Net
{
    /// <summary>Provides extension methods for sending Wake On LAN signals (magic packets) using a specific <see cref="PhysicalAddress"/>.</summary>
    public static class PhysicalAddressExtensions
    {
        #region Wol

        /// <summary>Sends a Wake On LAN signal (magic packet) to the broadcast IP address with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        public static void SendWol(this PhysicalAddress address) => address.SendWol(IPAddress.Broadcast, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP address with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        public static void SendWol(this PhysicalAddress address, IPAddress target) => address.SendWol(target, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP address with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPAddress"/>.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        public static void SendWol(this PhysicalAddress address, IPAddress target, SecureOnPassword password)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            target.SendWol(address.GetAddressBytes(), password);
        }


        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP end point with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        public static void SendWol(this PhysicalAddress address, IPEndPoint target) => address.SendWol(target, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP end point with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        public static void SendWol(this PhysicalAddress address, IPEndPoint target, SecureOnPassword password)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            target.SendWol(address.GetAddressBytes(), password);
        }

        #endregion
        #region TAP

        /// <summary>Sends a Wake On LAN signal (magic packet) to the broadcast IP address with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        public static Task SendWolAsync(this PhysicalAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            return IPAddress.Broadcast.SendWolAsync(address.GetAddressBytes());
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP address with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPAddress"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this PhysicalAddress address, IPAddress target) => address.SendWolAsync(target, null);


        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP address with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPAddress"/>.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this PhysicalAddress address, IPAddress target, SecureOnPassword password)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            return target.SendWolAsync(address.GetAddressBytes(), password);
        }

        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP end point with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this PhysicalAddress address, IPEndPoint target) => address.SendWolAsync(target, null);

        /// <summary>Sends a Wake On LAN signal (magic packet) to a specific IP end point with the physical address.</summary>
        /// <param name="address">The instance of the physical address that should be used in the magic packet.</param>
        /// <param name="target">Destination <see cref="IPEndPoint"/>.</param>
        /// <param name="password">The SecureOn password of the client.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <returns>An asynchronous <see cref="Task"/> which sends a Wake On LAN signal (magic packet) to a client.</returns>
        public static Task SendWolAsync(this PhysicalAddress address, IPEndPoint target, SecureOnPassword password)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            return target.SendWolAsync(address.GetAddressBytes(), password);
        }

        #endregion
        #region common

        /// <summary>Gets the type (<see cref="PhysicalAddressAdministrator"/>) of the <see cref="PhysicalAddress" />.</summary>
        /// <param name="address">The address.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <returns>The <see cref="PhysicalAddressType" /> of the physical addess (MAC address).</returns>
        public static PhysicalAddressType GetAddressType(this PhysicalAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var bytes = address.GetAddressBytes();
            if (bytes == null || bytes.Length < 1)
                throw new ArgumentException($"Invalid {nameof(address)}.");
            return (bytes[0] & 0x1) == 0 ? PhysicalAddressType.Unicast : PhysicalAddressType.Multicast;
        }

        /// <summary>Gets the administrator (<see cref="PhysicalAddressAdministrator"/>) of the <see cref="PhysicalAddress" />.</summary>
        /// <param name="address">The address.</param>
        /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
        /// <returns>The <see cref="PhysicalAddressAdministrator" /> of the physical addess (MAC address).</returns>
        public static PhysicalAddressAdministrator GetAddressAdministrator(this PhysicalAddress address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var bytes = address.GetAddressBytes();
            if (bytes == null || bytes.Length < 1)
                throw new ArgumentException($"Invalid {nameof(address)}.");
            return (bytes[0] & 0x2) == 0 ? PhysicalAddressAdministrator.Global : PhysicalAddressAdministrator.Local;
        }

        #endregion
    }
}
