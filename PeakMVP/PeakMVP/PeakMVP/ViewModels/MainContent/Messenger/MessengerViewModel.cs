using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Helpers.EqualityComparers;
using PeakMVP.Models.DataItems.MainContent.Messenger;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.DataItems.MainContent;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Messenger.Arguments;
using PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs;
using PeakMVP.Views.CompoundedViews.MainContent.Messenger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Messenger {
    public class MessengerViewModel : TabbedViewModel {

        private readonly IMessengerDataItems _messengerDataItems;
        private readonly IStateService _stateService;
        private readonly IPossibleConversationItemsFactory _possibleConversationItemsFactory;

        private readonly GenericEqualityComparer<ProfileDTO> _profileIdEqualityComparer = new GenericEqualityComparer<ProfileDTO>((profile) => profile.Id);
        private readonly GenericEqualityComparer<GroupDTO> _groupIdEqualityComparer = new GenericEqualityComparer<GroupDTO>((group) => group.Id);
        private readonly GenericEqualityComparer<TeamDTO> _teamIdEqualityComparer = new GenericEqualityComparer<TeamDTO>((team) => team.Id);

        public MessengerViewModel(
            IMessengerDataItems messengerDataItems,
            IStateService stateService,
            IPossibleConversationItemsFactory possibleConversationItemsFactory) {

            _messengerDataItems = messengerDataItems;
            _stateService = stateService;
            _possibleConversationItemsFactory = possibleConversationItemsFactory;

            MessengerTabs = new ObservableCollection<IMessengerDataItem>(_messengerDataItems.ResolveDefaultMessengerTabItems(GlobalSettings.Instance.UserProfile));
            SelectedMessengerTab = MessengerTabs.FirstOrDefault();

            IsNestedPullToRefreshEnabled = true;
        }

        private ObservableCollection<IMessengerDataItem> _messengerTabs = new ObservableCollection<IMessengerDataItem>();
        public ObservableCollection<IMessengerDataItem> MessengerTabs {
            get => _messengerTabs;
            private set {
                _messengerTabs?.ForEach(mDI => mDI.Dispose());

                SetProperty<ObservableCollection<IMessengerDataItem>>(ref _messengerTabs, value);
            }
        }

        private IMessengerDataItem _selectedMessengerTab;
        public IMessengerDataItem SelectedMessengerTab {
            get => _selectedMessengerTab;
            set {
                MessengerTabs?.ForEach(messengerTab => messengerTab.IsSelected = false);

                SetProperty<IMessengerDataItem>(ref _selectedMessengerTab, value);

                if (_selectedMessengerTab != null) {
                    _selectedMessengerTab.IsSelected = true;
                }
            }
        }

        protected override Task NestedRefreshAction() =>
            Task.Run(async () => {
                await MessengerTabs.ForEachAsync(messengerTabs => messengerTabs.AskToRefreshAsync());
            });

        public override void Dispose() {
            base.Dispose();

            MessengerTabs?.ForEach(mT => {
                mT.Dispose();
                mT.IsSelected = false;
            });
            SelectedMessengerTab?.Dispose();
            SelectedMessengerTab = null;
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ContentPageBaseViewModel) {
                MessengerTabs.OfType<IResolvePossibleConversations>().ForEach(mCT => mCT.ResolvePossibleConversationsAsync());
            }
            else if (navigationData is StartConversationArgs startConversationArgs) {
                OnStartConversation(startConversationArgs);
            }
            else if (navigationData is StartOuterConversationArgs startOuterConversationArgs) {
                try {
                    PossibleConverstionItem possibleConverstionItem = _possibleConversationItemsFactory.BuildConversationItem(startOuterConversationArgs.TargetCompanion);
                    OnStartConversation(new StartConversationArgs() { TargetCompanion = possibleConverstionItem });
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    Debugger.Break();
                }
            }
            else if (navigationData is CloseConversationTabArgs closeConversationTabArgs) {
                if (MessengerTabs.Remove(closeConversationTabArgs.TabToClose)) {
                    closeConversationTabArgs.TabToClose.Dispose();
                    SelectedMessengerTab = MessengerTabs.Last();
                }
            }

            MessengerTabs.ForEach(mDI => mDI.InitializeAsync(navigationData));

            return base.InitializeAsync(navigationData);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            try {
                SelectedMessengerTab = MessengerTabs?.FirstOrDefault();
                MessengerTabs?.OfType<ConversationTabViewModel>().ToArray<ConversationTabViewModel>().ForEach<ConversationTabViewModel>(tabToClose => tabToClose.CloseTabCommand.Execute(null));
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();
            }
        }

        protected override void TakeIntent() {
            base.TakeIntent();


            try {
                MessengerTabs.OfType<IResolvePossibleConversations>().ForEach(mCT => mCT.ResolvePossibleConversationsAsync());
                //ResolveFriendsTabsToClose(((FriendsMessengerTabViewModel)MessengerTabs.First(tab=>tab is FriendsMessengerTabViewModel)).PossibleConversations)
            }
            catch (Exception exc) {
                Debugger.Break();
                Crashes.TrackError(exc);
            }
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted, (sender, friendId) => OnCompanionWasDeleted<ProfileDTO>(friendId));
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, args) => OnCompanionWasDeleted<TeamDTO>(args));
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.ChangedGroups += OnStateServiceChangedGroups;
            _stateService.ChangedTeams += OnStateServiceChangedTeams;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.MessagingEvents.RefreshedUnreadMessagesForConverseClusterTab += OnMessagingEventsRefreshedUnreadMessagesForConverseClusterTab;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.MessagingEvents.RefreshedUnreadMessagesForConverseClusterTab -= OnMessagingEventsRefreshedUnreadMessagesForConverseClusterTab;
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.MESSAGES_TITLE;
            TabIcon = NavigationContext.MESSAGES_IMAGE_PATH;
            RelativeViewType = typeof(MessengerView);
        }

        private void OnStartConversation(StartConversationArgs conversationArgs) {
            ConversationTabViewModel possibleConversationTab =
                MessengerTabs.OfType<ConversationTabViewModel>().FirstOrDefault<ConversationTabViewModel>((cTVM) => (cTVM.TargetCompanion?.Companion.Id == conversationArgs.TargetCompanion.Companion.Id
                    && cTVM.TargetCompanion?.MessagingCompanionType == conversationArgs.TargetCompanion.MessagingCompanionType));

            if (possibleConversationTab != null) {
                SelectedMessengerTab = possibleConversationTab;
            }
            else {
                ConversationTabViewModel conversationTabViewModel = _messengerDataItems.BuildConversationTab(conversationArgs.TargetCompanion);
                MessengerTabs.Add(conversationTabViewModel);
                SelectedMessengerTab = conversationTabViewModel;
            }
        }

        //private void TryStartConversation

        private void OnCompanionWasDeleted<TCompanionType>(long companionId) where TCompanionType : IPossibleMessaging {
            ConversationTabViewModel conversationToClose = MessengerTabs?
                .OfType<ConversationTabViewModel>()
                .FirstOrDefault<ConversationTabViewModel>(cVM => (cVM.TargetCompanion.Companion is TCompanionType && cVM.TargetCompanion.Companion.Id == companionId));

            if (conversationToClose != null) {
                MessengerTabs?.Remove(conversationToClose);

                if (SelectedMessengerTab == conversationToClose) {
                    SelectedMessengerTab = MessengerTabs?.LastOrDefault();
                }

                conversationToClose.Dispose();
            }
        }

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs e) => ResolveFriendsTabsToClose(e.Data.Select<FriendshipDTO, ProfileDTO>(friendship => friendship.Profile));

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) {
            IEnumerable<GroupDTO> companionsTabsToClose = MessengerTabs?
                .OfType<ConversationTabViewModel>()
                .Where(cT => cT.TargetCompanion.Companion is GroupDTO)
                .Select<ConversationTabViewModel, GroupDTO>(cT => (GroupDTO)cT.TargetCompanion.Companion)
                .Except<GroupDTO>(e.Data, _groupIdEqualityComparer)
                .ToArray<GroupDTO>();

            companionsTabsToClose.ForEach<GroupDTO>(gDTO => OnCompanionWasDeleted<GroupDTO>(gDTO.Id));
        }

        private async void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) {
            try {
                ResolveTeamsTabsToClose(e.TeamInvites);
                ResolveFriendsTabsToClose(e.FriendshipInvites.Select<FriendshipDTO, ProfileDTO>(friendship => friendship.Profile));
            }
            catch (Exception exc) {
                await DialogService.ToastAsync(string.Format("{0} .{1}", StateService.CHANGED_INVITES_HANDLING_ERROR, exc.Message));
            }
        }

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => ResolveTeamsTabsToClose(e.Data.Select<TeamMember, TeamDTO>(tMDTO => tMDTO.Team));

        private void ResolveTeamsTabsToClose(IEnumerable<TeamDTO> eventTeams) {
            IEnumerable<TeamDTO> teamsTabsToClose = MessengerTabs?
                .OfType<ConversationTabViewModel>()
                .Where(cT => cT.TargetCompanion.Companion is TeamDTO)
                .Select<ConversationTabViewModel, TeamDTO>(cT => (TeamDTO)cT.TargetCompanion.Companion)
                .Except<TeamDTO>(eventTeams, _teamIdEqualityComparer)
                .ToArray<TeamDTO>();

            teamsTabsToClose.ForEach<TeamDTO>(tDTO => OnCompanionWasDeleted<TeamDTO>(tDTO.Id));
        }

        private void ResolveFriendsTabsToClose(IEnumerable<ProfileDTO> friends) {
            IEnumerable<ProfileDTO> companionsTabsToClose = MessengerTabs?
                .OfType<ConversationTabViewModel>()
                .Where(cT => cT.TargetCompanion.Companion is ProfileDTO)
                .Select<ConversationTabViewModel, ProfileDTO>(cT => (ProfileDTO)cT.TargetCompanion.Companion)
                .Except<ProfileDTO>(friends, _profileIdEqualityComparer)
                .ToArray<ProfileDTO>();

            companionsTabsToClose.ForEach<ProfileDTO>(pDTO => OnCompanionWasDeleted<ProfileDTO>(pDTO.Id));
        }

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => OnCompanionWasDeleted<TeamDTO>(e.Id);

        private void OnMessagingEventsRefreshedUnreadMessagesForConverseClusterTab(object sender, int e) {
            try {
                int totalUnreadMessages = MessengerTabs.OfType<IMessagesCounterTab>().Sum<IMessagesCounterTab>(messengerTab => messengerTab.UnreadMessages);

                if (BudgeCount != totalUnreadMessages) {
                    BudgeCount = totalUnreadMessages;
                }

                IsBudgeVisible = BudgeCount > 0;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();

                BudgeCount = 0;
                IsBudgeVisible = false;
            }
        }
    }
}
