using System;
using System.Collections.Generic;
using System.Text;

namespace LittleCms
{
    public class CmsGlobal
    {

        public static CIEXYZ D50XYZ { get; } = CmsNative.cmsD50_XYZ();
        public static CIExyY D50xyY { get; } =  CmsNative.cmsD50_xyY();


        public unsafe static CIEXYZ AdaptToIlluminant(in CIEXYZ SourceWhitePt, in CIEXYZ Illuminant, in CIEXYZ Value)
        {
            CmsNative.CheckError(CmsNative.cmsAdaptToIlluminant(out var result, SourceWhitePt, Illuminant, Value));
            return result;
        }
    }
}
