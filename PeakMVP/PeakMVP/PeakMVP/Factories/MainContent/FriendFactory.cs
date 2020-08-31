using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.MainContent.Invites;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public class FriendFactory : IFriendFactory {
        public List<FriendshipInviteViewModel> CreateFriends(IEnumerable<FriendshipDTO> friends) {
            List<FriendshipInviteViewModel> friendItemViewModels = new List<FriendshipInviteViewModel>();

            if (friends != null) {
                foreach (var friend in friends) {
                    if (!friend.IsRequest && friend.IsConfirmed) {
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
    }
}
