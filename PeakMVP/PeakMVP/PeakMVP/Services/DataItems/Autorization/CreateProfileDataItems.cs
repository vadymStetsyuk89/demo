using PeakMVP.Models.DataItems.Autorization;
using System.Collections.ObjectModel;

namespace PeakMVP.Services.DataItems.Autorization {
    internal sealed class CreateProfileDataItems : ICreateProfileDataItems<ProfileTypeItem> {

        private static readonly string _PLAYER_OVER_TITLE_VALUE = "Player (13+)";
        private static readonly string _PARENT_TITLE_VALUE = "Parent";
        private static readonly string _FAN_TITLE_VALUE = "Fan";
        private static readonly string _ORGANIZATION_TITLE_VALUE = "Organization";
        private static readonly string _COACH_TITLE_VALUE = "Coach";

        private static readonly string _PLAYER_OVER_IMAGE_VALUE = "PeakMVP.Images.ic_player-13.png";
        private static readonly string _PARENT_IMAGE_VALUE = "PeakMVP.Images.ic_parent.png";
        private static readonly string _FAN_IMAGE_VALUE = "PeakMVP.Images.ic_parent.png";
        private static readonly string _ORGANIZATION_IMAGE_VALUE = "PeakMVP.Images.ic_org.png";
        private static readonly string _COACH_IMAGE_VALUE = "PeakMVP.Images.ic_coach.png";

        public ObservableCollection<ProfileTypeItem> BuildDataItems() {
            return new ObservableCollection<ProfileTypeItem>() {
                new ProfileTypeItem {
                    Icon = _PLAYER_OVER_IMAGE_VALUE,
                    Name = _PLAYER_OVER_TITLE_VALUE,
                    ProfileType = ProfileType.Player
                },
                new ProfileTypeItem {
                    Icon = _PARENT_IMAGE_VALUE,
                    Name = _PARENT_TITLE_VALUE,
                    ProfileType = ProfileType.Parent
                },
                new ProfileTypeItem {
                    Icon = _FAN_IMAGE_VALUE,
                    Name = _FAN_TITLE_VALUE,
                    ProfileType = ProfileType.Fan
                },
                new ProfileTypeItem {
                    Icon = _ORGANIZATION_IMAGE_VALUE,
                    Name = _ORGANIZATION_TITLE_VALUE,
                    ProfileType = ProfileType.Organization
                },
                new ProfileTypeItem {
                    Icon = _COACH_IMAGE_VALUE,
                    Name = _COACH_TITLE_VALUE,
                    ProfileType = ProfileType.Coach
                },
            };
        }
    }
}
