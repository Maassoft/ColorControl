using System;

namespace novideo_srgb
{
    public class SrgbEOTF : ToneCurve
    {
        private double _black;

        public SrgbEOTF(double black)
        {
            _black = black;
        }

        public double SampleAt(double x)
        {
            if (x >= 1) return 1;

            double result;
            if (x <= 0.04045)
            {
                result = x / 12.92;
            }
            else
            {
                result = Math.Pow((x + 0.055) / 1.055, 2.4);
            }

            return result * (1 - _black) + _black;
        }

        public double SampleInverseAt(double x)
        {
            if (_black != 0) throw new NotSupportedException();
            if (x >= 1) return 1;

            if (x <= 0.0031308) return 12.92 * x;
            return 1.055 * Math.Pow(x, 1 / 2.4) - 0.055;
        }
    }
}