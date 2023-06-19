using System;
using System.Globalization;
using System.Windows.Data;
using DJ.Resolver;
using NLog;

namespace DJ.XamlMultiValueConverter
{
    public class ILogEventResolverToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is LogEventInfo logEventInfo && values[1] is ILogEventInfoResolver resolver)
                return resolver.Resolve(logEventInfo);
            return "####";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
