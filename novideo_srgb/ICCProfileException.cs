using System;

namespace novideo_srgb
{
    public class ICCProfileException : FormatException
    {
        public ICCProfileException(string message) : base(message)
        {
        }
    }
}