using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Teams {
    public class TeamMemberProviderViewModel : NestedViewModel, IAskToRefresh {

        private readonly ITeamMemberFactory _teamMemberFactory;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getTeamMembersCancellationTokenSource = new CancellationTokenSource();

        public event EventHandler TeamMembersUpdated = delegate { };

        public TeamMemberProviderViewModel(
            ITeamMemberService teamMemberService,
            ITeamMemberFactory teamMemberFactory,
            IStateService stateService) {

            _teamMemberFactory = teamMemberFactory;
            _teamMemberService = teamMemberService;
            _stateService = stateService;
        }

        private ObservableCollection<TeamMemberViewModel> _teamMembers = new ObservableCollection<TeamMemberViewModel>();
        public ObservableCollection<TeamMemberViewModel> TeamMembers {
            get => _teamMembers;
            private set {
                _teamMembers?.ForEach(tMVM => tMVM.Dispose());
                SetProperty(ref _teamMembers, value);

                TeamMembersUpdated(this, new EventArgs());
            }
        }

        public Task AskToRefreshAsync() => GetTeamMembersAsync();

        public List<TeamMemberViewModel> BuildTeamMemberViewModels(IEnumerable<TeamMember> teamMembers) => _teamMemberFactory.CreateItems(teamMembers);

        public override Task InitializeAsync(object navigationData) {
            TeamMembers?.ForEach<TeamMemberViewModel>(teamMember => teamMember?.InitializeAsync(navigationData));

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
            TeamMembers?.ForEach<TeamMemberViewModel>(teamMember => teamMember?.Dispose());
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetTeamMembersAsync);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted, (sender, args) => ExecuteActionWithBusy(GetTeamMembersAsync));
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, args) => ExecuteActionWithBusy(GetTeamMembersAsync));
            MessagingCenter.Subscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestAcceptedForNewTeamMember, (sender) => ExecuteActionWithBusy(GetTeamMembersAsync));
            MessagingCenter.Subscribe<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped, (sender, args) => ExecuteActionWithBusy(GetTeamMembersAsync));

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;

            _stateService.ChangedTeams += OnStateServiceChangedTeams;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);
            MessagingCenter.Unsubscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestAcceptedForNewTeamMember);
            MessagingCenter.Unsubscribe<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped);

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated;

            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        private Task GetTeamMembersAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getTeamMembersCancellationTokenSource;

                try {
                    List<TeamMember> teamMembers = await _teamMemberService.GetTeamMembersAsync(cancellationTokenSource.Token, GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent);

                    if (teamMembers != null) {
                        /// Parent `team member` response will contains duplicates
                        ObservableCollection<TeamMemberViewModel> resolvedTeamMembers = (_teamMemberFactory.CreateItems(teamMembers)).ToObservableCollection();

                        Device.BeginInvokeOnMainThread(() => TeamMembers = resolvedTeamMembers);
                    }
                    else {
                        throw new InvalidOperationException(TeamMemberService.CANT_GET_TEAM_MEMBERS_ERROR_MESSAGE);
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

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => ExecuteActionWithBusy(GetTeamMembersAsync);

        private void OnProfileSettingsEventsProfileUpdated(object sender, ProfileUpdatedArgs e) => ExecuteActionWithBusy(GetTeamMembersAsync);
    }
}
