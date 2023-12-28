using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    internal record MluTagSignature(TagSignature TagSignature) : ISafeTagSignature<MLU>
    {
        public unsafe MLU ReadFromProfile(IccProfile profile)
        {
            var ptr = profile.ReadTag(TagSignature);
            if (ptr == null) throw new TagNotFoundException();
            return MLU.CopyFromObject((IntPtr)ptr);
        }

        public unsafe void WriteToProfile(IccProfile profile, MLU data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            profile.WriteTag(TagSignature, (void*)data.Handle);
        }
    }
}
