using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Sockets.GroupMessaging;
using PeakMVP.Models.Sockets.Messages;
using Plugin.Connectivity;

namespace PeakMVP.Services.SignalR.GroupMessaging {
    public class GroupMessagingService : SignalServiceBase, IGroupMessagingService {

        private static readonly string _RECEIVE_NEW_GROUP_MESSAGE = "GroupMessage";
        private static readonly string _SEND_GROUP_MESSAGE_ACTION_KEY = "SendGroupMessage";
        private static readonly string _RECEIVE_NEW_TEAM_MESSAGE = "TeamMessage";
        private static readonly string _SEND_TEAM_MESSAGE_ACTION_KEY = "SendTeamMessage";
        private static readonly string _RECEIVE_NEW_FAMILY_MESSAGE = "FamilyMessage";
        private static readonly string _SEND_FAMILY_MESSAGE_ACTION_KEY = "SendFamilyMessage";
        private static readonly string _FETCH_UNREAD_MESSAGES = "UnreadedMessages";
        private static readonly string _UNREAD_GROUP_MESSAGES = "UnreadedGroupMessages";
        private static readonly string _MARK_AS_READED_TEAM_MESSAGE = "MarkAsReadedTeamMessage";
        private static readonly string _MARK_AS_READED_GROUP_MESSAGE = "MarkAsReadedGroupMessage";
        private static readonly string _MARK_AS_READED_FAMILY_MESSAGE = "MarkAsReadedFamilyMessage";

        public event EventHandler<ReceivedMessageSignalArgs> GroupMessageReceived = delegate { };
        public event EventHandler<ReceivedMessageSignalArgs> TeamMessageReceived = delegate { };
        public event EventHandler<ReceivedMessageSignalArgs> FamilyMessageReceived = delegate { };
        public event EventHandler<ResolvedUnreadGroupMessagesSignalArgs> RefreshedUnreadGroupingMessages = delegate { };

        public override string SocketHub { get; protected set; } = GlobalSettings.Instance.Endpoints.SignalGateways.GroupMessagesSocketGateway;

        public Task SendMessageToGroupAsync(string messageText, long companionGroupId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            SendMessageSignalArgs sendMessage = new SendMessageSignalArgs() {
                Text = messageText,
                To = companionGroupId
            };

            return _hubConnection.SendCoreAsync(_SEND_GROUP_MESSAGE_ACTION_KEY, new SendMessageSignalArgs[] { sendMessage }, cancellationTokenSource.Token);
        }

        public Task SendMessageToFamilyAsync(string messageText, long companionFamilyId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            SendFamilyMessageSignalArgs sendMessage = new SendFamilyMessageSignalArgs() {
                Text = messageText,
                FamilyId = companionFamilyId
            };

            return _hubConnection.SendCoreAsync(_SEND_FAMILY_MESSAGE_ACTION_KEY, new SendFamilyMessageSignalArgs[] { sendMessage }, cancellationTokenSource.Token);
        }

        public Task SendMessageToTeamAsync(string messageText, long companionTeamId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            SendTeamMessageSignalArgs sendMessage = new SendTeamMessageSignalArgs() {
                Text = messageText,
                TeamId = companionTeamId
            };

            return _hubConnection.SendCoreAsync(_SEND_TEAM_MESSAGE_ACTION_KEY, new SendTeamMessageSignalArgs[] { sendMessage }, cancellationTokenSource.Token);
        }

        public Task MarkTeamMessageAsReadedAsync(long lastMessageId, long teamId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            MarkTeamMessageAsReadedSignalArgs markTeamMessageAsReadedSignalArgs = new MarkTeamMessageAsReadedSignalArgs() {
                TeamId = teamId,
                MessageId = lastMessageId
            };

            return _hubConnection.SendCoreAsync(_MARK_AS_READED_TEAM_MESSAGE, new object[] { markTeamMessageAsReadedSignalArgs }, cancellationTokenSource.Token);
        }

        public Task MarkGroupMessageAsReadedAsync(long lastMessageId, long groupId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            MarkGroupMessageAsReadedSignalArgs markGroupMessageAsReadedSignalArgs = new MarkGroupMessageAsReadedSignalArgs() {
                GroupId = groupId,
                MessageId = lastMessageId
            };

            return _hubConnection.SendCoreAsync(_MARK_AS_READED_GROUP_MESSAGE, new object[] { markGroupMessageAsReadedSignalArgs }, cancellationTokenSource.Token);
        }

        public Task MarkFamilyMessageAsReadedAsync(long lastMessageId, long familyId, CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            MarkFamilyMessageAsReadedSignalArgs markFamilyMessageAsReadedSignalArgs = new MarkFamilyMessageAsReadedSignalArgs() {
                FamilyId = familyId,
                MessageId = lastMessageId
            };

            return _hubConnection.SendCoreAsync(_MARK_AS_READED_FAMILY_MESSAGE, new object[] { markFamilyMessageAsReadedSignalArgs }, cancellationTokenSource.Token);
        }

        public Task RefreshUnreadMessagesFromGroupingChatsAsync(CancellationTokenSource cancellationTokenSource) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            return _hubConnection.SendCoreAsync(_FETCH_UNREAD_MESSAGES, new object[] { }, cancellationTokenSource.Token);
        }

        protected override void OnStartListeningToHub() {
            _hubConnection.On<ReceivedMessageSignalArgs>(_RECEIVE_NEW_GROUP_MESSAGE, (args) => {
                GroupMessageReceived.Invoke(this, args);
            });

            _hubConnection.On<ReceivedMessageSignalArgs>(_RECEIVE_NEW_TEAM_MESSAGE, (args) => {
                TeamMessageReceived.Invoke(this, args);
            });

            _hubConnection.On<ReceivedMessageSignalArgs>(_RECEIVE_NEW_FAMILY_MESSAGE, (args) => {
                FamilyMessageReceived.Invoke(this, args);
            });

            _hubConnection.On<object>(_UNREAD_GROUP_MESSAGES, (args) => {
                try {
                    ResolvedUnreadGroupMessagesSignalArgs resolvedUnreadGroupMessagesSignalArgs = JsonConvert.DeserializeObject<ResolvedUnreadGroupMessagesSignalArgs>(args.ToString());
                    RefreshedUnreadGroupingMessages.Invoke(this, resolvedUnreadGroupMessagesSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException("GroupMessagingService on SignalR UnreadedMessages", exc);
                }
            });
        }
    }
}
