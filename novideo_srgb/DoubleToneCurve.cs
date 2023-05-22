namespace novideo_srgb
{
    public class DoubleToneCurve : ToneCurve
    {
        private double[] _values;

        public DoubleToneCurve(double[] values)
        {
            _values = values;
        }

        public double SampleAt(double x)
        {
            if (x == 0) return _values[0];
            if (x >= 1) return _values[_values.Length - 1];

            var index = x * (_values.Length - 1);
            var frac = index - (uint)index;
            return _values[(uint)index] * (1 - frac) + _values[(uint)index + 1] * frac;
        }

        public double SampleInverseAt(double x)
        {
            if (_values[0] >= x) return 0;
            if (_values[_values.Length - 1] <= x) return 1;

            var lowerIndex = -1;
            for (var i = 0; i < _values.Length; i++)
            {
                if (x >= _values[i])
                {
                    lowerIndex = i;
                }
            }

            var low = _values[lowerIndex];
            var high = _values[lowerIndex + 1];
            var frac = (x - low) / (high - low);

            return (lowerIndex + frac) / (_values.Length - 1);
        }
    }
}