using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class IsNullToBoolConverter : IValueConverter {

        public bool WhenIsNull { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string stringValue) {
                return string.IsNullOrEmpty(stringValue) ? WhenIsNull : !WhenIsNull;
            }
            else {
                return value == null ? WhenIsNull : !WhenIsNull;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("IsNullToBoolConverter.ConvertBack");
    }
}
