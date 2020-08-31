using PeakMVP.Models.Identities.Feed;
using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters.Media {
    public class MediafileTypeToImageSourceConverter : IValueConverter {

        private static readonly string _IMAGE_FILE_ICON_RESOURCE_PATH = "resource://PeakMVP.Images.Svg.ic_picture_file.svg";
        private static readonly string _VIDEO_FILE_ICON_RESOURCE_PATH = "resource://PeakMVP.Images.Svg.ic_video_file.svg";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is AttachedFeedMedia attachedFeedMedia) {
                return BuildImageSource(attachedFeedMedia.MediaType);
            }
            else if (value is MediaType mediaType) {
                return BuildImageSource(mediaType);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new InvalidOperationException("MediafileTypeToImageSourceConverter.ConvertBack");

        //private ImageSource BuildImageSource(MediaType mediaType) {
        //    switch (mediaType) {
        //        case MediaType.Picture:
        //            return ImageSource.FromResource(_IMAGE_FILE_ICON_RESOURCE_PATH);
        //        case MediaType.Video:
        //            return ImageSource.FromResource(_VIDEO_FILE_ICON_RESOURCE_PATH);
        //        default:
        //            Debugger.Break();
        //            break;
        //    }
        //    return null;
        //}
        private string BuildImageSource(MediaType mediaType) {
            switch (mediaType) {
                case MediaType.Picture:
                    return _IMAGE_FILE_ICON_RESOURCE_PATH;
                case MediaType.Video:
                    return _VIDEO_FILE_ICON_RESOURCE_PATH;
                default:
                    Debugger.Break();
                    break;
            }

            return null;
        }
    }
}
