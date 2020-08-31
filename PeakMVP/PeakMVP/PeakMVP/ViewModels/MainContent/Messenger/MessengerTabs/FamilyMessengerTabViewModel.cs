using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.GroupMessaging;
using PeakMVP.Models.Sockets.Messages;
using PeakMVP.Services.Chat;
using PeakMVP.Services.Family;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public class FamilyMessengerTabViewModel : ConversationTabViewModel, IResolvePossibleConversations, IMessagesCounterTab {

        private static readonly string _GET_FAMILY_ERROR = "Can't resolve own family info";

        private readonly IFamilyService _familyService;
        private readonly IPossibleConversationItemsFactory _possibleConversationItemsFactory;

        private CancellationTokenSource _getFamilyCancellationTokenSource = new CancellationTokenSource();

        public FamilyMessengerTabViewModel(
            IChatService chatService,
            IMessageItemFactory messageItemFactory,
            IFamilyService familyService,
            IPossibleConversationItemsFactory possibleConversationItemsFactory,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService)
            : base(chatService,
                  messageItemFactory,
                  messagesService,
                  groupMessagingService) {

            _familyService = familyService;
            _possibleConversationItemsFactory = possibleConversationItemsFactory;

            CanBeClosed = false;
        }

        private int _unreadMessages;
        public int UnreadMessages {
            get => _unreadMessages;
            private set => SetProperty<int>(ref _unreadMessages, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getFamilyCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getFamilyCancellationTokenSource);
        }

        public Task ResolvePossibleConversationsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getFamilyCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getFamilyCancellationTokenSource;

                try {
                    FamilyDTO familyDTO = await _familyService.GetFamilyAsync(cancellationTokenSource);

                    if (familyDTO != null) {
                        PossibleConverstionItem familyConversationItem = _possibleConversationItemsFactory.BuildConversationItem(familyDTO);

                        await InitializeAsync(familyConversationItem);
                    }
                    else {
                        throw new InvalidOperationException(_GET_FAMILY_ERROR);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            _groupMessagingService.FamilyMessageReceived += OnGroupMessagingServiceFamilyMessageReceived;
            _groupMessagingService.RefreshedUnreadGroupingMessages += OnGroupMessagingServiceResolvedUnreadGroupMessages;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            _groupMessagingService.FamilyMessageReceived -= OnGroupMessagingServiceFamilyMessageReceived;
            _groupMessagingService.RefreshedUnreadGroupingMessages -= OnGroupMessagingServiceResolvedUnreadGroupMessages;
        }

        private async void OnGroupMessagingServiceFamilyMessageReceived(object sender, ReceivedMessageSignalArgs e) {
            try {
                if (TargetCompanion.MessagingCompanionType == MessagingCompanionType.Family) {
                    long newMessageId = long.Parse(e.Id);

                    if (IsMessageFromCurrentConversation(long.Parse(e.FromId), long.Parse(e.ToId)) && !MessagesHistory.Any(messageItem => messageItem.Data.Id == newMessageId)) {
                        Device.BeginInvokeOnMainThread(() => {
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

        private void OnGroupMessagingServiceResolvedUnreadGroupMessages(object sender, ResolvedUnreadGroupMessagesSignalArgs e) {
            try {
                UnreadMessages = e.FamilyChatCounter != null ? (int)e.FamilyChatCounter.Count : 0;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();

                UnreadMessages = 0;
            }

            GlobalSettings.Instance.AppMessagingEvents.MessagingEvents.RefreshedUnreadMessagesForConverseClusterTabInvoke(this, UnreadMessages);
        }
    }
}
