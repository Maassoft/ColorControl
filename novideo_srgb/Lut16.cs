using System;

namespace novideo_srgb
{
    public class Lut16
    {
        private ushort[,,,] _lut;
        private int _lutSize;

        private ToneCurve[] inputCurves;
        private ToneCurve[] outputCurves;

        public Lut16(ToneCurve[] input, ushort[,,,] lut, ToneCurve[] output)
        {
            _lut = lut;
            _lutSize = _lut.GetLength(0);

            inputCurves = input;
            outputCurves = output;
        }

        public Matrix SampleGrayscaleAt(double index)
        {
            return SampleAt(index, index, index);
        }

        public Matrix SampleAt(double r, double g, double b)
        {
            var result = Matrix.FromValues(new[,]
            {
                { inputCurves[0].SampleAt(r) },
                { inputCurves[1].SampleAt(g) },
                { inputCurves[2].SampleAt(b) }
            });

            result = SampleCLUTTetrahedral(result);

            for (var i = 0; i < 3; i++)
            {
                result[i] = outputCurves[i].SampleAt(result[i]);
            }

            return result;
        }

        private Matrix S(int x, int y, int z)
        {
            x = Math.Min(x, _lutSize - 1);
            y = Math.Min(y, _lutSize - 1);
            z = Math.Min(z, _lutSize - 1);
            var sample0 = _lut[x, y, z, 0] / (double)ushort.MaxValue;
            var sample1 = _lut[x, y, z, 1] / (double)ushort.MaxValue;
            var sample2 = _lut[x, y, z, 2] / (double)ushort.MaxValue;

            return Matrix.FromValues(new[,] { { sample0 }, { sample1 }, { sample2 } });
        }

        private Matrix S(double x, double y, double z)
        {
            return S((int)x, (int)y, (int)z);
        }

        // https://www.filmlight.ltd.uk/pdf/whitepapers/FL-TL-TN-0057-SoftwareLib.pdf
        private Matrix SampleCLUTTetrahedral(Matrix rgb)
        {
            var lutIndex = rgb * (_lutSize - 1);
            var n = lutIndex.Map(Math.Floor);
            var f = lutIndex.Map(x => x - (int)x);

            Matrix Sxyz;
            if (f[0] > f[1])
            {
                if (f[1] > f[2])
                {
                    Sxyz = (1 - f[0]) * S(n[0], n[1], n[2])
                           + (f[0] - f[1]) * S(n[0] + 1, n[1], n[2])
                           + (f[1] - f[2]) * S(n[0] + 1, n[1] + 1, n[2])
                           + (f[2]) * S(n[0] + 1, n[1] + 1, n[2] + 1);
                }
                else if (f[0] > f[2])
                {
                    Sxyz = (1 - f[0]) * S(n[0], n[1], n[2])
                           + (f[0] - f[2]) * S(n[0] + 1, n[1], n[2])
                           + (f[2] - f[1]) * S(n[0] + 1, n[1], n[2] + 1)
                           + (f[1]) * S(n[0] + 1, n[1] + 1, n[2] + 1);
                }
                else
                {
                    Sxyz = (1 - f[2]) * S(n[0], n[1], n[2])
                           + (f[2] - f[0]) * S(n[0], n[1], n[2] + 1)
                           + (f[0] - f[1]) * S(n[0] + 1, n[1], n[2] + 1)
                           + (f[1]) * S(n[0] + 1, n[1] + 1, n[2] + 1);
                }
            }
            else
            {
                if (f[2] > f[1])
                {
                    Sxyz = (1 - f[2]) * S(n[0], n[1], n[2])
                           + (f[2] - f[1]) * S(n[0], n[1], n[2] + 1)
                           + (f[1] - f[0]) * S(n[0], n[1] + 1, n[2] + 1)
                           + (f[0]) * S(n[0] + 1, n[1] + 1, n[2] + 1);
                }
                else if (f[2] > f[0])
                {
                    Sxyz = (1 - f[1]) * S(n[0], n[1], n[2])
                           + (f[1] - f[2]) * S(n[0], n[1] + 1, n[2])
                           + (f[2] - f[0]) * S(n[0], n[1] + 1, n[2] + 1)
                           + (f[0]) * S(n[0] + 1, n[1] + 1, n[2] + 1);
                }
                else
                {
                    Sxyz = (1 - f[1]) * S(n[0], n[1], n[2])
                           + (f[1] - f[0]) * S(n[0], n[1] + 1, n[2])
                           + (f[0] - f[2]) * S(n[0] + 1, n[1] + 1, n[2])
                           + (f[2]) * S(n[0] + 1, n[1] + 1, n[2] + 1);
                }
            }

            return Sxyz;
        }
    }
}