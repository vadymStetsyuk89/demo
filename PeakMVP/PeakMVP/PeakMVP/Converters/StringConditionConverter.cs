using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class StringConditionConverter : IValueConverter {

        public string StubForNovalue { get; set; } = "No value";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string result = null;

            if (value is string stringValue) {
                result = string.IsNullOrEmpty(stringValue) || string.IsNullOrEmpty(stringValue) ? StubForNovalue : stringValue;
            }
            else {
                result = StubForNovalue;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("StringConditionConvertor.ConvertBack");
    }
}
