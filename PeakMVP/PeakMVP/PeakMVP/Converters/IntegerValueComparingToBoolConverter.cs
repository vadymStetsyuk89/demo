using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class IntegerValueComparingToBoolConverter : IValueConverter {

        public bool IsLessThan { get; set; }

        public int LimitValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (IsLessThan) {
                return (((int)value) < LimitValue);
            }
            else {
                return (((int)value) > LimitValue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("IntegerValueComparingToBoolConverter.ConvertBack");
        }
    }
}
