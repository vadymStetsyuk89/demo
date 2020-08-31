using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Sockets.GroupMessaging;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Chat;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public class TeamsMessengerTabViewModel : MessagingClusterTabBaseViewModel {

        private readonly ITeamMemberService _teamMemberService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getTeamMembersCancellationTokenSource = new CancellationTokenSource();

        public TeamsMessengerTabViewModel(
            ITeamMemberService teamMemberService,
            IPossibleConversationItemsFactory possibleConversationItemsFactory,
            IStateService stateService,
            IChatService chatService,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService)
            : base(possibleConversationItemsFactory,
                  chatService,
                  messagesService,
                  groupMessagingService) {

            _teamMemberService = teamMemberService;
            _stateService = stateService;
        }

        public override Task AskToRefreshAsync() => ResolvePossibleConversationsAsync();

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
        }

        public override Task ResolvePossibleConversationsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getTeamMembersCancellationTokenSource;

                try {
                    List<TeamMember> foundTeamMembers = await _teamMemberService.GetTeamMembersAsync(cancellationTokenSource.Token);

                    ChargePossibleConverstionItems(foundTeamMembers?.Select<TeamMember, TeamDTO>(tMDTO => tMDTO.Team));
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

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
        }

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
            MessagingCenter.Instance.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, args) => LostRealtionWithTeam(args));
            _stateService.ChangedTeams += OnStateServiceChangedTeams;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            /// If it will be neccessary remove this subscriptions to the `long terme app events subscription`
            /// 
            MessagingCenter.Instance.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);
            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
        }

        private void LostRealtionWithTeam(long targetTeamId) {
            PossibleConverstionItem itemToRemove = PossibleConversations?.FirstOrDefault<PossibleConverstionItem>(pCI => (pCI.Companion is TeamDTO && pCI.Companion.Id == targetTeamId));
            PossibleConversations?.Remove(itemToRemove);
        }

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) => ExecuteActionWithBusy(ResolvePossibleConversationsAsync);

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => ExecuteActionWithBusy(ResolvePossibleConversationsAsync);

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => ExecuteActionWithBusy(ResolvePossibleConversationsAsync);

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => LostRealtionWithTeam(e.Id);

        //private void OnGroupMessagingServiceResolvedUnreadGroupMessages(object sender, ResolvedUnreadGroupMessagesSignalArgs e) {
        //    try {
        //        if (e.TeamChatCounters != null && e.TeamChatCounters.Any()) {
        //            PossibleConversations.ForEach<PossibleConverstionItem>(possibleConversation => possibleConversation.UnreadMessagesCount = 0);
        //            UnreadMessages = 0;

        //            e.TeamChatCounters.ForEach(teamChat => {
        //                PossibleConverstionItem possibleConverstionItem = PossibleConversations.FirstOrDefault(possibleConversation => teamChat.Id == possibleConversation.Companion.Id);
        //                if (possibleConverstionItem != null) {
        //                    possibleConverstionItem.UnreadMessagesCount = (int)teamChat.Count;
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
                if (e.TeamChatCounters != null && e.TeamChatCounters.Any()) {
                    PossibleConversations.ForEach(possibleConversation => {
                        TeamUnreadMessages unreadMessages = e.TeamChatCounters.FirstOrDefault(teamUnreadMessages => teamUnreadMessages.Id == possibleConversation.Companion.Id);

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
