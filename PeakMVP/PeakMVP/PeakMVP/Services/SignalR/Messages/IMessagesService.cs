using PeakMVP.Models.Sockets.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.SignalR.Messages {
    public interface IMessagesService: ISignalService {

        event EventHandler<ReceivedMessageSignalArgs> FriendMessageReceived;

        event EventHandler<ResolvedUnreadFriendMessagesSignalArgs> RefreshedUnreadFriendMessages;

        Task SendMessageToFriendAsync(string messageText, long companionId, CancellationTokenSource cancellationTokenSource);

        Task MarkAsReadAsync(IEnumerable<long> messagesIds, CancellationTokenSource cancellationTokenSource);

        Task RefreshUnreadFriendMessagesAsync(long profileId, CancellationTokenSource cancellationTokenSource);
    }
}
