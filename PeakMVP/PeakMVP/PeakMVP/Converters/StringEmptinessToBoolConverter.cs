using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    class StringEmptinessToBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || !(value is string)) {
                return false;
            }

            return (!(string.IsNullOrEmpty(value.ToString())) && !(string.IsNullOrWhiteSpace(value.ToString())));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("StringEmptynessToBoolConverter.ConvertBack");
        }
    }
}
