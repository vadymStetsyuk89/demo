using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class IntWithLimitValueConverter : IValueConverter {

        public int LimitValue { get; set; } = 99;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((int)value) < LimitValue ? value : string.Format("{0}*", LimitValue);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("IntWithLimitValueConverter.ConvertBack");
    }
}
