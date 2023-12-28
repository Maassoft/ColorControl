using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    internal record IccDataTagSignature(TagSignature TagSignature) : ISafeTagSignature<ICCData>
    {
        public unsafe ICCData ReadFromProfile(IccProfile profile)
        {
            var ptr = profile.ReadTag(TagSignature);
            if (ptr == null) throw new TagNotFoundException();
            return _ICCData.ToManaged((_ICCData*)ptr);
        }

        public unsafe void WriteToProfile(IccProfile profile, ICCData obj)
        {
            var data = new byte[sizeof(_ICCDataHeader) + obj.Data.Length];
            obj.Data.Span.CopyTo(data.AsSpan().Slice(sizeof(_ICCDataHeader)));
            fixed (byte* ptr = data)
            {
                var header = (_ICCDataHeader*)ptr;
                header->flag = obj.Flag;
                header->len = (uint)obj.Data.Length;

                profile.WriteTag(TagSignature, ptr);
            }
        }
    }
}
