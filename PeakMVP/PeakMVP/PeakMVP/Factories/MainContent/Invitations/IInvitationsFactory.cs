using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Invites;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent.Invitations {
    public interface IInvitationsFactory {

        List<FriendshipInviteViewModel> BuildFriendshipInvites(IEnumerable<FriendshipDTO> friends);

        List<GroupInviteItemViewModel> BuildGroupsInvites(IEnumerable<GroupDTO> groupInviteDTOs);

        List<TeamInviteItemViewModel> BuildTeamInvites(IEnumerable<TeamDTO> teamMemberDTOs);

        List<ChildInviteItemBaseViewModel> BuildChildrenInvites(InvitesResponse rawInvitesData);

        List<ChildInviteItemBaseViewModel> BuildChildrenInvites(ChangedInvitesSignalArgs rawInvitesData);
    }
}
