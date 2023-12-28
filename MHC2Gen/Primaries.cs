using LittleCms;

namespace MHC2Gen
{
    public class CIExy
    {
        public double x { get; set; }
        public double y { get; set; }

        public CIExy() { }

        public CIExy(CIExy source)
        {
            x = source.x;
            y = source.y;
        }

        public CIEXYZ ToXYZ(double Y = 1.0)
        {
            return new() { X = x * Y / y, Y = Y, Z = (1 - x - y) * Y / y };
        }
    }
    public class RgbPrimaries
    {
        public CIExy Red { get; }
        public CIExy Green { get; }
        public CIExy Blue { get; }
        public CIExy White { get; }

        public RgbPrimaries(RgbPrimaries primaries)
        {
            Red = new CIExy(primaries.Red);
            Green = new CIExy(primaries.Green);
            Blue = new CIExy(primaries.Blue);
            White = new CIExy(primaries.White);

        }
        public RgbPrimaries(CIExy red, CIExy green, CIExy blue, CIExy white)
        {
            Red = red;
            Green = green;
            Blue = blue;
            White = white;
        }

        public static RgbPrimaries sRGB { get; } = new RgbPrimaries(new() { x = 0.64, y = 0.33 }, new() { x = 0.30, y = 0.60 }, new() { x = 0.15, y = 0.06 }, new() { x = 0.3127, y = 0.3290 });
        public static RgbPrimaries AdobeRGB { get; } = new RgbPrimaries(new() { x = 0.64, y = 0.33 }, new() { x = 0.21, y = 0.71 }, new() { x = 0.15, y = 0.06 }, new() { x = 0.3127, y = 0.3290 });
        public static RgbPrimaries P3D65 { get; } = new RgbPrimaries(new() { x = 0.68, y = 0.32 }, new() { x = 0.265, y = 0.690 }, new() { x = 0.15, y = 0.06 }, new() { x = 0.3127, y = 0.3290 });
        public static RgbPrimaries Rec2020 { get; } = new RgbPrimaries(new() { x = 0.708, y = 0.292 }, new() { x = 0.170, y = 0.797 }, new() { x = 0.131, y = 0.046 }, new() { x = 0.3127, y = 0.3290 });

    }
}
