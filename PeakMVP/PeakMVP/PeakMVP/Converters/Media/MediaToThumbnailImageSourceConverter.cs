using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Services.ProfileMedia;
using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters.Media {
    public class MediaToThumbnailImageSourceConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is ProfileMediaDTO mediaValue) {
                if (mediaValue.Mime == ProfileMediaService.MIME_IMAGE_TYPE) {
                    return ImageSource.FromUri(new Uri(mediaValue.Url));
                }
                else if (mediaValue.Mime == ProfileMediaService.MIME_VIDEO_TYPE) {
                    return ImageSource.FromUri(new Uri(mediaValue.ThumbnailUrl));
                }
                else {
                    Debugger.Break();
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new InvalidOperationException("MediaToThumbnailImageSourceConverter.ConvertBack");
    }
}
