using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class StringToUpperLoverCaseConverter : IValueConverter {

        public bool ToUpperCase { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value != null && value is string valueString) {
                return (ToUpperCase) ? valueString.ToUpper() : valueString.ToLower();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("StringToUpperLoverCaseConverter.ConvertBack");
        }
    }
}
