using LittleCms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHC2Gen
{
    internal static class Extension
    {

        public static CIExy ToXY(in this CIExyY xyY)
        {
            return new() { x = xyY.x, y = xyY.y };
        }
        public static CIExy ToXY(in this CIEXYZ xyz)
        {
            return xyz.ToCIExyY().ToXY();
        }


    }
}
