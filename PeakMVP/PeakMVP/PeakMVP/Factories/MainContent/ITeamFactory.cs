using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.ViewModels.MainContent.Invites;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface ITeamFactory {

        List<TeamRequestItemViewModel> CreateRequestItems(GetTeamRequestsResponse getTeamRequestsResponse);

        TeamDTO BuildTeam(CreateTeamResponse data);

        TeamDTO BuildTeam(TeamEndsPartnershipWithOrganizationResponse data);

        TeamDTO BuildTeam(GetTeamByIdResponse data);
    }
}
