using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    internal record ToneCurveTagSignature(TagSignature TagSignature) : ISafeTagSignature<ToneCurve>
    {
        public unsafe ToneCurve ReadFromProfile(IccProfile profile)
        {
            var ptr = profile.ReadTag(TagSignature);
            if (ptr == null) throw new TagNotFoundException();
            return ToneCurve.CopyFromObject((IntPtr)ptr);
        }

        public unsafe void WriteToProfile(IccProfile profile, ToneCurve data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            profile.WriteTag(TagSignature, (void*)data.Handle);
        }
    }
}
