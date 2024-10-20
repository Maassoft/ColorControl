using LittleCms.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using static LittleCms.CmsNative;

namespace LittleCms
{
    public class IccProfile : CmsObject
    {
        public override CmsContext Context { get; }
        public byte[]? RawBytes { get; }

        public IccProfile(IntPtr handle, bool moveOwnership) : base(handle, moveOwnership)
        {
            Context = CmsContext.GetFromHandle(cmsGetProfileContextID(handle));
        }
        public IccProfile(IntPtr handle, CmsContext context, ReadOnlySpan<byte> bytes) : base(handle, true)
        {
            Context = context;
            RawBytes = bytes.ToArray();
        }

        public static IccProfile Create_sRGB(CmsContext context)
        {
            return new IccProfile(CheckError(cmsCreate_sRGBProfileTHR(context.Handle)), true);
        }

        public static IccProfile Create_sRGB()
        {
            return new IccProfile(CheckError(cmsCreate_sRGBProfile()), true);
        }

        public static IccProfile CreateXYZ(CmsContext context)
        {
            return new IccProfile(CheckError(cmsCreateXYZProfileTHR(context.Handle)), true);
        }

        public static IccProfile CreateXYZ()
        {
            return new IccProfile(CheckError(cmsCreateXYZProfile()), true);
        }

        public static IccProfile CreateRGB(CIExyY whitepoint, CIExyYTRIPLE primaries, RgbToneCurve transfer)
        {
            return CreateRGB(CmsContext.Default, whitepoint, primaries, transfer);
        }

        public static unsafe IccProfile CreateRGB(CmsContext context, CIExyY whitepoint, CIExyYTRIPLE primaries, RgbToneCurve transfer)
        {
            Span<IntPtr> transferhandle = stackalloc IntPtr[] { transfer.Red.Handle, transfer.Green.Handle, transfer.Blue.Handle };
            var handle = CheckError(cmsCreateRGBProfile(whitepoint, primaries, ref transferhandle[0]));
            return new(handle, true);
        }

        public static unsafe IccProfile Open(CmsContext context, ReadOnlySpan<byte> bytes)
        {
            fixed (byte* ptr = bytes)
            {
                var handle = CheckError(cmsOpenProfileFromMemTHR(context.Handle, ptr, (uint)bytes.Length));
                return new IccProfile(handle, context, bytes);
            }
        }
        public static IccProfile Open(ReadOnlySpan<byte> bytes) => Open(CmsContext.Default, bytes);


        public unsafe byte[] GetBytes()
        {
            uint newlen = 0;
            CheckError(cmsSaveProfileToMem(Handle, null, ref newlen));
            var newicc = new byte[newlen];
            fixed (byte* ptr = newicc)
                CheckError(cmsSaveProfileToMem(Handle, ptr, ref newlen));
            return newicc;
        }

        protected override void FreeObject()
        {
            cmsCloseProfile(Handle);
        }

        public unsafe void* ReadTag(TagSignature sig)
        {
            return cmsReadTag(Handle, sig);
        }

        public unsafe bool ContainsTag(TagSignature sig) => ReadTag(sig) != null;

        public T ReadTag<T>(ISafeTagSignature<T> sig)
        {
            return sig.ReadFromProfile(this);
        }

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public T? ReadTagOrDefault<T>(ISafeTagSignature<T> sig, T? defaultValue = default)
        {
            try
            {
                return sig.ReadFromProfile(this);
            }
            catch (TagNotFoundException)
            {
                return defaultValue;
            }
        }

        public bool TryReadTag<T>(ISafeTagSignature<T> sig, [NotNullWhen(returnValue: true)] out T? value)
        {
            try
            {
                value = sig.ReadFromProfile(this)!;
                return true;
            }
            catch (TagNotFoundException)
            {
                value = default;
                return false;
            }
        }

