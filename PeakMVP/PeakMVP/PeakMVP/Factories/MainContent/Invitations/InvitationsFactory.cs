using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Invites;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.Factories.MainContent.Invitations {
    public class InvitationsFactory : IInvitationsFactory {

        public List<FriendshipInviteViewModel> BuildFriendshipInvites(IEnumerable<FriendshipDTO> friends) {
            List<FriendshipInviteViewModel> friendItemViewModels = new List<FriendshipInviteViewModel>();

            if (friends != null) {
                foreach (var friend in friends) {
                    if (friend.IsRequest) {
                        friendItemViewModels.Add(new FriendshipInviteViewModel {
                            Avatar = (friend.Profile.Avatar?.Url != null)
                           ? string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, friend.Profile.Avatar?.Url)
                           : null,
                            FullName = friend.Profile.DisplayName,
                            FriendId = friend.Profile.Id,
                            IsRequest = friend.IsRequest,
                            ProfileType = friend.Profile.Type,
                            Profile = friend.Profile
                        });
                    }
                }
            }

            return friendItemViewModels;
        }

        public List<TeamInviteItemViewModel> BuildTeamInvites(IEnumerable<TeamDTO> teamInviteDTOs) {
            List<TeamInviteItemViewModel> teamInviteItemViewModels = new List<TeamInviteItemViewModel>();

            if (teamInviteDTOs != null) {
                foreach (var teamInvite in teamInviteDTOs) {
                    teamInviteItemViewModels.Add(new TeamInviteItemViewModel {
                        TeamId = teamInvite.Id,
                        TeamName = teamInvite.Name,
                        Owner = teamInvite.Owner?.DisplayName
                    });
                }
            }

            return teamInviteItemViewModels;
        }

        public List<GroupInviteItemViewModel> BuildGroupsInvites(IEnumerable<GroupDTO> groupInviteDTOs) {
            List<GroupInviteItemViewModel> groupRequestItemViewModels = new List<GroupInviteItemViewModel>();

            if (groupInviteDTOs != null) {
                foreach (var item in groupInviteDTOs) {
                    groupRequestItemViewModels.Add(new GroupInviteItemViewModel {
                        GroupName = item.Name,
                        GroupId = item.Id
                    });
                }
            }

            return groupRequestItemViewModels;
        }

        public List<ChildInviteItemBaseViewModel> BuildChildrenInvites(InvitesResponse rawInvitesData) {
            List<ChildInviteItemBaseViewModel> childrenInvites = null;

            if (rawInvitesData != null) {
                childrenInvites = BuildChildInvites(rawInvitesData.ChildInvites);
            }
            else {
                childrenInvites = new List<ChildInviteItemBaseViewModel>();
            }

            return childrenInvites;
        }

        public List<ChildInviteItemBaseViewModel> BuildChildrenInvites(ChangedInvitesSignalArgs rawInvitesData) {
            List<ChildInviteItemBaseViewModel> childrenInvites = null;

            if (rawInvitesData != null ) {
                childrenInvites = BuildChildInvites(rawInvitesData.ChildInvites);
            }
            else {
                childrenInvites = new List<ChildInviteItemBaseViewModel>();
            }

            return childrenInvites;
        }

        private List<ChildInviteItemBaseViewModel> BuildChildInvites(IEnumerable<ChildInvitesDTO> childInvitesData) {
            List<ChildInviteItemBaseViewModel> childrenInvites = null;

            if (childInvitesData != null) {
                childrenInvites = childInvitesData
                    .SelectMany<ChildInvitesDTO, ChildInviteItemBaseViewModel>(childInviteDTO => {
                        List<ChildInviteItemBaseViewModel> oneChildInvites = new List<ChildInviteItemBaseViewModel>();

                        oneChildInvites.AddRange(childInviteDTO.FriendshipInvites
                            .Select<FriendshipDTO, ChildFriendshipInviteItemViewModel>(friendshipInvite => {
                                ChildFriendshipInviteItemViewModel childFriendship = ViewModelLocator.Resolve<ChildFriendshipInviteItemViewModel>();
                                childFriendship.UploadParticipants((IInviteTo)friendshipInvite.Profile, childInviteDTO.Child);

                                return childFriendship;
                            }));

                        oneChildInvites.AddRange(childInviteDTO.GroupInvites
                            .Select<GroupDTO, ChildInviteToGroupItemViewModel>(groupInvite => {
                                ChildInviteToGroupItemViewModel childToGroup = ViewModelLocator.Resolve<ChildInviteToGroupItemViewModel>();
                                childToGroup.UploadParticipants((IInviteTo)groupInvite, childInviteDTO.Child);

                                return childToGroup;
                            }));

                        oneChildInvites.AddRange(childInviteDTO.TeamInvites
                            .Select<TeamDTO, ChildInviteToTeamItemViewModel>(teamInvite => {
                                ChildInviteToTeamItemViewModel childToTeam = ViewModelLocator.Resolve<ChildInviteToTeamItemViewModel>();
                                childToTeam.UploadParticipants((IInviteTo)teamInvite, childInviteDTO.Child);

                                return childToTeam;
                            }));

                        return oneChildInvites;
                    })
                    .ToList<ChildInviteItemBaseViewModel>();
            }
            else {
                childrenInvites = new List<ChildInviteItemBaseViewModel>();
            }

            return childrenInvites;
        }
    }
}
