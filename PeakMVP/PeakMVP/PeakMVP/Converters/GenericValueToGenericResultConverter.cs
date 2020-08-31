using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class GenericValueComparerToGenericResultConverter<TTargetValue, TResult> : IValueConverter {

        public TTargetValue TargetValue { get; set; }

        public TResult TrueResult { get; set; }

        public TResult FalseResult { get; set; }

        public bool UseValueAsResult { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || !value.Equals(TargetValue)) {
                return UseValueAsResult ? value : FalseResult;
            }
            else {
                return TrueResult;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new InvalidOperationException("GenericValueToGenericResultConverter.ConvertBack");
    }
}