        public unsafe void WriteTag(TagSignature sig, void* handle)
        {
            var result = cmsWriteTag(Handle, sig, handle);
            if (handle != null)
            {
                CheckError(result);
            }
        }

        public void WriteTag<T>(ISafeTagSignature<T> sig, T value)
        {
            sig.WriteToProfile(this, value);
        }

        public unsafe void RemoveTag(TagSignature sig)
        {
            CheckError(cmsWriteTag(Handle, sig, null));
        }

        public unsafe void RemoveTag<T>(ISafeTagSignature<T> sig)
        {
            CheckError(cmsWriteTag(Handle, sig.TagSignature, null));
        }

        public unsafe byte[]? ReadRawTag(TagSignature sig)
        {
            if (!cmsIsTag(Handle, sig)) return null;
            var len = cmsReadRawTag(Handle, sig, null, 0);
            var buf = new byte[len];
            fixed (byte* ptr = buf)
                len = cmsReadRawTag(Handle, sig, ptr, len);
            return buf;
        }

        public void WriteRawTag(TagSignature sig, ReadOnlySpan<byte> bytes)
        {
            CheckError(cmsWriteRawTag(Handle, sig, in bytes[0], (uint)bytes.Length));
        }

        public void ComputeProfileId()
        {
            CheckError(cmsMD5computeID(Handle));
        }

        public HeaderFlags HeaderFlags
        {
            get => cmsGetHeaderFlags(Handle);
            set => cmsSetHeaderFlags(Handle, value);
        }

        public unsafe string GetInfo(InfoType info, string languageCode = MLU.NoLanguage, string countryCode = MLU.NoCountry)
        {
            var (lang, cont) = MLU.EncodeLanguageCountryCode(languageCode, countryCode);
            var len = cmsGetProfileInfo(Handle, info, in lang, in cont, null, 0);
            if (len == 0) return "";
            var buf = new byte[len];
            fixed (byte* ptr = buf)
                len = cmsGetProfileInfo(Handle, info, in lang, in cont, ptr, len);
            var s = WcharEncoding.GetString(buf)[..^1];
            return s;
        }

        public ulong HeaderAttributes
        {
            get
            {
                cmsGetHeaderAttributes(Handle, out var attr);
                return attr;
            }
            set => cmsSetHeaderAttributes(Handle, value);
        }

        public Guid HeaderProfileId
        {
            get
            {
                cmsGetHeaderProfileID(Handle, out var attr);
                return attr;
            }
            set => cmsSetHeaderProfileID(Handle, value);
        }

        public DateTime HeaderCreationDateTime
        {
            get
            {
                CheckError(cmsGetHeaderCreationDateTime(Handle, out var tm));
                return tm.ToDateTime();
            }
        }

        public RenderingIntent HeaderRenderingIntent
        {
            get => (RenderingIntent)cmsGetHeaderRenderingIntent(Handle);
            set => cmsSetHeaderRenderingIntent(Handle, (uint)value);
        }

        public uint HeaderManufacturer
        {
            get => cmsGetHeaderManufacturer(Handle);
            set => cmsSetHeaderManufacturer(Handle, value);
        }

        public uint HeaderCreator
        {
            get => cmsGetHeaderCreator(Handle);
        }

        public uint HeaderModel
        {
            get => cmsGetHeaderModel(Handle);
            set => cmsSetHeaderModel(Handle, value);
        }

        public ColorSpaceSignature PCS
        {
            get => cmsGetPCS(Handle);
            set => cmsSetPCS(Handle, value);
        }


        public ColorSpaceSignature ColorSpace
        {
            get => cmsGetColorSpace(Handle);
            set => cmsSetColorSpace(Handle, value);
        }

        public ProfileClassSignature DeviceClass
        {
            get => cmsGetDeviceClass(Handle);
            set => cmsSetDeviceClass(Handle, value);
        }

        public double ProfileVersion
        {
            get => cmsGetProfileVersion(Handle);
            set => cmsSetProfileVersion(Handle, value);
        }
    }
}
