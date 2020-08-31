using PeakMVP.Models.Rests.DTOs;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class UserAvatarConverter : IValueConverter {

        private static readonly string _DEFAULT_AVATAR_STUB = "PeakMVP.Images.ic_profile-avatar_white.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            ImageSource imageSource = null;

            if (value is ProfileDTO profile) {
                if (profile.Avatar == null) {
                    imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
                }
                else {
                    string path = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, string.Format("/{0}", profile.Avatar.Url));

                    try {
                        imageSource = ImageSource.FromUri(new Uri(path));
                    }
                    catch {
                        imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
                    }
                }
            }
            else {
                try {
                    if (!(value is string valueString) || string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString())) {
                        imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
                    }
                    else {
                        Uri outUri = null;
                        string path = null;

                        if (!Uri.TryCreate(valueString, UriKind.Absolute, out outUri)) {
                            path = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, string.Format("/{0}", value.ToString()));
                        }
                        else {
                            path = valueString;
                        }

                        imageSource = ImageSource.FromUri(new Uri(path));
                    }
                }
                catch {
                    imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
                }
            }

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("UserAvatarConverter.ConvertBack");

        //private static readonly string _DEFAULT_AVATAR_STUB = "PeakMVP.Images.ic_profile-avatar_white.png";

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        //    ImageSource imageSource = null;

        //    if (value is ProfileDTO profile) {
        //        if (profile.Avatars == null || !(profile.Avatars.Any())) {
        //            imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
        //        }
        //        else {
        //            string path = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, string.Format("/{0}", profile.Avatars.LastOrDefault().Url));

        //            try {
        //                imageSource = ImageSource.FromUri(new Uri(path));
        //            }
        //            catch {
        //                imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
        //            }
        //        }
        //    }
        //    else {
        //        if (!(value is string) || string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString())) {
        //            imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
        //        }
        //        else {
        //            string path = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, string.Format("/{0}", value.ToString()));

        //            try {
        //                imageSource = ImageSource.FromUri(new Uri(path));
        //            }
        //            catch {
        //                imageSource = ImageSource.FromResource(_DEFAULT_AVATAR_STUB);
        //            }
        //        }
        //    }

        //    return imageSource;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("UserAvatarConverter.ConvertBack");
    }
}
