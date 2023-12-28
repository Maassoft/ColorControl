using System;
using System.Collections.Generic;

namespace LittleCms.Data
{
    internal record BlittableDataTag<T>(TagSignature TagSignature) : ISafeTagSignature<T> where T : unmanaged
    {
        public unsafe T ReadFromProfile(IccProfile profile)
        {
            var ptr = profile.ReadTag(TagSignature);
            if (ptr == null) throw new TagNotFoundException();
            return *(T*)ptr;
        }

        public unsafe void WriteToProfile(IccProfile profile, T data)
        {
            profile.WriteTag(TagSignature, &data);
        }
    }
}
