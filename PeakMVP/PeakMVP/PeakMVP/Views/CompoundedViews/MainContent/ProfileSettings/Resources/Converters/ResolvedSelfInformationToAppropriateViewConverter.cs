using PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileSettings.SelfInformations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Views.CompoundedViews.MainContent.ProfileSettings.Resources.Converters
{
    public class ResolvedSelfInformationToAppropriateViewConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            View resolvedResult = null;

            if (value is CoachSelfInformationViewModel) {
                resolvedResult = new CoachSelfInformation();
            }
            else if (value is OrganizationSelfInformationViewModel) {
                resolvedResult = new OrganizationSelfInformation();
            }
            else if (value is FanSelfInformationViewModel) {
                resolvedResult = new FanSelfInformation();
            }
            else if (value is PlayerSelfInformationViewModel) {
                resolvedResult = new PlayerSelfInformation();
            }
            else if (value is ParentSelfInformationViewModel) {
                resolvedResult = new ParentSelfInformation();
            }
            else {
                resolvedResult = new Label() {
                    Text = string.Format("Unsuported vm type"),
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
            }

            return resolvedResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("ResolvedSelfInformationToAppropriateViewConverter.ConvertBack");
        }
    }
}
