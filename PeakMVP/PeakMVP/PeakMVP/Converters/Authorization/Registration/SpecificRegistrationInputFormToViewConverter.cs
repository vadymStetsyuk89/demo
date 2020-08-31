using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Views.Authorization.Registration;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters.Authorization.Registration {
    public class SpecificRegistrationInputFormToViewConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            View resolvedResult = null;

            if (value != null) {
                switch (((ProfileTypeItem)value).ProfileType) {
                    case ProfileType.Player:
                        resolvedResult = null;
                        break;
                    case ProfileType.Parent:
                        resolvedResult = new ParentRegistrationInput();
                        break;
                    case ProfileType.Organization:
                        resolvedResult = new OrganizationRegistrationInput();
                        break;
                    case ProfileType.Coach:
                        resolvedResult = new CoachRegistrationInput();
                        break;
                    case ProfileType.Fan:
                        resolvedResult = null;
                        break;
                    default:
                        resolvedResult = null;
                        break;
                }
            }

            return resolvedResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("SpecificRegistrationInputFormToViewConverter.ConvertBack");
    }
}
