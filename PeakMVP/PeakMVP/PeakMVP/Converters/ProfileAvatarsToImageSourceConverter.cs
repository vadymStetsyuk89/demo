using PeakMVP.Models.Rests.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class ProfileAvatarsToImageSourceConverter : IValueConverter {

        private static readonly string _STUMB_PROFILE_AVATAR_IMAGE_PATH = "PeakMVP.Images.ic_avatar_stumb.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is IEnumerable<MediaDTO>) {
                return (((IEnumerable<MediaDTO>)value).Any())
                    ? ImageSource.FromUri(new Uri(string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, ((IEnumerable<MediaDTO>)value).Last().Url)))
                    : ImageSource.FromResource(GlobalSettings.Instance.UserProfile.DEFAULT_AVATAR);
            }
            else {
                return ImageSource.FromResource(_STUMB_PROFILE_AVATAR_IMAGE_PATH);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("ProfileAvatarsToImageSourceConverter.ConvertBack");
        }
    }
}
