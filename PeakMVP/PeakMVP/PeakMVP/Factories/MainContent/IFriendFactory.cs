using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.MainContent.Friends;
using PeakMVP.ViewModels.MainContent.Invites;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface IFriendFactory {

        List<FriendshipInviteViewModel> CreateFriends(IEnumerable<FriendshipDTO> friends);
    }
}
