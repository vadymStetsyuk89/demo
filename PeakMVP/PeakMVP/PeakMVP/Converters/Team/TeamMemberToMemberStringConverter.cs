using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Converters.Team {
    public class TeamMemberToMemberStringConverter : IValueConverter {

        private static readonly string _CANT_CONVERT_STUB_ERROR = "Can't convert";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                return string.Format("{0} - {1}", ((TeamMember)value).Member.DisplayName, ((TeamMember)value).Member.Type);
            }
            catch {
                return _CANT_CONVERT_STUB_ERROR;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException("TeamMemberToMemberStringConverter.ConvertBack");
    }
}
