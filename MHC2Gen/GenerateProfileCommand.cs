using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHC2Gen
{
    internal class ST2084
    {
        private const double m1 = 2610.0 / 16384.0;
        private const double m2 = 128.0 * 2523.0 / 4096.0;
        private const double c1 = 3424.0 / 4096.0;
        private const double c2 = 32.0 * 2413.0 / 4096.0;
        private const double c3 = 32.0 * 2392.0 / 4096.0;

        public static double NitsToSignal(double nits)
        {
            var Ypowm1 = Math.Pow(nits / 10000.0, m1);
            return Math.Pow((c1 + c2 * Ypowm1) / (1 + c3 * Ypowm1), m2);
        }

        public static double SignalToNits(double signal)
        {
            var Epow1divm2 = Math.Pow(signal, 1.0 / m2);
            return 10000 * Math.Pow(Math.Max(Epow1divm2 - c1, 0) / (c2 - c3 * Epow1divm2), 1 / m1);
        }
    }
}
