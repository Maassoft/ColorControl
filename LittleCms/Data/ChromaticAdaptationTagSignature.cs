using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    internal class ChromaticAdaptationTagSignature : ISafeTagSignature<double[,]>
    {
        public TagSignature TagSignature => TagSignature.ChromaticAdaptationTag;

        public unsafe double[,] ReadFromProfile(IccProfile profile)
        {
            var ptr = (double*)profile.ReadTag(TagSignature);
            if (ptr == null) throw new TagNotFoundException();
            var result = new double[3, 3];
            new ReadOnlySpan<double>(ptr, 9).CopyTo(MemoryMarshal.CreateSpan(ref result[0, 0], 9));
            return result;
        }

        public unsafe void WriteToProfile(IccProfile profile, double[,] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            fixed (double* ptr = &data[0, 0])
                profile.WriteTag(TagSignature, ptr);
        }
    }
}
