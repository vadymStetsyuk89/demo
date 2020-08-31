using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.Search;
using PeakMVP.Models.Rests.Responses.Search;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace PeakMVP.Factories.MainContent {
    public class FoundUserGroupDataItemFactory : IFoundUserGroupDataItemFactory {

        public static readonly string PLAYER_TYPE_CONSTANT_VALUE = "Player";
        public static readonly string PARENT_TYPE_CONSTANT_VALUE = "Parent";
        public static readonly string ORGANIZATION_TYPE_CONSTANT_VALUE = "Organization";
        public static readonly string COACH_TYPE_CONSTANT_VALUE = "Coach";
        public static readonly string FAN_TYPE_CONSTANT_VALUE = "Fan";
        public static readonly string TEAM_TYPE_CONSTANT_VALUE = "Team";

        public FoundGroupDataItem CreateSingleGroup() {
            return new FoundGroupDataItem() {
                FoundUsers = new List<FoundSingleDataItemBase>()
            };
        }

        public List<FoundGroupDataItem> BuildFoundGroupDataItems(SearchResponse searchResponse) {
            List<FoundGroupDataItem> result = new List<FoundGroupDataItem>();

            List<FoundGroupDataItem> foundUserGroups = BuildUserBasedGroupDataItem(searchResponse.Profiles);
            FoundGroupDataItem foundTeamGroups = BuildTeamBasedGroupDataItem(searchResponse.Teams);

            result.InsertRange(result.Count, foundUserGroups);

            if (foundTeamGroups != null) {
                result.Add(foundTeamGroups);
            }

            return result;
        }

        private List<FoundGroupDataItem> BuildUserBasedGroupDataItem(ProfilesNodeDTO data) {
            List<FoundGroupDataItem> result = new List<FoundGroupDataItem>();

            if (data.Coach != null && data.Coach.Any()) {
                result.Add(BuildOneFoundUserGroupDataItem(data.Coach, COACH_TYPE_CONSTANT_VALUE));
            }

            if (data.Fan != null && data.Fan.Any()) {
                result.Add(BuildOneFoundUserGroupDataItem(data.Fan, FAN_TYPE_CONSTANT_VALUE));
            }

            if (data.Organization != null && data.Organization.Any()) {
                result.Add(BuildOneFoundUserGroupDataItem(data.Organization, ORGANIZATION_TYPE_CONSTANT_VALUE));
            }

            if (data.Parent != null && data.Parent.Any()) {
                result.Add(BuildOneFoundUserGroupDataItem(data.Parent, PARENT_TYPE_CONSTANT_VALUE));
            }

            if (data.Player != null && data.Player.Any()) {
                result.Add(BuildOneFoundUserGroupDataItem(data.Player, PLAYER_TYPE_CONSTANT_VALUE));
            }

            return result;
        }

        private FoundGroupDataItem BuildTeamBasedGroupDataItem(IEnumerable<TeamDTO> data) {
            FoundGroupDataItem result = null;

            if (data != null && data.Any()) {
                result = new FoundGroupDataItem() {
                    FoundUsers = new List<FoundSingleDataItemBase>(),
                    IconSourcePath = ResolveIcon(FoundUserGroupDataItemFactory.TEAM_TYPE_CONSTANT_VALUE),
                    GroupType = TEAM_TYPE_CONSTANT_VALUE
                };

                data.ForEach<TeamDTO>((tDTO) => result.FoundUsers.Add(BuildOneFoundSingleTeamDataItem(tDTO)));

                result.UsersCount = result.FoundUsers.Count;
            }

            return result;
        }

        private FoundGroupDataItem BuildOneFoundUserGroupDataItem(IEnumerable<ProfileDTO> profiles, string groupType) {
            FoundGroupDataItem result = new FoundGroupDataItem();
            result.IconSourcePath = ResolveIcon(groupType);

            List<FoundSingleDataItemBase> singleItems = new List<FoundSingleDataItemBase>();
            profiles.ForEach<ProfileDTO>(pDTO => singleItems.Add(BuildOneFoundSingleUserDataItem(pDTO)));

            result.FoundUsers = singleItems;
            result.UsersCount = singleItems.Count;
            result.GroupType = groupType;

            return result;
        }

        private FoundSingleUserDataItem BuildOneFoundSingleUserDataItem(ProfileDTO profileDTO) {
            FoundSingleUserDataItem result = new FoundSingleUserDataItem();
            result.UserProfile = profileDTO;
            result.Type = profileDTO.Type;
            result.DisplayText = profileDTO.DisplayName;
            result.Icon = (profileDTO.Avatar != null && profileDTO.Avatar?.Url != null)
                ? string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, profileDTO.Avatar?.Url)
                : null;

            return result;
        }

        private FoundSingleTeamDataItem BuildOneFoundSingleTeamDataItem(TeamDTO teamDTO) {
            FoundSingleTeamDataItem result = new FoundSingleTeamDataItem();
            result.Team = teamDTO;
            result.DisplayText = teamDTO.Name;
            result.Type = TEAM_TYPE_CONSTANT_VALUE;

            return result;
        }

        private string ResolveIcon(string targetType) {
            string result = "";

            if (targetType == FoundUserGroupDataItemFactory.COACH_TYPE_CONSTANT_VALUE) {
                result = "PeakMVP.Images.ic_coach.png";
            }
            else if (targetType == FoundUserGroupDataItemFactory.ORGANIZATION_TYPE_CONSTANT_VALUE) {
                result = "PeakMVP.Images.ic_org.png";
            }
            else if (targetType == FoundUserGroupDataItemFactory.PARENT_TYPE_CONSTANT_VALUE || targetType == FoundUserGroupDataItemFactory.FAN_TYPE_CONSTANT_VALUE) {
                result = "PeakMVP.Images.ic_parent.png";
            }
            else if (targetType == FoundUserGroupDataItemFactory.PLAYER_TYPE_CONSTANT_VALUE) {
                result = "PeakMVP.Images.ic_player-13.png";
            }
            else if (targetType == FoundUserGroupDataItemFactory.TEAM_TYPE_CONSTANT_VALUE) {
                result = "PeakMVP.Images.ic_org.png";
            }

            return result;
        }
    }
}
