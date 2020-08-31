using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers.EqualityComparers;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Invites;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.ViewModels.MainContent.Teams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class PlayerProfileContentViewModel : UserTypeDependentProfileContentBaseViewModel, IProfileInfoDependent {

        private readonly IInviteService _inviteService;
        private readonly ITeamMemberFactory _teamMemberFactory;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IStateService _stateService;

        private readonly GenericEqualityComparer<TeamMember> _teamMemberIdEqualityComparer = new GenericEqualityComparer<TeamMember>((teamMember) => teamMember.Id);

        private CancellationTokenSource _getTeamMembersCancellationTokenSource = new CancellationTokenSource();

        public PlayerProfileContentViewModel(ITeamMemberService teamMemberService,
                                             ITeamMemberFactory teamMemberFactory,
                                             IInviteService inviteService,
                                             IStateService stateService) {
            _teamMemberService = teamMemberService;
            _teamMemberFactory = teamMemberFactory;
            _inviteService = inviteService;
            _stateService = stateService;
        }

        string _displayName;
        public string DisplayName {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        TeamInviteItemViewModel _selectedItem;
        public TeamInviteItemViewModel SelectedItem {
            get { return _selectedItem; }
            set {
                if (SetProperty(ref _selectedItem, value)) {
                    if (value != null) {
                        WorkWithItem(value);
                    }
                    SelectedItem = null;
                }
            }
        }

        ObservableCollection<TeamMemberViewModel> _pickedTeams;
        public ObservableCollection<TeamMemberViewModel> PickedTeams {
            get => _pickedTeams;
            set {
                _pickedTeams?.ForEach(tMVM => tMVM.Dispose());
                SetProperty(ref _pickedTeams, value);
            }
        }

        public void ResolveProfileInfo() {
            DisplayName = GlobalSettings.Instance.UserProfile.DisplayName;
        }

        public override Task AskToRefreshAsync() =>
            Task.Run(async () => {
                Device.BeginInvokeOnMainThread(() => ResolveProfileInfo());
                await GetTeamMembersAsync();
            });

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);

            PickedTeams?.ForEach(tMVM => tMVM.Dispose());
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetTeamMembersAsync);
            ResolveProfileInfo();
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Instance.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted, (sender, args) => ExecuteActionWithBusy(GetTeamMembersAsync));
            MessagingCenter.Instance.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, args) => ExecuteActionWithBusy(GetTeamMembersAsync));
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;
            _stateService.ChangedTeams += OnStateServiceChangedTeams;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Instance.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted);
            MessagingCenter.Instance.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated; ;
            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        private async void WorkWithItem(TeamInviteItemViewModel value) {
            await NavigationService.NavigateToAsync<TeamsInfoViewModel>(new TeamMember {
                Team = new TeamDTO { Id = value.TeamId, Name = value.TeamName }
            });
        }

        private Task GetTeamMembersAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getTeamMembersCancellationTokenSource;

                try {
                    List<TeamMember> items = await _teamMemberService.GetTeamMembersAsync(cancellationTokenSource.Token);
                    ObservableCollection<TeamMemberViewModel> preparedTeamMembers = (items != null && items.Any()) ? (_teamMemberFactory.CreateItems(items)).ToObservableCollection() : new ObservableCollection<TeamMemberViewModel>();

                    Device.BeginInvokeOnMainThread(() => PickedTeams = preparedTeamMembers);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnProfileSettingsEventsProfileUpdated(object sender, ProfileUpdatedArgs e) {
            ResolveProfileInfo();
            ExecuteActionWithBusy(GetTeamMembersAsync);
        }
    }
}
