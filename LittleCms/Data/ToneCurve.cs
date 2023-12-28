using System;
using System.Collections.Generic;
using System.Text;

using static LittleCms.CmsNative;

namespace LittleCms.Data
{
    public class ToneCurve : CmsObject
    {
#pragma warning disable CS0809
        [Obsolete("lcms2 don't expose API to get context of cmsToneCurve")]
        public override CmsContext Context => throw new NotImplementedException();
#pragma warning restore CS0809

        public ToneCurve(nint handle, bool moveOwnership) : base(handle, moveOwnership) { }

        public ToneCurve(CmsContext context, double gamma)
        {
            AttachObject(CheckError(cmsBuildGamma(context.Handle, gamma)), true);
        }

        public ToneCurve(CmsContext context, ReadOnlySpan<float> table)
        {
            AttachObject(CheckError(cmsBuildTabulatedToneCurveFloat(context.Handle, (uint)table.Length, in table[0])), true);
        }

        public ToneCurve(CmsContext context, ReadOnlySpan<ushort> table)
        {
            AttachObject(CheckError(cmsBuildTabulatedToneCurve16(context.Handle, (uint)table.Length, in table[0])), true);
        }

        public ToneCurve(double gamma) : this(CmsContext.Default, gamma) { }

        public ToneCurve(ReadOnlySpan<float> table) : this(CmsContext.Default, table) { }

        public ToneCurve(ReadOnlySpan<ushort> table) : this(CmsContext.Default, table) { }

        public static ToneCurve CopyFromObject(nint copyFromObject)
        {
            var handle = CheckError(cmsDupToneCurve(copyFromObject));
            return new(handle, true);
        }
        protected override void FreeObject()
        {
            throw new NotImplementedException();
        }

        public ToneCurve Reverse()
        {
            var handle = CheckError(cmsReverseToneCurve(Handle));
            return new(handle, true);
        }

        public ToneCurve Reverse(uint sampleCount)
        {
            var handle = CheckError(cmsReverseToneCurveEx(sampleCount, Handle));
            return new(handle, true);
        }

        public ushort EvalU16(ushort x) => cmsEvalToneCurve16(Handle, x);
        public float EvalF32(float x) => cmsEvalToneCurveFloat(Handle, x);

    }
}
