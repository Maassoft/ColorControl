using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;


using static LittleCms.CmsNative;
using System.Runtime.CompilerServices;

namespace LittleCms.Data
{
    public class MLU : CmsObject
    {
        public const string NoLanguage = "\0\0";
        public const string NoCountry = "\0\0";

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [Obsolete("Only allocator in context is used in MLU, which is not modeled in this binding")]
        public override CmsContext Context { get; } = CmsContext.Default;
#pragma warning restore CS0809

        internal unsafe static (uint, uint) EncodeLanguageCountryCode(string code1, string code2)
        {
            if (code1 == null || code1.Length != 2 || code1[0] >= 0x80 || code1[1] >= 0x80) throw new ArgumentException();
            if (code2 == null || code2.Length != 2 || code2[0] >= 0x80 || code2[1] >= 0x80) throw new ArgumentException();
            Span<uint> result32 = stackalloc uint[2];
            var result8 = MemoryMarshal.Cast<uint, byte>(result32);
            result8[0] = (byte)code1[0];
            result8[1] = (byte)code1[1];
            result8[4] = (byte)code2[0];
            result8[5] = (byte)code2[1];
            return (result32[0], result32[1]);
        }

        /// <summary>
        /// Attach a cmsMLU
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="moveOwnership"></param>
        public MLU(IntPtr handle, bool moveOwnership = false) : base(handle, moveOwnership)
        {
            // FIXME: missing cmsGetMLUContextID
        }

        public MLU(int itemCount) : this(CmsContext.Default, itemCount) { }

        public MLU(CmsContext context, int itemCount)
        {
            Context = context;
            IntPtr handle = CheckError(cmsMLUalloc(context.Handle, (uint)itemCount));
            AttachObject(handle, true);
        }

        /// <summary>
        /// Copy a cmsMLU by handle
        /// </summary>
        /// <param name="handle"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static MLU CopyFromObject(nint handleToCopy)
        {
            nint handle = CheckError(cmsMLUdup(handleToCopy));
            return new(handle, true);
        }

        /// <summary>
        /// Creates a MLU with en-US string
        /// </summary>
        /// <param name="s"></param>
        public MLU(string s) : this(1)
        {
            Set("en", "US", s);
        }

        protected override void FreeObject()
        {
            cmsMLUfree(Handle);
        }

        public unsafe void Set(string language, string country, string s)
        {
            var (lang, cont) = EncodeLanguageCountryCode(language, country);
            var wchbuf = WcharEncoding.GetBytes(s + "\0");
            CheckError(cmsMLUsetWide(Handle, in lang, in cont, in wchbuf[0]));

        }

        public unsafe string? Get(string languageCode, string countryCode)
        {
            var (lang, cont) = EncodeLanguageCountryCode(languageCode, countryCode);
            var len = cmsMLUgetWide(Handle, in lang, in cont, null, 0);
            if (len == 0)
            {
                return null;
            }
            var buf = new byte[(int)len];
            fixed (byte* ptr = buf)
                len = CheckError(cmsMLUgetWide(Handle, in lang, in cont, ptr, len));
            var s = WcharEncoding.GetString(buf);
            return s[..^1];
        }

        public string? GetAny() => Get(NoLanguage, NoCountry);

        public override string ToString() => GetAny() ?? "";


    }
}