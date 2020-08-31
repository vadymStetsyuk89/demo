using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class StringToURLImageSourceConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return (value is string)
                ? (string.IsNullOrWhiteSpace(value.ToString()) || string.IsNullOrEmpty(value.ToString()))
                    ? ImageSource.FromResource(GlobalSettings.Instance.UserProfile.DEFAULT_AVATAR)
                    : ImageSource.FromUri(new Uri(value.ToString()))
                : ImageSource.FromResource(GlobalSettings.Instance.UserProfile.DEFAULT_AVATAR);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("StringToURLImageSourceConverter ConvertBack is invalid");
        }
    }
}
