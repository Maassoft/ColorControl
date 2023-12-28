using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LittleCms
{
    public class CmsException : Exception
    {
        public CmsError CmsError { get; }
        public CmsException() : base("Error occurred in lcms2") { CmsError = CmsError.UNDEFINED; }
        public CmsException(CmsError code, string message) : base(message) { CmsError = code; }
    }

    partial class CmsNative
    {
        private static readonly ThreadLocal<CmsException?> LastError;
        static CmsNative()
        {
            var libver = cmsGetEncodedCMMversion();
            if (libver != LCMS_VERSION)
            {
                throw new TypeLoadException("lcms2 version mismatch");
            }
            LastError = new();
            cmsSetLogErrorHandler(ErrorCallback);
        }

        private static void ErrorCallback(IntPtr ContextID, CmsError ErrorCode, string Text)
        {
            LastError.Value = new(ErrorCode, Text);
        }

        private static void ThrowLastTlsError()
        {
            if (LastError.IsValueCreated && LastError.Value != null)
            {
                var ex = LastError.Value;
                LastError.Value = null;
                throw ex;
            }
            else
            {
                throw new CmsException();
            }
        }

        public static uint CheckError(uint value)
        {
            if (value == 0)
            {
                ThrowLastTlsError();
            }
            return value;
        }

        public static void CheckError(bool value)
        {
            if (!value)
            {
                ThrowLastTlsError();
            }
        }

        public static IntPtr CheckError(IntPtr value)
        {
            if (value == IntPtr.Zero)
            {
                ThrowLastTlsError();
            }
            return value;
        }

        public unsafe static void* CheckError(void* value)
        {
            if (value == null)
            {
                ThrowLastTlsError();
            }
            return value;
        }
    }
}
