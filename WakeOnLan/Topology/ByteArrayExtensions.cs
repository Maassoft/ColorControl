using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System.Net.Topology
{
    internal static class ByteArrayExtensions
    {
        internal static IEnumerable<bool> ToBitStream(this byte[] bytes, bool fromLeft)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (fromLeft)
            {
                for (int i = 0; i < bytes.Length; ++i)
                {
                    byte tmp = bytes[i].ReverseBits();
                    for (int j = 0; j < 8; ++j)
                    {
                        yield return (tmp & 1) == 1;
                        tmp >>= 1;
                    }
                }
            }
            else
            {
                for (int i = bytes.Length - 1; i >= 0; --i)
                {
                    byte tmp = bytes[i];
                    for (int j = 0; j < 8; ++j)
                    {
                        yield return (tmp & 1) == 1;
                        tmp >>= 1;
                    }
                }
            }
        }

        private static int CountFromSide(byte[] bits, bool value, bool fromleft)
        {
            int counter = 0;
            var str = bits.ToBitStream(fromleft);
            foreach (var bit in str)
            {
                if (bit == value)
                    ++counter;
                else return counter;
            }
            return counter;
        }

        internal static int CountFromLeft(this byte[] bits, bool value)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));
            return CountFromSide(bits, value, true);
        }

        internal static int CountFromRight(this byte[] bits, bool value)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));
            return CountFromSide(bits, value, false);
        }

        internal static string ToBinaryString(this byte[] bits, char separator)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            const int radix = 2;
            const int padding = 8;
            const char paddingChar = '0';

            var sb = new StringBuilder();
            sb.Append(Convert.ToString(bits[0], radix).PadLeft(padding, paddingChar)).Append(separator);
            sb.Append(Convert.ToString(bits[1], radix).PadLeft(padding, paddingChar)).Append(separator);
            sb.Append(Convert.ToString(bits[2], radix).PadLeft(padding, paddingChar)).Append(separator);
            sb.Append(Convert.ToString(bits[3], radix).PadLeft(padding, paddingChar));
            return sb.ToString();
        }

        internal static string ToBinaryString(this byte[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            const int radix = 2;
            const int padding = 8;
            const char paddingChar = '0';

            var sb = new StringBuilder();
            sb.Append(Convert.ToString(bits[0], radix).PadLeft(padding, paddingChar));
            sb.Append(Convert.ToString(bits[1], radix).PadLeft(padding, paddingChar));
            sb.Append(Convert.ToString(bits[2], radix).PadLeft(padding, paddingChar));
            sb.Append(Convert.ToString(bits[3], radix).PadLeft(padding, paddingChar));
            return sb.ToString();
        }

        internal static bool RepresentsValidNetMask(this byte[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            int fromLeft = bits.CountFromLeft(true);
            int fromRight = bits.CountFromRight(false);

            // Sum of all counted indexes schloud be equal the whole length
            return (fromLeft + fromRight) == (8 * NetMask.MaskLength);
        }

        internal static byte[] And(this byte[] b1, byte[] b2)
        {
            if (b1 == null)
                throw new ArgumentNullException(nameof(b1));
            if (b2 == null)
                throw new ArgumentNullException(nameof(b2));

            if (b1.Length == 1 && b2.Length == 1)
            {
                int ib1 = b1[0];
                int ib2 = b2[0];
                return new[] { (byte)(ib1 & ib2) };
            }
            if (b1.Length == 2 && b2.Length == 2)
            {
                var sb1 = BitConverter.ToInt16(b1, 0);
                var sb2 = BitConverter.ToInt16(b2, 0);
                return BitConverter.GetBytes((short)(sb1 & sb2));
            }
            if (b1.Length == 4 && b2.Length == 4)
            {
                var ib1 = BitConverter.ToInt32(b1, 0);
                var ib2 = BitConverter.ToInt32(b2, 0);
                return BitConverter.GetBytes(ib1 & ib2);
            }
            if (b1.Length != b2.Length)
            {
                // Or maybe throw exception?

                int maxIndex = Math.Max(b1.Length, b2.Length);
                byte[] biggerArray = b1.Length > b2.Length ? b1 : b2;
                byte[] smallerArray = b1.Length <= b2.Length ? b1 : b2;

                var paddedArray = new byte[maxIndex];

                Buffer.BlockCopy(smallerArray, 0, paddedArray, 0, smallerArray.Length);

                Debug.Assert(biggerArray.Length == paddedArray.Length);

                return And(biggerArray, paddedArray);
            }

            var targetIndex = b1.Length;
            var andedArray = new byte[targetIndex];
            Buffer.BlockCopy(b1, 0, andedArray, 0, andedArray.Length);

            for (int i = 0; i < targetIndex; ++i)
                andedArray[i] &= b2[i];

            return andedArray;
        }

        internal static byte[] Or(this byte[] b1, byte[] b2)
        {
            if (b1 == null)
                throw new ArgumentNullException(nameof(b1));
            if (b2 == null)
                throw new ArgumentNullException(nameof(b2));

            if (b1.Length == 1 && b2.Length == 1)
            {
                int ib1 = b1[0];
                int ib2 = b2[0];
                return new[] { (byte)(ib1 | ib2) };
            }
            if (b1.Length == 2 && b2.Length == 2)
            {
                var sb1 = BitConverter.ToInt16(b1, 0);
                var sb2 = BitConverter.ToInt16(b2, 0);
                return BitConverter.GetBytes((short)(sb1 | sb2));
            }
            if (b1.Length == 4 && b2.Length == 4)
            {
                var ib1 = BitConverter.ToInt32(b1, 0);
                var ib2 = BitConverter.ToInt32(b2, 0);
                return BitConverter.GetBytes(ib1 | ib2);
            }
            if (b1.Length != b2.Length)
            {
                // Or maybe throw exception?

                int maxIndex = Math.Max(b1.Length, b2.Length);
                byte[] biggerArray = b1.Length > b2.Length ? b1 : b2;
                byte[] smallerArray = b1.Length <= b2.Length ? b1 : b2;

                var paddedArray = new byte[maxIndex];

                Buffer.BlockCopy(smallerArray, 0, paddedArray, 0, smallerArray.Length);

                Debug.Assert(biggerArray.Length == paddedArray.Length);

                return Or(biggerArray, paddedArray);
            }

            var targetIndex = b1.Length;
            var oredArray = new byte[targetIndex];
            Buffer.BlockCopy(b1, 0, oredArray, 0, oredArray.Length);

            for (int i = 0; i < targetIndex; ++i)
                oredArray[i] |= b2[i];

            return oredArray;
        }

        internal static byte[] Xor(this byte[] b1, byte[] b2)
        {
            if (b1 == null)
                throw new ArgumentNullException(nameof(b1));
            if (b2 == null)
                throw new ArgumentNullException(nameof(b2));

            // TODO: Testing

            if (b1.Length == 1 && b2.Length == 1)
            {
                int ib1 = b1[0];
                int ib2 = b2[0];
                return new[] { (byte)(ib1 ^ ib2) };
            }
            if (b1.Length == 2 && b2.Length == 2)
            {
                var sb1 = BitConverter.ToInt16(b1, 0);
                var sb2 = BitConverter.ToInt16(b2, 0);
                return BitConverter.GetBytes((short)(sb1 ^ sb2));
            }
            if (b1.Length == 4 && b2.Length == 4)
            {
                var ib1 = BitConverter.ToInt32(b1, 0);
                var ib2 = BitConverter.ToInt32(b2, 0);
                return BitConverter.GetBytes(ib1 ^ ib2);
            }
            if (b1.Length != b2.Length)
            {
                // Or maybe throw exception?

                int maxIndex = Math.Max(b1.Length, b2.Length);
                byte[] biggerArray = b1.Length > b2.Length ? b1 : b2;
                byte[] smallerArray = b1.Length <= b2.Length ? b1 : b2;

                var paddedArray = new byte[maxIndex];

                Buffer.BlockCopy(smallerArray, 0, paddedArray, 0, smallerArray.Length);

                Debug.Assert(biggerArray.Length == paddedArray.Length);

                return Xor(biggerArray, paddedArray);
            }

            var targetIndex = b1.Length;
            var xoredArray = new byte[targetIndex];
            Buffer.BlockCopy(b1, 0, xoredArray, 0, xoredArray.Length);

            for (int i = 0; i < targetIndex; ++i)
                xoredArray[i] ^= b2[i];

            return xoredArray;
        }

        internal static byte[] Not(this byte[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            if (bits.Length == 4)
            {
                var i = BitConverter.ToInt32(bits, 0);
                return BitConverter.GetBytes(~i);
            }
            var newBytes = new byte[bits.Length];
            for (int i = 0; i < newBytes.Length; ++i)
                newBytes[i] = unchecked((byte)(~bits[i]));
            return newBytes;
        }
    }
}
