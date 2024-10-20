using System.Globalization;
using System.Windows.Controls;
using static System.Double;

namespace novideo_srgb
{
    public class RangeRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public override ValidationResult? Validate(object valueObj, CultureInfo cultureInfo)
        {
            if (valueObj == null) return null;

            var valueString = (string)valueObj;
            if (valueString.EndsWith("."))
            {
                return new ValidationResult(false, "Input must not end in '.'");
            }

            double value = 0;
            try
            {
                if (valueString.Length > 0)
                    value = Parse(valueString.Replace(',', 'a'), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if (value < Min || value > Max)
            {
                return new ValidationResult(false,
                    $"Value must be between {Min} and {Max}");
            }

            return ValidationResult.ValidResult;
        }
    }
}