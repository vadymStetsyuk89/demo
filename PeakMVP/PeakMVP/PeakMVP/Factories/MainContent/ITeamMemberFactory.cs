using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.TeamMembers;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface ITeamMemberFactory {

        TeamMemberViewModel CreateItem(CreateTeamResponse createTeamResponse);

        List<TeamMemberViewModel> CreateItems(IEnumerable<TeamMember> teamMemberDTOs);

        TeamMember BuildTeamMember(GetTeamMembersByMemberIdResponse data);

        //List<TeamInviteItemViewModel> CreateTeamInvites(IEnumerable<TeamDTO> teamMemberDTOs);
    }
}
