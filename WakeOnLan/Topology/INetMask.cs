
namespace System.Net.Topology
{
    /// <summary>Provides an interface for IP net masks.</summary>
    public interface INetMask
    {
        /// <summary>Gets the length of the net mask in bits.</summary>
        int AddressLength { get; }

        /// <summary>Gets the amount of set bits from the left side (used in CIDR-Notation of net masks).</summary>
        int Cidr { get; }

        /// <summary>Gets the bits of the net mask instance as an BitArray object instance.</summary>
        /// <returns>The bits of the net mask instance as an BitArray object instance</returns>
        byte[] GetMaskBytes();
    }
}
