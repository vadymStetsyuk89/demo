using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class PublishDateTimeConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is DateTime)) {
                return null;
            }

            DateTime publishDate = ((DateTime)value).ToLocalTime();
            TimeSpan timeOffset = DateTime.Now - publishDate;

            string outputString = "";

            if (timeOffset.TotalMinutes <= 1) {
                outputString = "Moment ago";
            }
            else if (timeOffset.TotalMinutes <= 59) {
                outputString = string.Format("{0}m. ago", timeOffset.Minutes);
            }
            else if (timeOffset.TotalMinutes <= 1440) {
                outputString = string.Format("{0}h. ago", timeOffset.Hours);
            }
            else {
                outputString = string.Format("{0}d. ago", timeOffset.Days);
            }

            return outputString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("PublishDateTimeConverter.ConvertBack");
        }
    }
}
