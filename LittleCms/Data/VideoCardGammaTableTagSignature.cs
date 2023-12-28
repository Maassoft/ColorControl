using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    internal class VideoCardGammaTableTagSignature : ISafeTagSignature<RgbToneCurve>
    {
        public TagSignature TagSignature => TagSignature.VcgtTag;

        public unsafe RgbToneCurve ReadFromProfile(IccProfile profile)
        {
            var ptr = (IntPtr*)profile.ReadTag(TagSignature);
            if (ptr == null) throw new TagNotFoundException();
            return new RgbToneCurve(
                ToneCurve.CopyFromObject(ptr[0]),
                ToneCurve.CopyFromObject(ptr[1]),
                ToneCurve.CopyFromObject(ptr[2])
                );
        }

        public unsafe void WriteToProfile(IccProfile profile, RgbToneCurve obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var handles = stackalloc IntPtr[] { obj.Red.Handle, obj.Green.Handle, obj.Blue.Handle };
            profile.WriteTag(TagSignature, handles);
        }
    }
}
