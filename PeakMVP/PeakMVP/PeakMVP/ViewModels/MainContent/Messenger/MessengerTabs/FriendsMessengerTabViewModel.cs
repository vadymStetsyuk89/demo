using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.Messages;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Chat;
using PeakMVP.Services.Friends;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.Services.SignalR.StateNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public class FriendsMessengerTabViewModel : MessagingClusterTabBaseViewModel {

        private readonly IFriendService _friendService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getFriendsCancellationTokenSource = new CancellationTokenSource();

        public FriendsMessengerTabViewModel(
            IFriendService friendService,
            IPossibleConversationItemsFactory possibleConversationItemsFactory,
            IStateService stateService,
            IChatService chatService,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService)
            : base(possibleConversationItemsFactory,
                  chatService,
                  messagesService,
                  groupMessagingService) {

            _friendService = friendService;
            _stateService = stateService;
        }

        public override Task AskToRefreshAsync() => ResolvePossibleConversationsAsync();

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getFriendsCancellationTokenSource);
        }

        public override Task ResolvePossibleConversationsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getFriendsCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getFriendsCancellationTokenSource;

                try {
                    List<FriendshipDTO> foundFriends = await _friendService.GetAllFriendsAsync(cancellationTokenSource.Token);

                    ChargePossibleConverstionItems(foundFriends.Select<FriendshipDTO, ProfileDTO>(fDTO => fDTO.Profile));
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getFriendsCancellationTokenSource);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            _messagesService.RefreshedUnreadFriendMessages += OnMessagesServiceResolvedUnreadFriendMessages;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            _messagesService.RefreshedUnreadFriendMessages -= OnMessagesServiceResolvedUnreadFriendMessages;
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            /// If it will be neccessary remove this subscriptions to the `long terme app events subscription`
            /// 
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted, (sender, args) => {
                PossibleConverstionItem itemToRemove = PossibleConversations?.FirstOrDefault<PossibleConverstionItem>(pCI => (pCI.Companion is ProfileDTO && pCI.Companion.Id == args));
                PossibleConversations?.Remove(itemToRemove);
            });
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteAccepted, (sender, args) => { });
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            /// If it will be neccessary remove this subscriptions to the `long terme app events subscription`
            /// 
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteAccepted);
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
        }

        private void OnMessagesServiceResolvedUnreadFriendMessages(object sender, ResolvedUnreadFriendMessagesSignalArgs e) {
            try {
                if (e.Counts != null && e.Counts.Any()) {
                    PossibleConversations.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
                    UnreadMessages = 0;

                    e.Counts.ForEach(friendUnreadingMessages => {
                        PossibleConverstionItem possibleConverstionItem = PossibleConversations.FirstOrDefault(possibleConversation => friendUnreadingMessages.Id == possibleConversation.Companion.Id);
                        if (possibleConverstionItem != null) {
                            possibleConverstionItem.UnreadMessagesCount = (int)friendUnreadingMessages.Count;
                        }
                    });

                    UnreadMessages = PossibleConversations.Sum<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount);
                }
                else {
                    PossibleConversations.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
                    UnreadMessages = 0;
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();

                PossibleConversations?.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
                UnreadMessages = 0;
            }

            GlobalSettings.Instance.AppMessagingEvents.MessagingEvents.RefreshedUnreadMessagesForConverseClusterTabInvoke(this, UnreadMessages);
        }

        protected override void TODO() => _messagesService.RefreshUnreadFriendMessagesAsync(GlobalSettings.Instance.UserProfile.Id, new CancellationTokenSource());

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs args) => ExecuteActionWithBusy(ResolvePossibleConversationsAsync);

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) => ExecuteActionWithBusy(ResolvePossibleConversationsAsync);
    }
}
