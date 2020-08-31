using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class LengthILisToBoolConverter : IValueConverter {

        /// <summary>
        /// True: if sequence is not null or sequence contains at least one element - true.
        /// False: if sequence is null or sequence doesn't contains at least one element - true.
        /// </summary>
        public bool IsAny { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (IsAny) {
                if (value == null || ((IList)value).Count == 0) {
                    return false;
                }

                return true;
            }
            else {
                if (value == null || ((IList)value).Count == 0) {
                    return true;
                }

                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("SequenceLengthToBoolConverter.ConvertBack");
        }
    }
}
