using PeakMVP.Helpers;
using PeakMVP.Models.Sockets.Messages;
using Plugin.Connectivity;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace PeakMVP.Services.SignalR.Messages {
    public class MessagesService : SignalServiceBase, IMessagesService {

        private static readonly string _SEND_FRIEND_MESSAGE_ACTION_KEY = "SendMessage";
        private static readonly string _RECEIVE_NEW_MESSAGE = "Message";
        private static readonly string _MARK_AS_READED = "MarkAsReaded";
        private static readonly string _UNREAD_FRIEND_MESSAGES = "UnreadedMessages";

        public event EventHandler<ReceivedMessageSignalArgs> FriendMessageReceived = delegate { };
        public event EventHandler<ResolvedUnreadFriendMessagesSignalArgs> RefreshedUnreadFriendMessages = delegate { };

        public override string SocketHub { get; protected set; } = GlobalSettings.Instance.Endpoints.SignalGateways.MessagesSocketGateway;

        public Task SendMessageToFriendAsync(string messageText, long companionId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            SendMessageSignalArgs sendMessage = new SendMessageSignalArgs() {
                Text = messageText,
                To = companionId
            };

            return _hubConnection.SendCoreAsync(_SEND_FRIEND_MESSAGE_ACTION_KEY, new SendMessageSignalArgs[] { sendMessage }, cancellationTokenSource.Token);
        }

        public Task MarkAsReadAsync(IEnumerable<long> messagesIds, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            MarkFriendMessagesAsReadedSignalArgs markFriendMessagesAsReadedSignalArgs = new MarkFriendMessagesAsReadedSignalArgs() {
                MessageIds = messagesIds?.ToArray()
            };

            return _hubConnection.SendCoreAsync(_MARK_AS_READED, new object[] { markFriendMessagesAsReadedSignalArgs }, cancellationTokenSource.Token);
        }

        public Task RefreshUnreadFriendMessagesAsync(long profileId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            return _hubConnection.SendCoreAsync(_UNREAD_FRIEND_MESSAGES, new object[] { new TODO() { ProfileId = profileId } }, cancellationTokenSource.Token);
        }

        protected override void OnStartListeningToHub() {
            _hubConnection.On<ReceivedMessageSignalArgs>(_RECEIVE_NEW_MESSAGE, (args) => {
                FriendMessageReceived.Invoke(this, args);
            });

            _hubConnection.On<object>(_UNREAD_FRIEND_MESSAGES, (args) => {
                try {
                    ResolvedUnreadFriendMessagesSignalArgs resolvedUnreadFriendMessagesSignalArgs = JsonConvert.DeserializeObject<ResolvedUnreadFriendMessagesSignalArgs>(args.ToString());
                    RefreshedUnreadFriendMessages.Invoke(this, resolvedUnreadFriendMessagesSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException("MessagesService on SignalR UnreadedMessages", exc);
                }
            });
        }
    }
}
