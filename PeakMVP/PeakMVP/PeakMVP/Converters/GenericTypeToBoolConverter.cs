using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class GenericTypeToBoolConverter<TTargetType>: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool result = (value == null) ? false : (value is TTargetType);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("GenericTypeToBoolConverter<T> convertBack is invalid");
        }
    }
}
