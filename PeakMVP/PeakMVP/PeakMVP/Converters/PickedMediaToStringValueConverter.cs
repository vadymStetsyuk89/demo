using PeakMVP.Models.Identities.Medias;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class PickedMediaToStringValueConverter : IValueConverter {

        public string StubValue { get; set; } = "Stub";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            string result = StubValue;

            if ((value != null) && (value is PickedMediaBase) && !(string.IsNullOrEmpty(((PickedMediaBase)value).Name))) {
                result = ((PickedMediaBase)value).Name;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("StringToStringWithPossibleStumbValueConverter.ConvertBack");
        }
    }
}
