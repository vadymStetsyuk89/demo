using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class Base64ToImageSourceConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            ImageSource imageSource = null;

            try {
                imageSource = ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(value.ToString())));
            }
            catch (Exception exc) {
                Crashes.TrackError(exc, new Dictionary<string, string>() { { "Method", "Base64ToImageSourceConverter.Convert" } });

                imageSource = null;
            }

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("Base64ToImageSourceConverter.ConvertBack");
        }
    }
}
