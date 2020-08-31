using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Views.CompoundedViews.MainContent.Teams.Resources.Converters {
    public class TeamMemberToFormatedStringConverter : IValueConverter {

        private static readonly string _ANDROID_ICONS_FONT_PATH = "icomoon.ttf#Icomoon";
        private static readonly string _IOS_ICONS_FONT_PATH = "icomoon";

        public string IconCode { get; set; }

        public double IconFontSize { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            TeamMember teamMember = value as TeamMember;
            FormattedString formattedString = new FormattedString();

            if (teamMember == null || teamMember.Member == null || teamMember.Team == null || teamMember.Team.Owner == null) {
                formattedString.Spans.Add(new Span() { Text = "null" });
            }
            else {
                Span nameSpan = new Span() {
                    Text = string.Format(" {0} {1}", teamMember.Member.FirstName, teamMember.Member.LastName)
                };

                if (teamMember.Team.Owner.Id == teamMember.Member.Id) {
                    formattedString.Spans.Add(new Span() {
                        FontFamily = (Device.RuntimePlatform == Device.Android) ? TeamMemberToFormatedStringConverter._ANDROID_ICONS_FONT_PATH : TeamMemberToFormatedStringConverter._IOS_ICONS_FONT_PATH,
                        Text = IconCode,
                        FontSize = IconFontSize
                    });
                }

                formattedString.Spans.Add(nameSpan);
            }

            return formattedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("GroupMemberToFormatedStringConverter.ConvertBack");
        }
    }
}
