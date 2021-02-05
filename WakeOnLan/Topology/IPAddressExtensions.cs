using System.Collections.Generic;
using System.Diagnostics;

namespace System.Net.Topology
{
    /// <summary>Provides extension methods for the <see cref="T:System.Net.IPAddress"/>.</summary>
    public static class IPAddressExtensions
    {
        private const string OnlyIPv4Supported = "Only IPv4 is currently supported";

        /// <summary>Enumerates through the siblings of an <see cref="T:System.Net.IPAddress"/> in a network. Compliant to RFC 950 (2^n-2).</summary>
        /// <param name="address">The address</param>
        /// <param name="mask">The net mask of the network</param>
        public static IEnumerable<IPAddress> GetSiblings(this IPAddress address, NetMask mask)
        {
            return GetSiblings(address, mask, SiblingOptions.ExcludeUnusable);
        }

        /// <summary>Enumerates through the siblings of an <see cref="T:System.Net.IPAddress"/> in a network.</summary>
        /// <param name="address">The address</param>
        /// <param name="mask">The net mask of the network</param>
        /// <param name="options">Options which addresses to include an which not</param>
        public static IEnumerable<IPAddress> GetSiblings(this IPAddress address, NetMask mask, SiblingOptions options)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));
            if (address.AddressFamily != Sockets.AddressFamily.InterNetwork)
                throw new NotSupportedException(OnlyIPv4Supported);

            bool includeSelf = BitHelper.IsOptionSet(options, SiblingOptions.IncludeSelf);
            bool includeBroadcast = BitHelper.IsOptionSet(options, SiblingOptions.IncludeBroadcast);
            bool includeNetworkIdentifier = BitHelper.IsOptionSet(options, SiblingOptions.IncludeNetworkIdentifier);

            bool alreadyReturnedSelf = false;

            var netPrefix = address.GetNetworkPrefix(mask);

            if (includeNetworkIdentifier)
            {
                netPrefix = address.GetNetworkPrefix(mask);
                if (netPrefix.Equals(address))
                    alreadyReturnedSelf = true;
                yield return netPrefix;
            }

            var selfAddressBytes = address.GetAddressBytes();

            var netPrefixBytes = netPrefix.GetAddressBytes();

            int cidr = mask.Cidr;
            uint maxHosts = 0xFFFFFFFF;

            if (cidr > 0)
                maxHosts = (uint)(1 << (8 * NetMask.MaskLength - cidr)) - 1;

            var hostBytes = new byte[NetMask.MaskLength];
            for (int hostPart = 1; hostPart < maxHosts; ++hostPart)
            {
                unchecked
                {
                    hostBytes[0] = (byte)(hostPart >> 24);
                    hostBytes[1] = (byte)(hostPart >> 16);
                    hostBytes[2] = (byte)(hostPart >> 8);
                    hostBytes[3] = (byte)(hostPart >> 0);
                }

                Debug.WriteLine("HostPart: " + hostPart.ToString("X2").PadLeft(8, '0') + " (" + BitConverter.ToString(hostBytes) + ")");

                var nextIpBytes = netPrefixBytes.Or(hostBytes);
                var nextIp = new IPAddress(nextIpBytes);

                if (!alreadyReturnedSelf)
                {
                    if (includeSelf)
                    {
                        if (nextIpBytes[0] == selfAddressBytes[0]
                         && nextIpBytes[1] == selfAddressBytes[1]
                         && nextIpBytes[2] == selfAddressBytes[2]
                         && nextIpBytes[3] == selfAddressBytes[3])
                            alreadyReturnedSelf = true;
                        yield return nextIp;
                    }
                    else if (nextIpBytes[0] != selfAddressBytes[0]
                          || nextIpBytes[1] != selfAddressBytes[1]
                          || nextIpBytes[2] != selfAddressBytes[2]
                          || nextIpBytes[3] != selfAddressBytes[3])
                        yield return nextIp;
                }
                else
                    yield return nextIp;
            }

            if (includeBroadcast)
            {
                var broadcastAddress = address.GetBroadcastAddress(mask);
                if (!address.Equals(broadcastAddress) || (address.Equals(broadcastAddress) && !alreadyReturnedSelf))
                    yield return broadcastAddress;
            }
        }

        /// <summary>Gets the network prefix of an <see cref="T:System.Net.IPAddress"/>.</summary>
        /// <param name="address">The address</param>
        /// <param name="mask">The net mask of the network</param>
        /// <returns>The network prefix of an <see cref="T:System.Net.IPAddress"/></returns>
        public static IPAddress GetNetworkPrefix(this IPAddress address, NetMask mask)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));

            if (address.AddressFamily != Sockets.AddressFamily.InterNetwork)
                throw new NotSupportedException(OnlyIPv4Supported);

            return mask & address;
        }

        /// <summary>Gets the broadcast address of an <see cref="T:System.Net.IPAddress"/>.</summary>
        /// <param name="address">The address</param>
        /// <param name="mask">The net mask of the network</param>
        /// <returns>The broadcast address of an <see cref="T:System.Net.IPAddress"/></returns>
        public static IPAddress GetBroadcastAddress(this IPAddress address, NetMask mask)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (mask == null)
                throw new ArgumentNullException("mask");

            if (address.AddressFamily != Sockets.AddressFamily.InterNetwork)
                throw new NotSupportedException(OnlyIPv4Supported);

            // TODO: Test

            var ipBytes = address.GetAddressBytes();
            var notMaskBytes = mask.GetMaskBytes().Not();

            var broadcastAddressBytes = notMaskBytes.Or(ipBytes);
            return new IPAddress(broadcastAddressBytes);
        }

        /// <summary>Gets the host identifier (rest) an <see cref="T:System.Net.IPAddress"/>.</summary>
        /// <param name="address">The address</param>
        /// <param name="mask">The net mask of the network</param>
        /// <returns>The host identifier (rest) an <see cref="T:System.Net.IPAddress"/></returns>
        public static IPAddress GetHostIdentifier(this IPAddress address, NetMask mask)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));
            if (address.AddressFamily != Sockets.AddressFamily.InterNetwork)
                throw new NotSupportedException(OnlyIPv4Supported);

            var maskBits = mask.GetMaskBytes();
            var ipBits = address.GetAddressBytes();

            // ~Mask & IP
            var retVal = maskBits.Not().And(ipBits);
            var bytes = new byte[NetMask.MaskLength];
            Buffer.BlockCopy(retVal, 0, bytes, 0, bytes.Length);

            return new IPAddress(bytes);
        }
    }
}
