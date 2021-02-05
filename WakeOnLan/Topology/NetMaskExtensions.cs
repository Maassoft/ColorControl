
namespace System.Net.Topology
{
    /// <summary>Provides extension methods for the <see cref="T:System.Net.Topology.NetMask"/>.</summary>
    public static class NetMaskExtensions
    {
        /// <summary>Gets the number of siblings an <see cref="T:System.Net.IPAddress"/> can have in a given network. Compliant to RFC 950 (2^n-2).</summary>
        /// <param name="mask">The net mask of the network</param>
        /// <returns>The number of siblings an <see cref="T:System.Net.IPAddress"/> can have in the given network.</returns>
        public static int GetSiblingCount(this NetMask mask) => GetSiblingCount(mask, SiblingOptions.ExcludeUnusable);

        /// <summary>Gets the number of siblings an <see cref="T:System.Net.IPAddress"/> can have in a given network.</summary>
        /// <param name="mask">The net mask of the network</param>
        /// <param name="options">Options which addresses to include an which not</param>
        /// <returns>The number of siblings an <see cref="T:System.Net.IPAddress"/> can have in the given network.</returns>
        public static int GetSiblingCount(this NetMask mask, SiblingOptions options)
        {
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));

            bool includeSelf = BitHelper.IsOptionSet(options, SiblingOptions.IncludeSelf);
            bool includeBroadcast = BitHelper.IsOptionSet(options, SiblingOptions.IncludeBroadcast);
            bool includeNetworkIdentifier = BitHelper.IsOptionSet(options, SiblingOptions.IncludeNetworkIdentifier);

            var hostPartBits = mask.GetMaskBytes().CountFromRight(false);
            var total = 1 << hostPartBits;
            total -= includeSelf ? 0 : 1;
            total -= includeBroadcast ? 0 : 1;
            total -= includeNetworkIdentifier ? 0 : 1;

            // TODO: Testing

            return total;
        }
    }
}
