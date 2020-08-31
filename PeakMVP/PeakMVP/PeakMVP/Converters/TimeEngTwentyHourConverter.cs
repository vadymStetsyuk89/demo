using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class TimeEngTwentyHourConverter : IValueConverter {

        public bool IsToUpper { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                string dateString = string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:M/d/yy h:mm tt}", ((DateTime)value));

                if (IsToUpper) {
                    dateString = dateString.ToUpper();
                }

                return dateString;
            }
            catch (Exception exc) {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("TimeEngTwentyHourConverter.ConvertBack");
    }
}
