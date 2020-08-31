using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.TeamMembers;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public class TeamMemberFactory : ITeamMemberFactory {

        private static readonly string GREEN_ACCEPTANCE_ICON = "PeakMVP.Images.ic_accept_green.png";
        private static readonly string YELLOW_ACCEPTANCE_ICON = "PeakMVP.Images.ic_no_accept_yellow.png";
        private static readonly string ACCEPTED_TEAM_MEMBER_STATUS = "Accepted";

        public TeamMember BuildTeamMember(GetTeamMembersByMemberIdResponse data) {
            return new TeamMember() {
                Assignments = data.Assignments,
                ContactInfo = data.ContactInfo,
                Id = data.Id,
                Joined = data.Joined,
                Member = data.Member,
                Position = data.Position,
                Status = data.Status,
                Team = data.Team
            };
        }

        public TeamMemberViewModel CreateItem(CreateTeamResponse createTeamResponse) {
            return new TeamMemberViewModel {
                Sport = createTeamResponse.Sport,
                Owner = createTeamResponse.Organization?.DisplayName,
                Team = createTeamResponse.Name,
                Joined = DateTime.Now,
                CanBeDeleted = createTeamResponse.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id
            };
        }

        public List<TeamMemberViewModel> CreateItems(IEnumerable<TeamMember> data) {
            List<TeamMemberViewModel> teamMemberViewModels = new List<TeamMemberViewModel>();

            foreach (TeamMember teamMember in data) {
                teamMemberViewModels.Add(new TeamMemberViewModel {
                    Data = teamMember,
                    Icon = (teamMember.Status == ACCEPTED_TEAM_MEMBER_STATUS) ? GREEN_ACCEPTANCE_ICON : YELLOW_ACCEPTANCE_ICON,
                    Sport = teamMember.Team.Sport,
                    Owner = teamMember.Team?.Owner.DisplayName,
                    Team = teamMember.Team.Name,
                    Role = teamMember.Member?.Type,
                    Joined = teamMember.Joined.HasValue ? teamMember.Joined.Value.ToLocalTime() : default(DateTime?),
                    CanBeDeleted = teamMember.Team?.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id
                });
            }

            return teamMemberViewModels;
        }
    }
}
