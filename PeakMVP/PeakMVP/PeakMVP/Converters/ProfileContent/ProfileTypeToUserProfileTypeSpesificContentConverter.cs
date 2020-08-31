using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileContent;
using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters.ProfileContent {
    public class ProfileTypeToUserProfileTypeSpesificContentConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            View view = null;

            if (value is ProfileType profileType) {
                switch (profileType) {
                    case ProfileType.Player:
                        view = new PlayerProfileContentView();
                        break;
                    case ProfileType.Parent:
                        view = new ParentProfileContentView();
                        break;
                    case ProfileType.Organization:
                        view = new OrganizationProfileContentView();
                        break;
                    case ProfileType.Coach:
                        view = new CoachProfileContentView();
                        break;
                    case ProfileType.Fan:
                        view = null;
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("ProfileTypeToUserProfileTypeSpesificContentConverter.ConvertBack");

    }
}
