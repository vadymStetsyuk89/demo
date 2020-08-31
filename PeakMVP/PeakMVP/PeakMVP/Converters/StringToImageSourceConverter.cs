using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class StringToImageSourceConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value != null) ? ImageSource.FromResource(value.ToString()) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
