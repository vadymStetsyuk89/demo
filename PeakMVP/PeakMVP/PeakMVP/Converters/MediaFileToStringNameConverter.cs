using PeakMVP.Models.Rests.DTOs;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class MediaFileToStringNameConverter : IValueConverter {

        public string StumbValue { get; set; } = "";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is MediaFile) {
                return (string.IsNullOrEmpty(((MediaFile)value).Path) || string.IsNullOrWhiteSpace(((MediaFile)value).Path))
                    ? StumbValue
                    : Path.GetFileName(((MediaFile)value).Path);
            }
            else if (value is MediaDTO) {
                return (string.IsNullOrEmpty(((MediaDTO)value).Url) || string.IsNullOrWhiteSpace(((MediaDTO)value).Url))
                    ? StumbValue
                    : Path.GetFileName(((MediaDTO)value).Url);
            }
            else {
                return StumbValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("NameFromStringPathConverter.ConvertBack");
        }
    }
}
