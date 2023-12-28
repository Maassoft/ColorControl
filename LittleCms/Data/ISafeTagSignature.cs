using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    public interface ISafeTagSignature<T>
    {
        TagSignature TagSignature { get; }
        T ReadFromProfile(IccProfile profile);
        void WriteToProfile(IccProfile profile, T data);
    }

}
