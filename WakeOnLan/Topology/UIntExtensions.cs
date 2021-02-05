using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System.Net.Topology
{
    internal static class UIntExtensions
    {
        // TODO: Tests
        public static int CountOnesFromLeft(this uint value)
        {
            var occurences = 0;
            for (uint i = 1u << 31; i >= 0; i >>= 1)
            {
                if ((i & value) != 0)
                    ++occurences;
                else
                    break;
            }
            return occurences;
        }

        // TODO: Tests
        public static uint CreateWithOnesFromLeft(int count)
        {
            if (count > 32)
                throw new ArgumentException("Cannot set more than 32 bits to one!");

            var value = 0u;

            var mask = 1u << 31;
            for (int i = 0; i < count; ++i)
            {
                value |= mask;
                mask >>= 1;
            }

            return value;
        }
    }
}
