using PeakMVP.Models.Identities.Groups;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Converters {
    public class GroupMemberToFormatedStringConverter : IValueConverter {

        private static readonly string _ANDROID_ICONS_FONT_PATH = "icomoon.ttf#Icomoon";
        private static readonly string _IOS_ICONS_FONT_PATH = "icomoon";

        public string IconCode { get; set; }

        public double IconFontSize { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            Member groupMember = value as Member;
            FormattedString formattedString = new FormattedString();

            if (groupMember == null || groupMember.Profile == null) {
                formattedString.Spans.Add(new Span() { Text = "null" });
            }
            else {
                Span nameSpan = new Span() {
                    Text = string.Format(" {0} {1}", groupMember.Profile.FirstName, groupMember.Profile.LastName)
                };

                if (groupMember.IsGroupOwner) {
                    formattedString.Spans.Add(new Span() {
                        FontFamily = (Device.RuntimePlatform == Device.Android) ? GroupMemberToFormatedStringConverter._ANDROID_ICONS_FONT_PATH : GroupMemberToFormatedStringConverter._IOS_ICONS_FONT_PATH,
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
