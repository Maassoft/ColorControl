using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms.Data
{
    public class TagNotFoundException : Exception
    {
        private const int hr = unchecked((int)0x80131577);
        public TagNotFoundException()
        {
            HResult = hr;
        }

        public TagNotFoundException(string message) : base(message)
        {
            HResult = hr;

        }
        public TagNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
            HResult = hr;
        }
    }
}
