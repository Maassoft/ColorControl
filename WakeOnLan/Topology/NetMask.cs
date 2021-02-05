using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net.Topology
{
    // TODO: Use Span<T> when available (instead of byte arrays)
    /// <summary>Represents an IPv4 net mask.</summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct NetMask : INetMask, IEquatable<NetMask>
    {
        [FieldOffset(0)]
        private readonly uint _mask;

        #region byte-wise fields

        // We are assuming here, that the byte-ordering is like on a normal x86 system

        [FieldOffset(3)]
        private readonly byte _maskB0;
        [FieldOffset(2)]
        private readonly byte _maskB1;
        [FieldOffset(1)]
        private readonly byte _maskB2;
        [FieldOffset(0)]
        private readonly byte _maskB3;

        #endregion

        internal const int MaskLength = 4;

        /// <summary>Represents an empty IPv4 NetMask (all bits set to 0).</summary>
        public static NetMask Empty { get; } = new NetMask();

        /// <summary>Gets the length of the net mask in bits.</summary>
        public int AddressLength => MaskLength * 8;

        /// <summary>Gets the amount of set bits from the left side (used in CIDR-Notation of net masks).</summary>
        public int Cidr => GetCidr(_mask);

        #region Ctors

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> cloning an existing instance of <see cref="T:System.Net.Topology.NetMask"/>.</summary>
        public NetMask(NetMask mask)
        {
            if (mask == null)
                throw new ArgumentNullException(nameof(mask));
            _maskB0 = _maskB1 = _maskB2 = _maskB3 = 0;
            _mask = mask._mask;
        }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> from an array of <see cref="System.Byte"/>.</summary>
        public NetMask(byte[] value)
        {
            _maskB0 = _maskB1 = _maskB2 = _maskB3 = 0;
            _mask = 0;
            if (value == null || value.Length == 0) // maybe throw ArgumentNullException?
                return;

            if (value.Length != MaskLength)
                throw new ArgumentException("Invalid mask length.");

            CheckMaskBytes(value); // check if passed mask are a valid mask. if not, throw Exception
            _maskB0 = value[0];
            _maskB1 = value[1];
            _maskB2 = value[2];
            _maskB3 = value[3];
        }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> from a given <see cref="T:System.Net.IPAddress"/>.</summary>
        /// <param name="address">The IPv4 address.</param>
        public NetMask(IPAddress address)
            : this(address == null ? null : address.GetAddressBytes())
        { }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/>.</summary>
        /// <param name="m0">The first byte.</param>
        /// <param name="m1">The second byte.</param>
        /// <param name="m2">The third byte.</param>
        /// <param name="m3">The fourth byte.</param>
        public NetMask(byte m0, byte m1, byte m2, byte m3)
            : this(new[] { m0, m1, m2, m3 })
        { }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/>.</summary>
        /// <param name="cidr">The mask represented by the CIDR notation integer.</param>
        public NetMask(byte cidr)
        {
            // maybe change parameter type interpretation to CIDR?
            if (cidr > MaskLength * 8)
                throw new ArgumentException("Invalid CIDR length");
            _maskB0 = _maskB1 = _maskB2 = _maskB3 = 0; // to keep init checks happy
            _mask = GetUIntFromCidrValue(cidr);
        }

        private NetMask(uint ipv4Mask)
        {
            _maskB0 = _maskB1 = _maskB2 = _maskB3 = 0; // to keep init checks happy
            _mask = ipv4Mask;
        }

        #endregion

        private static void CheckMaskBytes(byte[] bytes)
        {
            if (!bytes.RepresentsValidNetMask())
                throw new ArgumentException("The passed mask do not represent a valid net mask.");
        }

        /// <summary>Gets the bits of the net mask instance as an BitArray object instance.</summary>
        /// <returns>The bits of the net mask instance as an BitArray object instance.</returns>
        public byte[] GetMaskBytes() => new[] { _maskB0, _maskB1, _maskB2, _maskB3 };

        private static int GetCidr(uint mask)
        {
            Debug.Assert(sizeof(uint) == MaskLength);
            return mask.CountOnesFromLeft();
        }

        /// <summary>Extends the current <see cref="T:System.Net.Topology.NetMask"/> instance by a given value (CIDR-wise).</summary>
        /// <param name="mask">The mask to use as a reference.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Because <see cref="T:System.Net.Topology.NetMask"/> is a reference type, this method is static. If it were not like this, you could change the value of <see cref="T:System.Net.Topology.NetMask"/>.Empty, for example.</remarks>
        public static NetMask Extend(NetMask mask, int value)
        {
            int currentCidr = mask.Cidr;
            if (currentCidr >= MaskLength * 8)
                return new NetMask(mask);
            if (currentCidr <= 0)
                currentCidr = 0;

            int newLength = Math.Max(Math.Min(currentCidr + value, 32), 0);

            var m = BytesFromCidrValue(newLength);

            return new NetMask(m);
        }

        /// <summary>Abbreviates the current <see cref="T:System.Net.Topology.NetMask"/> instance by a given value (CIDR-wise).</summary>
        /// <param name="mask">The mask to use as a reference.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Because <see cref="T:System.Net.Topology.NetMask"/> is a reference type, this method is static. If it were not like this, you could change the value of <see cref="T:System.Net.Topology.NetMask"/>.Empty, for example.</remarks>
        public static NetMask Abbreviate(NetMask mask, int value)
        {
            int currentCidr = mask.Cidr;
            if (currentCidr < 1)
                return new NetMask(mask);
            if (currentCidr >= MaskLength * 8)
                currentCidr = MaskLength * 8;

            int newLength = Math.Max(Math.Min(currentCidr - value, 32), 0);

            var m = BytesFromCidrValue(newLength);
            return new NetMask(m);
        }

        /// <summary>Returns a value indicating whether the given array of <see cref="T:System.Byte"/> represents a valid net mask.</summary>
        /// <returns>True if the given array of <see cref="T:System.Byte"/> represents a valid net mask, otherwise false.</returns>
        public static bool GetIsValidNetMask(byte[] mask) => mask.RepresentsValidNetMask();

        // TODO: Testing(!)
        [Obsolete]
        private static byte[] BytesFromCidrValue(int cidr)
        {
            int target = MaskLength * 8 - cidr;
            int mask = 0;
            for (int i = 0; i < target; ++i)
            {
                mask >>= 1;
                mask |= unchecked((int)0x80000000);
            }
            var bytes = BitConverter.GetBytes(~mask);
            return new[] {
                bytes[0].ReverseBits(),
                bytes[1].ReverseBits(),
                bytes[2].ReverseBits(),
                bytes[3].ReverseBits()
            };
        }

        // TODO: Testing(!)
        private static uint GetUIntFromCidrValue(int cidr) => UIntExtensions.CreateWithOnesFromLeft(cidr);

        #region Operators

        #region Equality

        /// <summary>Returns a other indicating whether two instances of <see cref="T:System.Net.Topology.NetMask" /> are equal.</summary>
        /// <param name="n1">The first other to compare.</param>
        /// <param name="n2">The second other to compare.</param>
        /// <returns>true if <paramref name="n1" /> and <paramref name="n2" /> are equal; otherwise, false.</returns>
        public static bool operator ==(NetMask n1, NetMask n2)
        {
            if (ReferenceEquals(n1, n2))
                return true;
            if (((object)n1 == null) || ((object)n2 == null))
                return false;
            return n1.Equals(n2);
        }

        /// <summary>Returns a other indicating whether two instances of <see cref="T:System.Net.Topology.NetMask" /> are not equal.</summary>
        /// <param name="n1">The first other to compare. </param>
        /// <param name="n2">The second other to compare. </param>
        /// <returns>true if <paramref name="n1" /> and <paramref name="n2" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(NetMask n1, NetMask n2) => !(n1 == n2); // Problem solved

        #endregion
        #region And

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress operator &(IPAddress address, NetMask mask) => mask & address;

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress BitwiseAnd(IPAddress address, NetMask mask) => mask & address;

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress operator &(NetMask mask, IPAddress address)
        {
            var ipBytes = address == null
                ? new byte[MaskLength]
                : address.GetAddressBytes();
            var maskBytes = mask == null
                ? new byte[MaskLength]
                : new byte[] { mask._maskB0, mask._maskB1, mask._maskB2, mask._maskB3 };

            byte[] combinedBytes = maskBytes.And(ipBytes);
            return new IPAddress(combinedBytes);
        }

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress BitwiseAnd(NetMask mask, IPAddress address) => mask & address;

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the AND operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static NetMask operator &(NetMask n1, NetMask n2) => new NetMask(n1._mask & n2._mask);

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the AND operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static NetMask BitwiseAnd(NetMask n1, NetMask n2) => n1 & n2;

        #endregion
        #region Or

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the OR operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the OR operation.</returns>
        public static NetMask operator |(NetMask n1, NetMask n2) => new NetMask(n1._mask | n2._mask);

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the OR operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the OR operation.</returns>
        public static NetMask BitwiseOr(NetMask n1, NetMask n2) => n1 | n2;

        #endregion

        #endregion
        #region Common overrides

        /// <summary>Converts the other of this instance to its equivalent string representation.</summary>
        /// <returns>A string that represents the other of this instance.</returns>
        /// <filterpriority>1</filterpriority>
        public override string ToString()
        {
            var sb = new StringBuilder(4 * 3 + 3 * 3 + 32 + 3 + 3); // 255.255.255.255 (11111111111111111111111111111111)

            var arr = new byte[] { _maskB0, _maskB1, _maskB2, _maskB3 };
            var asString = arr.ToBinaryString('.');

            sb.Append(arr[0]).Append('.').Append(arr[1]).Append('.').Append(arr[2]).Append('.').Append(arr[3]).Append(" (");
            sb.Append(asString).Append(')');

            return sb.ToString();
        }

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and other.</summary>
        /// <returns>true if <paramref name="obj" /> is a <see cref="T:System.Net.Topology.NetMask" /> and equal to this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare with this instance. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj) => obj != null && obj is NetMask && Equals((NetMask)obj);

        /// <summary>Returns a value indicating whether this instance and a specified <see cref="T:System.Net.Topology.NetMask" /> object represent the same other.</summary>
        /// <returns>true if <paramref name="other" /> is equal to this instance; otherwise, false.</returns>
        /// <param name="other">An object to compare to this instance.</param>
        /// <filterpriority>2</filterpriority>
        public bool Equals(NetMask other) => other._mask == _mask;

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => unchecked((int)_mask);

        #endregion
    }
}
