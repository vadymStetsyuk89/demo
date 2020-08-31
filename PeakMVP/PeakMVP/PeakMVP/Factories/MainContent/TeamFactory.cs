using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.ViewModels.MainContent.Invites;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public sealed class TeamFactory : ITeamFactory {

        public List<TeamRequestItemViewModel> CreateRequestItems(GetTeamRequestsResponse getTeamRequestsResponse) {
            List<TeamRequestItemViewModel> teamRequestItemViewModels = new List<TeamRequestItemViewModel>();

            foreach (var item in getTeamRequestsResponse) {
                teamRequestItemViewModels.Add(new TeamRequestItemViewModel {
                    Profile = item.Profile,
                    Team = item.Team,
                    Id = item.Id
                });
            }

            return teamRequestItemViewModels;
        }

        public TeamDTO BuildTeam(CreateTeamResponse data) {
            TeamDTO teamDTO = null;

            if (data != null) {
                teamDTO = new TeamDTO() {
                    Coach = data.Coach,
                    CoachId = data.CoachId,
                    CreatedBy = data.CreatedBy,
                    Events = data.Events,
                    Games = data.Games,
                    Id = data.Id,
                    Locations = data.Locations,
                    Members = data.Members,
                    Name = data.Name,
                    Opponents = data.Opponents,
                    Organization = data.Organization,
                    OrganizationId = data.OrganizationId,
                    Owner = data.Owner,
                    Sport = data.Sport
                };
            }

            return teamDTO;
        }

        public TeamDTO BuildTeam(GetTeamByIdResponse data) {
            TeamDTO teamDTO = null;

            if (data != null) {
                teamDTO = new TeamDTO() {
                    Coach = data.Coach,
                    CoachId = data.CoachId,
                    CreatedBy = data.CreatedBy,
                    Events = data.Events,
                    Games = data.Games,
                    Id = data.Id,
                    Locations = data.Locations,
                    Members = data.Members,
                    Name = data.Name,
                    Opponents = data.Opponents,
                    Organization = data.Organization,
                    OrganizationId = data.OrganizationId,
                    Owner = data.Owner,
                    Sport = data.Sport
                };
            }

            return teamDTO;
        }

        public TeamDTO BuildTeam(TeamEndsPartnershipWithOrganizationResponse data) {
            TeamDTO teamDTO = null;

            if (data != null) {
                teamDTO = new TeamDTO() {
                    Coach = data.Coach,
                    CoachId = data.CoachId,
                    CreatedBy = data.CreatedBy,
                    Events = data.Events,
                    Games = data.Games,
                    Id = data.Id,
                    Locations = data.Locations,
                    Members = data.Members,
                    Name = data.Name,
                    Opponents = data.Opponents,
                    Organization = data.Organization,
                    OrganizationId = data.OrganizationId,
                    Owner = data.Owner,
                    Sport = data.Sport
                };
            }

            return teamDTO;
        }
    }
}
