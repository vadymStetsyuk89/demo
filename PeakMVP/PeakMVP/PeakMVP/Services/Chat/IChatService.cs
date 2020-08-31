using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Chat {
    public interface IChatService {
        Task<List<MessageDTO>> GetFriendChatAsync(long friendId, CancellationToken  cancellationToken = default(CancellationToken));

        Task<List<MessageDTO>> GetGroupChatAsync(long groupId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<MessageDTO>> GetTeamChatAsync(long teamId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<MessageDTO>> GetFamilyChatAsync(long profileId, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetUnreadMessagesCountAsync(long companionId, MessagingCompanionType companionType);
    }
}
