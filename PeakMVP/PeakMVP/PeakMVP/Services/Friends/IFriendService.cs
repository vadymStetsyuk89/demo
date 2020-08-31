using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Friends;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Friends {
    public interface IFriendService {
        Task<List<FriendshipDTO>> GetAllFriendsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<AddFriendResponse> AddFriendAsync(string friendShortId, CancellationToken cancellationToken = default(CancellationToken));

        Task<GetFriendByIdResponse> GetFriendByIdAsync(long friendId, CancellationToken cancellationToken = default(CancellationToken));

        Task<DeleteFriendResponse> RequestDeleteAsync(long friendId, CancellationToken cancellationToken = default(CancellationToken), long? childProfileId = null);

        Task<ConfirmFriendResponse> RequestConfirmAsync(long friendId, CancellationToken cancellationToken = default(CancellationToken), long? childProfileId = null);
    }
}
