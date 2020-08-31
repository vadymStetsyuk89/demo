using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.Messages;
using PeakMVP.Services.Chat;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.ViewModels.MainContent.Messenger.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public class ConversationTabViewModel : MessengerTabViewModelBase {

        public static string _CANT_START_CONVERSATION = "Can't start conversation";
        protected static readonly string _GROUP_MESSAGE_HANDLING_ERROR = "Can't handle new message in group conversation";
        private static readonly string _UNRESOLVED_COMPANION_TYPE_ERROR = "Unresolved target companion type";
        private static readonly string _UNRESOLVED_COMPANION_NAME_ERROR = "Unresolved name";
        private static readonly string _FRIEND_MESSAGE_HANDLING_ERROR = "Can't handle new message in conversation with friend";
        private static readonly string _RESOLVE_RELATION_BETWEEN_MESSAGE_AND_CONVERSATION_ERROR = "Can't resolve relation between message and current conversation";

        protected readonly IGroupMessagingService _groupMessagingService;
        protected readonly IMessageItemFactory _messageItemFactory;
        private readonly IChatService _chatService;
        private readonly IMessagesService _messagesService;

        private CancellationTokenSource _getConversationHistoryCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _sendMessageCancellationTokenSource = new CancellationTokenSource();

        public ConversationTabViewModel(
            IChatService chatService,
            IMessageItemFactory messageItemFactory,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService) {

            _chatService = chatService;
            _messageItemFactory = messageItemFactory;
            _messagesService = messagesService;
            _groupMessagingService = groupMessagingService;

            CanBeClosed = true;
        }

        public ICommand CloseTabCommand => new Command(async () => {
            Dispose();

            await NavigationService.LastPageViewModel.InitializeAsync(new CloseConversationTabArgs() { TabToClose = this });
        });

        public ICommand SendMessageCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _sendMessageCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _sendMessageCancellationTokenSource;

            try {
                if (TargetCompanion.Companion is ProfileDTO profileDTO) {
                    await _messagesService.SendMessageToFriendAsync(InputMessageToSend, TargetCompanion.Companion.Id, cancellationTokenSource);
                }
                else if (TargetCompanion.Companion is GroupDTO groupDTO) {
                    await _groupMessagingService.SendMessageToGroupAsync(InputMessageToSend, TargetCompanion.Companion.Id, cancellationTokenSource);
                }
                else if (TargetCompanion.Companion is TeamDTO teamDTO) {
                    await _groupMessagingService.SendMessageToTeamAsync(InputMessageToSend, TargetCompanion.Companion.Id, cancellationTokenSource);
                }
                else if (TargetCompanion.Companion is FamilyDTO familyDTO) {
                    await _groupMessagingService.SendMessageToFamilyAsync(InputMessageToSend, TargetCompanion.Companion.Id, cancellationTokenSource);
                }

                InputMessageToSend = "";
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }
        });

        private bool _isSelected;
        public override bool IsSelected {
            get => _isSelected;
            set {
                _isSelected = value;
                MarkAsReaded();
            }
        }

        private PossibleConverstionItem _targetCompanion;
        public PossibleConverstionItem TargetCompanion {
            get => _targetCompanion;
            protected set {
                SetProperty<PossibleConverstionItem>(ref _targetCompanion, value);

                string title = "";

                if (value.Companion is ProfileDTO profileDTO) {
                    title = string.Format("{0} {1}", profileDTO.FirstName, profileDTO.LastName);
                }
                else if (value.Companion is GroupDTO groupDTO) {
                    title = groupDTO.Name;
                }
                else if (value.Companion is TeamDTO teamDTO) {
                    title = teamDTO.Name;
                }
                else if (value.Companion is FamilyDTO familyDTO) {
                    title = value.CompanionName;
                }
                else {
                    title = _UNRESOLVED_COMPANION_NAME_ERROR;
                }

                Title = title;
            }
        }

        private ObservableCollection<MessageItem> _messagesHistory = new ObservableCollection<MessageItem>();
        public ObservableCollection<MessageItem> MessagesHistory {
            get => _messagesHistory;
            private set => SetProperty<ObservableCollection<MessageItem>>(ref _messagesHistory, value);
        }

        private string _inputMessageToSend;
        public string InputMessageToSend {
            get => _inputMessageToSend;
            set => SetProperty<string>(ref _inputMessageToSend, value);
        }

        public override Task AskToRefreshAsync() => GetConversationHistoryAsync();

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getConversationHistoryCancellationTokenSource);
            ResetCancellationTokenSource(ref _sendMessageCancellationTokenSource);
        }

        public Task TryToStartConversationAsync() {
            try {
                return GetConversationHistoryAsync();
            }
            catch (Exception exc) {
                throw new InvalidOperationException(_CANT_START_CONVERSATION, exc);
            }
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getConversationHistoryCancellationTokenSource);
            ResetCancellationTokenSource(ref _sendMessageCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is PossibleConverstionItem possibleConverstionItem) {
                TargetCompanion = possibleConverstionItem;

                ExecuteActionWithBusy(GetConversationHistoryAsync);
            }

            return base.InitializeAsync(navigationData);
        }

        protected async void MarkAsReaded() {
            if (IsSelected) {
                if (MessagesHistory != null && MessagesHistory.Any()) {
                    switch (TargetCompanion.MessagingCompanionType) {
                        case MessagingCompanionType.Family:
                            await _groupMessagingService.MarkFamilyMessageAsReadedAsync(MessagesHistory.Last<MessageItem>().Data.Id, TargetCompanion.Companion.Id, new CancellationTokenSource());
                            await _groupMessagingService.RefreshUnreadMessagesFromGroupingChatsAsync(new CancellationTokenSource());
                            break;
                        case MessagingCompanionType.Friend:
                            if (MessagesHistory != null && MessagesHistory.Any()) {
                                await _messagesService.MarkAsReadAsync(MessagesHistory.Where<MessageItem>(message => !message.Data.Seen).Select<MessageItem, long>(message => message.Data.Id).ToArray(), new CancellationTokenSource());
                                await _messagesService.RefreshUnreadFriendMessagesAsync(GlobalSettings.Instance.UserProfile.Id, new CancellationTokenSource());
                            }
                            break;
                        case MessagingCompanionType.Team:
                            await _groupMessagingService.MarkTeamMessageAsReadedAsync(MessagesHistory.Last<MessageItem>().Data.Id, TargetCompanion.Companion.Id, new CancellationTokenSource());
                            await _groupMessagingService.RefreshUnreadMessagesFromGroupingChatsAsync(new CancellationTokenSource());
                            break;
                        case MessagingCompanionType.Group:
                            await _groupMessagingService.MarkGroupMessageAsReadedAsync(MessagesHistory.Last<MessageItem>().Data.Id, TargetCompanion.Companion.Id, new CancellationTokenSource());
                            await _groupMessagingService.RefreshUnreadMessagesFromGroupingChatsAsync(new CancellationTokenSource());
                            break;
                        default:
                            Debugger.Break();
                            throw new InvalidOperationException("ConversationTabViewModel.TabSelected() Unrezolved companion type");
                    }
                }
            }
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            _messagesService.FriendMessageReceived += OnMessagesServiceFriendMessageReceived;
            _groupMessagingService.GroupMessageReceived += OnGroupMessagingServiceGroupMessageReceived;
            _groupMessagingService.TeamMessageReceived += OnGroupMessagingServiceTeamMessageReceived;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            _messagesService.FriendMessageReceived -= OnMessagesServiceFriendMessageReceived;
            _groupMessagingService.GroupMessageReceived -= OnGroupMessagingServiceGroupMessageReceived;
            _groupMessagingService.TeamMessageReceived -= OnGroupMessagingServiceTeamMessageReceived;
        }

        protected Task GetConversationHistoryAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getConversationHistoryCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getConversationHistoryCancellationTokenSource;

                try {
                    List<MessageDTO> foundConversationHistory = null;

                    switch (TargetCompanion.MessagingCompanionType) {
                        case MessagingCompanionType.Family:
                            foundConversationHistory = await _chatService.GetFamilyChatAsync(TargetCompanion.Companion.Id, cancellationTokenSource.Token);
                            break;
                        case MessagingCompanionType.Friend:
                            foundConversationHistory = await _chatService.GetFriendChatAsync(TargetCompanion.Companion.Id, cancellationTokenSource.Token);
                            break;
                        case MessagingCompanionType.Team:
                            foundConversationHistory = await _chatService.GetTeamChatAsync(TargetCompanion.Companion.Id, cancellationTokenSource.Token);
                            break;
                        case MessagingCompanionType.Group:
                            foundConversationHistory = await _chatService.GetGroupChatAsync(TargetCompanion.Companion.Id, cancellationTokenSource.Token);
                            break;
                        default:
                            throw new InvalidOperationException(_UNRESOLVED_COMPANION_TYPE_ERROR);
                    }

                    Device.BeginInvokeOnMainThread(() => {
                        MessagesHistory = (foundConversationHistory != null)
                            ? new ObservableCollection<MessageItem>(_messageItemFactory.CreateMessageItems(foundConversationHistory, TargetCompanion))
                            : new ObservableCollection<MessageItem>();

                        MarkAsReaded();
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        protected bool IsMessageFromCurrentConversation(long authorId, long companionId) {
            bool result = false;

            try {
                if (authorId == GlobalSettings.Instance.UserProfile.Id) {
                    result = companionId == TargetCompanion.Companion.Id;
                }
                else {
                    switch (TargetCompanion.MessagingCompanionType) {
                        case MessagingCompanionType.Family:
                            result = companionId == TargetCompanion.Companion.Id;
                            break;
                        case MessagingCompanionType.Friend:
                            result = authorId == TargetCompanion.Companion.Id;
                            break;
                        case MessagingCompanionType.Team:
                            result = companionId == TargetCompanion.Companion.Id;
                            break;
                        case MessagingCompanionType.Group:
                            result = companionId == TargetCompanion.Companion.Id;
                            break;
                        default:
                            Debugger.Break();
                            break;
                    }
                }
            }
            catch (Exception exc) {
                throw new InvalidOperationException(_RESOLVE_RELATION_BETWEEN_MESSAGE_AND_CONVERSATION_ERROR, exc);
            }

            return result;
        }

        private async void OnMessagesServiceFriendMessageReceived(object sender, ReceivedMessageSignalArgs e) {
            try {
                if (TargetCompanion.MessagingCompanionType == MessagingCompanionType.Friend) {
                    long newMessageId = long.Parse(e.Id);

                    if (IsMessageFromCurrentConversation(long.Parse(e.FromId), long.Parse(e.ToId)) && !MessagesHistory.Any(messageItem => messageItem.Data.Id == newMessageId)) {
                        Device.BeginInvokeOnMainThread(() => {
                            MessagesHistory?.ForEach(message => message.UpdateDate());
                            MessagesHistory.Add(_messageItemFactory.BuildSingleMessageItem(e, TargetCompanion));
                            MarkAsReaded();
                        });
                    }
                }
            }
            catch (Exception exc) {
                await DialogService.ToastAsync(string.Format("{0}. {1}", _FRIEND_MESSAGE_HANDLING_ERROR, exc.Message));
            }
        }

        private async void OnGroupMessagingServiceGroupMessageReceived(object sender, ReceivedMessageSignalArgs e) {
            try {
                if (TargetCompanion.MessagingCompanionType == MessagingCompanionType.Group) {
                    long newMessageId = long.Parse(e.Id);

                    if (IsMessageFromCurrentConversation(long.Parse(e.FromId), long.Parse(e.ToId)) && !MessagesHistory.Any(messageItem => messageItem.Data.Id == newMessageId)) {
                        Device.BeginInvokeOnMainThread(() => {
                            MessagesHistory?.ForEach(message => message.UpdateDate());
                            MessagesHistory.Add(_messageItemFactory.BuildSingleMessageItem(e, TargetCompanion));
                            MarkAsReaded();
                        });
                    }
                }
            }
            catch (Exception exc) {
                await DialogService.ToastAsync(string.Format("{0}. {1}", _GROUP_MESSAGE_HANDLING_ERROR, exc.Message));
            }
        }

        private async void OnGroupMessagingServiceTeamMessageReceived(object sender, ReceivedMessageSignalArgs e) {
            try {
                if (TargetCompanion.MessagingCompanionType == MessagingCompanionType.Team) {
                    long newMessageId = long.Parse(e.Id);

                    if (IsMessageFromCurrentConversation(long.Parse(e.FromId), long.Parse(e.ToId)) && !MessagesHistory.Any(messageItem => messageItem.Data.Id == newMessageId)) {
                        Device.BeginInvokeOnMainThread(() => {
                            MessagesHistory?.ForEach(message => message.UpdateDate());
                            MessagesHistory.Add(_messageItemFactory.BuildSingleMessageItem(e, TargetCompanion));
                            MarkAsReaded();
                        });
                    }
                }
            }
            catch (Exception exc) {
                await DialogService.ToastAsync(string.Format("{0}. {1}", _GROUP_MESSAGE_HANDLING_ERROR, exc.Message));
            }
        }
    }
}
