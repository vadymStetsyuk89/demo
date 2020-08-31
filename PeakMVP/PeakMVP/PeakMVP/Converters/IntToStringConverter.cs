using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class IntToStringConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value is int && (int)value > 0) ?
                ((int)value == 1) ? $"{value.ToString()} comment" : $"{value.ToString()} comments" : "No comments";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
