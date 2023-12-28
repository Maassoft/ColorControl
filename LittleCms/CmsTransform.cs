using System;
using System.Collections.Generic;
using System.Text;

using static LittleCms.CmsNative;

namespace LittleCms
{
    public class CmsTransform : CmsObject
    {
        private IccProfile inputProfile;
        private IccProfile outputProfile;
        public override CmsContext Context { get; }
        public CmsTransform(IccProfile inputProfile, CmsPixelFormat inputFormat,
            IccProfile outputProfile, CmsPixelFormat outputFormat,
            RenderingIntent intent, TransformFlags flags) : this(CmsContext.Default, inputProfile, inputFormat, outputProfile, outputFormat, intent, flags) { }

        public CmsTransform(CmsContext context, IccProfile inputProfile, CmsPixelFormat inputFormat,
            IccProfile outputProfile, CmsPixelFormat outputFormat,
            RenderingIntent intent, TransformFlags flags)
        {
            this.Context = context;
            this.inputProfile = inputProfile;
            this.outputProfile = outputProfile;

            var handle = CheckError(cmsCreateTransformTHR(context.Handle, inputProfile.Handle, inputFormat, outputProfile.Handle, outputFormat, (uint)intent, (uint)flags));
            AttachObject(handle, true);
        }


        protected override void FreeObject()
        {
            cmsDeleteTransform(Handle);
        }

        public unsafe void DoTransform<TIn, TOut>(ReadOnlySpan<TIn> input, Span<TOut> output, uint pixelsToTransform) where TIn : unmanaged where TOut : unmanaged
        {
            fixed (TIn* inptr = input)
            fixed (TOut* outptr = output)
                cmsDoTransform(Handle, inptr, outptr, pixelsToTransform);
        }

        public unsafe void DoTransform<TIn, TOut>(ReadOnlySpan<TIn> input, Span<TOut> output, uint pixelsPerLine, uint lineCount,
            uint bytesPerLineIn, uint bytesPerLineOut, uint bytesPerPlaneIn, uint bytesPerPlaneOut) where TIn : unmanaged where TOut : unmanaged
        {
            fixed (TIn* inptr = input)
            fixed (TOut* outptr = output)
                cmsDoTransformLineStride(Handle, inptr, outptr, pixelsPerLine, lineCount, bytesPerLineIn, bytesPerLineOut, bytesPerPlaneIn, bytesPerPlaneOut);
        }

    }
}
