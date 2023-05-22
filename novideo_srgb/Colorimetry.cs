// credit to https://mina86.com/2019/srgb-xyz-matrix/ and http://www.brucelindbloom.com/ for the math

namespace novideo_srgb
{
    public static class Colorimetry
    {
        public struct Point
        {
            public bool Equals(Point other)
            {
                return X.Equals(other.X) && Y.Equals(other.Y);
            }

            public override bool Equals(object obj)
            {
                return obj is Point other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (X.GetHashCode() * 397) ^ Y.GetHashCode();
                }
            }

            public double X;
            public double Y;
        }

        public struct ColorSpace
        {
            public bool Equals(ColorSpace other)
            {
                return Red.Equals(other.Red) && Green.Equals(other.Green) && Blue.Equals(other.Blue) &&
                       White.Equals(other.White);
            }

            public override bool Equals(object obj)
            {
                return obj is ColorSpace other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Red.GetHashCode();
                    hashCode = (hashCode * 397) ^ Green.GetHashCode();
                    hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                    hashCode = (hashCode * 397) ^ White.GetHashCode();
                    return hashCode;
                }
            }

            public Point Red;
            public Point Green;
            public Point Blue;
            public Point White;
        }

        public static Point D65 = new Point { X = 0.3127, Y = 0.3290 };

        public static ColorSpace sRGB = new ColorSpace
        {
            Red = new Point { X = 0.64, Y = 0.33 },
            Green = new Point { X = 0.3, Y = 0.6 },
            Blue = new Point { X = 0.15, Y = 0.06 },
            White = D65
        };

        public static ColorSpace DisplayP3 = new ColorSpace
        {
            Red = new Point { X = 0.68, Y = 0.32 },
            Green = new Point { X = 0.265, Y = 0.69 },
            Blue = new Point { X = 0.15, Y = 0.06 },
            White = D65
        };

        public static ColorSpace AdobeRGB = new ColorSpace
        {
            Red = new Point { X = 0.64, Y = 0.33 },
            Green = new Point { X = 0.21, Y = 0.71 },
            Blue = new Point { X = 0.15, Y = 0.06 },
            White = D65
        };

        public static ColorSpace BT2020 = new ColorSpace
        {
            Red = new Point { X = 0.708, Y = 0.292 },
            Green = new Point { X = 0.17, Y = 0.797 },
            Blue = new Point { X = 0.131, Y = 0.046 },
            White = D65
        };

        public static ColorSpace[] ColorSpaces => new[] { sRGB, DisplayP3, AdobeRGB, BT2020 };

        public static Matrix D50 = Matrix.FromValues(new[,] { { 0.9642 }, { 1 }, { 0.8249 } });

        public static Matrix RGBToXYZ(ColorSpace colorSpace)
        {
            var red = colorSpace.Red;
            var green = colorSpace.Green;
            var blue = colorSpace.Blue;
            var white = colorSpace.White;
            var whiteXYZ = Matrix.FromValues(new[,]
                { { white.X / white.Y }, { 1 }, { (1 - white.X - white.Y) / white.Y } });

            var Mprime = Matrix.FromValues(new[,]
            {
                { red.X / red.Y, green.X / green.Y, blue.X / blue.Y },
                { 1, 1, 1 },
                { (1 - red.X - red.Y) / red.Y, (1 - green.X - green.Y) / green.Y, (1 - blue.X - blue.Y) / blue.Y }
            });

            return Mprime * Matrix.FromDiagonal(Mprime.Inverse() * whiteXYZ);
        }

        public static Matrix XYZToRGB(ColorSpace colorSpace)
        {
            return RGBToXYZ(colorSpace).Inverse();
        }

        public static Matrix RGBToRGB(ColorSpace from, ColorSpace to)
        {
            var result = XYZToRGB(to) * RGBToXYZ(from);
            return result;
        }

        public static Matrix RGBToAdaptedXYZ(ColorSpace colorspace, Matrix whiteXYZ)
        {
            var xyz = RGBToXYZ(colorspace);
            var bradford = Matrix.FromValues(new[,]
            {
                { 0.8951, 0.2664, -0.1614 },
                { -0.7502, 1.7135, 0.0367 },
                { 0.0389, -0.0685, 1.0296 }
            });
            var ws = colorspace.White;
            var aws = bradford * Matrix.FromValues(new[,]
            {
                { ws.X / ws.Y }, { 1 }, { (1 - ws.X - ws.Y) / ws.Y }
            });
            var awd = bradford * whiteXYZ;
            var m = bradford.Inverse() * Matrix.FromDiagonal(new[]
                { awd[0] / aws[0], awd[1] / aws[1], awd[2] / aws[2] }) * bradford;
            return m * xyz;
        }

        public static Matrix RGBToPCSXYZ(ColorSpace colorspace)
        {
            return RGBToAdaptedXYZ(colorspace, D50);
        }

        public static Matrix PCSXYZToRGB(ColorSpace colorspace)
        {
            return RGBToPCSXYZ(colorspace).Inverse();
        }

        public static Matrix XYZScale(Matrix matrix, Matrix target)
        {
            var result = Matrix.Zero3x3();
            var white = matrix * Matrix.One3x1();
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    result[i, j] = matrix[i, j] * target[i] / white[i];
                }
            }

            return result;
        }

        public static Matrix XYZScaleToD50(Matrix matrix)
        {
            return XYZScale(matrix, D50);
        }
    }
}