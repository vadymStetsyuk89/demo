using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.GroupMessaging;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Chat;
using PeakMVP.Services.Groups;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.Services.SignalR.StateNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public class GroupsMessengerTabViewModel : MessagingClusterTabBaseViewModel {

        private readonly IGroupsService _groupsService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getGroupsCancellationTokenSource = new CancellationTokenSource();

        public GroupsMessengerTabViewModel(
            IGroupsService groupsService,
            IPossibleConversationItemsFactory possibleConversationItemsFactory,
            IStateService stateService,
            IChatService chatService,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService)
            : base(possibleConversationItemsFactory,
                  chatService,
                  messagesService,
                  groupMessagingService) {

            _groupsService = groupsService;
            _stateService = stateService;
        }

        public override Task AskToRefreshAsync() => ResolvePossibleConversationsAsync();

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getGroupsCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getGroupsCancellationTokenSource);
        }

        public override Task ResolvePossibleConversationsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getGroupsCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getGroupsCancellationTokenSource;

                try {
                    List<GroupDTO> foundgroupDTOs = await _groupsService.GetGroupsAsync(cancellationTokenSource);

                    ChargePossibleConverstionItems(foundgroupDTOs);
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

            _groupMessagingService.RefreshedUnreadGroupingMessages += OnGroupMessagingServiceResolvedUnreadGroupMessages;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            _groupMessagingService.RefreshedUnreadGroupingMessages -= OnGroupMessagingServiceResolvedUnreadGroupMessages;
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            /// If it will be neccessary remove this subscriptions to the `long terme app events subscription`
            /// 
            _stateService.ChangedGroups += OnStateServiceChangedGroups;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            /// If it will be neccessary remove this subscriptions to the `long terme app events subscription`
            /// 
            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
        }

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) => ChargePossibleConverstionItems(e.Data);

        //private void OnGroupMessagingServiceResolvedUnreadGroupMessages(object sender, ResolvedUnreadGroupMessagesSignalArgs e) {
        //    try {
        //        if (e.GroupChatCounters != null && e.GroupChatCounters.Any()) {
        //            PossibleConversations.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
        //            UnreadMessages = 0;

        //            e.GroupChatCounters.ForEach(groupChat => {
        //                PossibleConverstionItem possibleConverstionItem = PossibleConversations.FirstOrDefault(possibleConversation => groupChat.Id == possibleConversation.Companion.Id);
        //                if (possibleConverstionItem != null) {
        //                    possibleConverstionItem.UnreadMessagesCount = (int)groupChat.Count;
        //                }
        //            });

        //            UnreadMessages = PossibleConversations.Sum<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount);
        //        }
        //        else {
        //            PossibleConversations.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
        //            UnreadMessages = 0;
        //        }
        //    }
        //    catch (Exception exc) {
        //        Crashes.TrackError(exc);
        //        Debugger.Break();

        //        PossibleConversations?.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
        //        UnreadMessages = 0;
        //    }

        //    GlobalSettings.Instance.AppMessagingEvents.MessagingEvents.RefreshedUnreadMessagesForConverseClusterTabInvoke(this, UnreadMessages);
        //}

        private void OnGroupMessagingServiceResolvedUnreadGroupMessages(object sender, ResolvedUnreadGroupMessagesSignalArgs e) {
            try {
                if (e.GroupChatCounters != null && e.GroupChatCounters.Any()) {
                    //PossibleConversations.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
                    //UnreadMessages = 0;

                    //e.GroupChatCounters.ForEach(groupChat => {
                    //    PossibleConverstionItem possibleConverstionItem = PossibleConversations.FirstOrDefault(possibleConversation => groupChat.Id == possibleConversation.Companion.Id);
                    //    if (possibleConverstionItem != null) {
                    //        possibleConverstionItem.UnreadMessagesCount = (int)groupChat.Count;
                    //    }
                    //});

                    //UnreadMessages = PossibleConversations.Sum<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount);

                    PossibleConversations.ForEach(possibleConversation => {
                        GroupUnreadMessages unreadMessages = e.GroupChatCounters.FirstOrDefault(groupUnreadMessages => groupUnreadMessages.Id == possibleConversation.Companion.Id);

                        possibleConversation.UnreadMessagesCount = unreadMessages != null ? (int)unreadMessages.Count : 0;
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

        protected override void TODO() => _groupMessagingService.RefreshUnreadMessagesFromGroupingChatsAsync(new CancellationTokenSource());
    }
}
