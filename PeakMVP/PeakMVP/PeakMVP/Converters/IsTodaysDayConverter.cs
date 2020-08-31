using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class IsTodaysDayConverter : IValueConverter {

        public static readonly string UNSUPORTED_VALUE_TYPE = "Unsuported value type";

        public string TodaysReplacement { get; set; } = "Today";

        public string StringFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string result = null;

            if (value is DateTime dateTime) {
                if (dateTime.Date == DateTime.Now.Date) {
                    result = TodaysReplacement;
                }
                else {
                    result = dateTime.ToString(StringFormat);
                }

                value = dateTime.ToString(StringFormat);
            }
            else {
                value = UNSUPORTED_VALUE_TYPE;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("IsTodaysDayConverter.ConvertBack");
    }
}
