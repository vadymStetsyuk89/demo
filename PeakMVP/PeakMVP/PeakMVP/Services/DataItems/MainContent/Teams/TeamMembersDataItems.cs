using PeakMVP.Models.DataItems.MainContent.Teams;
using System.Collections.Generic;

namespace PeakMVP.Services.DataItems.MainContent.Teams {
    public class TeamMembersDataItems : ITeamMembersDataItems {

        private static readonly string _ALL_MEMBERS_FILTER_TITLE = "All";
        private static readonly string _PLAYERS_MEMBERS_FILTER_TITLE = "Players";
        private static readonly string _STAFF_MEMBERS_FILTER_TITLE = "Staff";

        public List<TeamMemberFilterDataItem> BuildTeamMemberFilterItems() {
            return new List<TeamMemberFilterDataItem>() {
                new TeamMemberFilterDataItem() {
                    Filter = TeamMemberFilters.All,
                    Title = _ALL_MEMBERS_FILTER_TITLE
                },
                new TeamMemberFilterDataItem() {
                    Filter = TeamMemberFilters.Players,
                    Title = _PLAYERS_MEMBERS_FILTER_TITLE
                },
                new TeamMemberFilterDataItem() {
                    Filter = TeamMemberFilters.Staff,
                    Title = _STAFF_MEMBERS_FILTER_TITLE
                }
            };
        }
    }
}
