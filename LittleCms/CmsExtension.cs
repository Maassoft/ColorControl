using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleCms
{
    public static class CmsExtension
    {

        public static CIExyY ToCIExyY(this CIEXYZ xyz)
        {
            return new CIExyY { x = xyz.X / (xyz.X + xyz.Y + xyz.Z), y = xyz.Y / (xyz.X + xyz.Y + xyz.Z), Y = xyz.Y };
        }
        public static CIExyY ToXY(this CIEXYZ xyz)
        {
            var sum = xyz.X + xyz.Y + xyz.Z;
            return new() { x = xyz.X / sum, y = xyz.Y / sum, Y = 1 };
        }

        public static CIEXYZ ToXYZ(this CIExyY xyY)
        {
            return new() { X = xyY.x * xyY.Y / xyY.y, Y = xyY.Y, Z = (1 - xyY.x - xyY.y) * xyY.Y / xyY.y };
        }

        public static int GetChannelCount(this ColorSpaceSignature sig)
        {
            return (int)CmsNative.cmsChannelsOf(sig);
        }


    }
}
