using PeakMVP.Models.Sockets.GroupMessaging;
using PeakMVP.Models.Sockets.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.SignalR.GroupMessaging {
    public interface IGroupMessagingService : ISignalService {

        event EventHandler<ReceivedMessageSignalArgs> GroupMessageReceived;

        event EventHandler<ReceivedMessageSignalArgs> TeamMessageReceived;

        event EventHandler<ReceivedMessageSignalArgs> FamilyMessageReceived;

        event EventHandler<ResolvedUnreadGroupMessagesSignalArgs> RefreshedUnreadGroupingMessages;

        Task SendMessageToGroupAsync(string messageText, long companionGroupId, CancellationTokenSource cancellationTokenSource);

        Task SendMessageToTeamAsync(string messageText, long companionTeamId, CancellationTokenSource cancellationTokenSource);

        Task SendMessageToFamilyAsync(string messageText, long companionFamilyId, CancellationTokenSource cancellationTokenSource);

        Task RefreshUnreadMessagesFromGroupingChatsAsync(CancellationTokenSource cancellationTokenSource);

        Task MarkTeamMessageAsReadedAsync(long lastMessageId, long teamId, CancellationTokenSource cancellationTokenSource);

        Task MarkGroupMessageAsReadedAsync(long lastMessageId, long groupId, CancellationTokenSource cancellationTokenSource);

        Task MarkFamilyMessageAsReadedAsync(long lastMessageId, long familyId, CancellationTokenSource cancellationTokenSource);
    }
}
