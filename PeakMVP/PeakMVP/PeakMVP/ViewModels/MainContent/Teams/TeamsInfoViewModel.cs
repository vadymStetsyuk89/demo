using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.Groups.GroupPopups;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.ViewModels.MainContent.Teams.Arguments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Teams {
    public class TeamsInfoViewModel : LoggedContentPageViewModel {

        private readonly ITeamFactory _teamFactory;
        private readonly ITeamService _teamService;
        private readonly IStateService _stateService;

        public static readonly string EXTERNAL_INVITE_RESENDED = "Invite resended";
        private static readonly string _TEAM_PARTNERSHIP_STOPPED_SUCCESFULLY_MESSAGE = "Partnership with {0} was successfully stoped";
        private static readonly string _DELETE_TEAM_COMMON_ERROR_MESSAGE = "Can't remove {0} team";
        private static readonly string _TEAM_END_PARTNERSHIP_WITH_ORGANIZATION_COMMON_ERROR_MESSAGE = "Partnership can't be stopped now";
        private static readonly string _REQUEST_TO_JOIN_TEAM_SENT_MESSAGE = "Request sent";
        private static readonly string _REQUEST_TO_JOIN_TEAM_CANCELED_MESSAGE = "Request canceled";

        private CancellationTokenSource _getTeamMembersCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _removeTeamCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _resolveFullTeamInfoByTeamIdAsyncCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _checkAppointmentCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _sendAppointmentCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _endPartnershipWitOrganizationCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _resolveIsRequestToJoinTemWasSentCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _sendRequestToJoinTeamCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getTeamByIdCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getTeamRequestsCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getExternalInvitesByTeamIdCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _resendExternalInviteCancellationTokenSource = new CancellationTokenSource();

        public TeamsInfoViewModel(
            ITeamService teamService,
            ITeamFactory teamFactory,
            IStateService stateService) {

            _teamFactory = teamFactory;
            _teamService = teamService;
            _stateService = stateService;

            ActionBarViewModel = ViewModelLocator.Resolve<ModeActionBarViewModel>();
            ActionBarViewModel.InitializeAsync(this);
            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = false;

            AddMemberToTeamPopupViewModel = ViewModelLocator.Resolve<AddMemberToTeamPopupViewModel>();
            AddMemberToTeamPopupViewModel.InitializeAsync(this);

            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

            RefreshCommand = new Command(async () => {
                IsRefreshing = true;

                await GetTeamByIdAsync(TargetTeam.Id);
                await GetTeamRequestsAsync(TargetTeam);

                await Task.Delay(AppConsts.DELAY_STUB);

                IsRefreshing = false;
            });
        }

        public ICommand ResendExternalInviteCommand => new Command(async (object param) => {
            if (param is ExternalInvite externalInviteParam) {
                Guid busyId = Guid.NewGuid();
                SetBusy(busyId, true);

                ResetCancellationTokenSource(ref _resendExternalInviteCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _resendExternalInviteCancellationTokenSource;

                try {
                    ResendExternalMemberInviteResponse resendExternal = await _teamService.ResendExternalMemberInviteAsync(TargetTeam.Id, externalInviteParam.Email, cancellationTokenSource);

                    await DialogService.ToastAsync(resendExternal != null ? EXTERNAL_INVITE_RESENDED : TeamService.RESEND_INVITE_TO_EXTERNAL_COMMON_ERROR_MESSAGE);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }

                SetBusy(busyId, false);
            }
        });

        public ICommand SentRequestToJoinTeamCommand => new Command(async () => {
            Guid busyId = Guid.NewGuid();
            SetBusy(busyId, true);

            ResetCancellationTokenSource(ref _sendRequestToJoinTeamCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _sendRequestToJoinTeamCancellationTokenSource;

            try {
                bool completion = false;

                if (IsRequestToJoinTeamWasSent) {
                    completion = (await _teamService.NixTeamAppointmentAsync(TargetTeam.Id, cancellationTokenSource)) != null;
                }
                else {
                    completion = await _teamService.SendTeamAppointmentRequestAsync(TargetTeam.Id, cancellationTokenSource);
                }

                ExecuteActionWithBusy(ResolveIsRequestToTeamWasSentAsync);

                await DialogService.ToastAsync(completion ? _REQUEST_TO_JOIN_TEAM_SENT_MESSAGE : _REQUEST_TO_JOIN_TEAM_CANCELED_MESSAGE);
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyId, false);
        }, () => IsSentRequestToJoinTeamCommandAvailable);

        public ICommand ShowAddMemberPopupCommand => new Command(() => AddMemberToTeamPopupViewModel.IsPopupVisible = IsPopupsVisible = true);

        public ICommand RemoveTeamCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _removeTeamCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _removeTeamCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                bool deleteCompletion = await _teamService.RemoveTeamByIdAsync(TargetTeam.Id, cancellationTokenSource);

                if (!deleteCompletion) {
                    throw new InvalidOperationException(string.Format(_DELETE_TEAM_COMMON_ERROR_MESSAGE, TargetTeam.Name));
                }

                TargetTeam = null;
                GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeletedInvoke(this, TargetTeam);

                await NavigationService.GoBackAsync();
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        });

        public ICommand EndPartnershipWithOrganizationCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _endPartnershipWitOrganizationCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _endPartnershipWitOrganizationCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                TeamDTO resultTeam = await _teamService.EndPartnershipWithOrganization(TargetTeam.Id, cancellationTokenSource);

                if (resultTeam != null) {
                    await DialogService.ToastAsync(string.Format(_TEAM_PARTNERSHIP_STOPPED_SUCCESFULLY_MESSAGE, TargetTeam.Name));
                    MessagingCenter.Instance.Send<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped, resultTeam);
                }
                else {
                    throw new InvalidOperationException(_TEAM_END_PARTNERSHIP_WITH_ORGANIZATION_COMMON_ERROR_MESSAGE);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }, () => CanEndPartnershipWithOrganization);

        public ICommand OverviewTeamMemberCommand => new Command(async () => await DialogService.ToastAsync("Overview team member command in developig"));

        private bool _isTeamManagementAvailable;
        public bool IsTeamManagementAvailable {
            get => _isTeamManagementAvailable;
            private set => SetProperty<bool>(ref _isTeamManagementAvailable, value);
        }

        private bool _isTeamCanBeDeleted;
        public bool IsTeamCanBeDeleted {
            get => _isTeamCanBeDeleted;
            private set => SetProperty<bool>(ref _isTeamCanBeDeleted, value);
        }

        bool _isSentRequestToJoinTeamCommandAvailable;
        public bool IsSentRequestToJoinTeamCommandAvailable {
            get { return _isSentRequestToJoinTeamCommandAvailable; }
            set { SetProperty(ref _isSentRequestToJoinTeamCommandAvailable, value); }
        }

        bool _isSentRequestToJoinTeamAvailable;
        public bool IsSentRequestToJoinTeamAvailable {
            get { return _isSentRequestToJoinTeamAvailable; }
            set { SetProperty(ref _isSentRequestToJoinTeamAvailable, value); }
        }

        bool _isRequestToJoinTeamWasSent;
        public bool IsRequestToJoinTeamWasSent {
            get { return _isRequestToJoinTeamWasSent; }
            set { SetProperty(ref _isRequestToJoinTeamWasSent, value); }
        }

        ObservableCollection<TeamRequestItemViewModel> _teamRequests;
        public ObservableCollection<TeamRequestItemViewModel> TeamRequests {
            get { return _teamRequests; }
            set { SetProperty(ref _teamRequests, value); }
        }

        private AddMemberToTeamPopupViewModel _addMemberToTeamPopupViewModel;
        public AddMemberToTeamPopupViewModel AddMemberToTeamPopupViewModel {
            get => _addMemberToTeamPopupViewModel;
            private set {
                _addMemberToTeamPopupViewModel?.Dispose();

                SetProperty(ref _addMemberToTeamPopupViewModel, value);
            }
        }

        private TeamDTO _targetTeam;
        public TeamDTO TargetTeam {
            get => _targetTeam;
            private set {
                SetProperty<TeamDTO>(ref _targetTeam, value);

                ResolveTeamManagementControls(value);
            }
        }

        private ObservableCollection<TeamMember> _membersFromTeam = new ObservableCollection<TeamMember>();
        public ObservableCollection<TeamMember> MembersFromTeam {
            get => _membersFromTeam;
            private set {
                SetProperty(ref _membersFromTeam, value);

                ResolveIsSentRequestToJoinTeamButton();
            }
        }

        private List<ExternalInvite> _externalInvites = new List<ExternalInvite>();
        public List<ExternalInvite> ExternalInvites {
            get => _externalInvites;
            private set => SetProperty(ref _externalInvites, value);
        }

        private bool _canEndPartnershipWithOrganization;
        public bool CanEndPartnershipWithOrganization {
            get => _canEndPartnershipWithOrganization;
            private set => SetProperty<bool>(ref _canEndPartnershipWithOrganization, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
            ResetCancellationTokenSource(ref _removeTeamCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveFullTeamInfoByTeamIdAsyncCancellationTokenSource);
            ResetCancellationTokenSource(ref _checkAppointmentCancellationTokenSource);
            ResetCancellationTokenSource(ref _sendAppointmentCancellationTokenSource);
            ResetCancellationTokenSource(ref _endPartnershipWitOrganizationCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveIsRequestToJoinTemWasSentCancellationTokenSource);
            ResetCancellationTokenSource(ref _sendRequestToJoinTeamCancellationTokenSource);
            ResetCancellationTokenSource(ref _getTeamByIdCancellationTokenSource);
            ResetCancellationTokenSource(ref _getTeamRequestsCancellationTokenSource);
            ResetCancellationTokenSource(ref _getExternalInvitesByTeamIdCancellationTokenSource);
            ResetCancellationTokenSource(ref _resendExternalInviteCancellationTokenSource);

            TeamRequests?.ForEach(r => r.Dispose());
            TeamRequests?.Clear();

            CancellationService.Cancel();
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is TeamMember teamMemberDTO) {
                ExecuteActionWithBusy<long>(GetTeamByIdAsync, teamMemberDTO.Team.Id);
                ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, teamMemberDTO.Team);
                ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, teamMemberDTO.Team);
            }
            else if (navigationData is TeamDTO teamDTO) {
                ExecuteActionWithBusy<long>(GetTeamByIdAsync, teamDTO.Id);
                ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, teamDTO);
                ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, teamDTO);
            }
            else if (navigationData is MembersAttachedToTheTeamArgs) {
                if (TargetTeam != null) {
                    ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                }
            }

            AddMemberToTeamPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            try {
                ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, TargetTeam);
                ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, TargetTeam);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
            }
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
            ResetCancellationTokenSource(ref _removeTeamCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveFullTeamInfoByTeamIdAsyncCancellationTokenSource);
            ResetCancellationTokenSource(ref _checkAppointmentCancellationTokenSource);
            ResetCancellationTokenSource(ref _sendAppointmentCancellationTokenSource);
            ResetCancellationTokenSource(ref _endPartnershipWitOrganizationCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveIsRequestToJoinTemWasSentCancellationTokenSource);
            ResetCancellationTokenSource(ref _sendRequestToJoinTeamCancellationTokenSource);
            ResetCancellationTokenSource(ref _getTeamByIdCancellationTokenSource);
            ResetCancellationTokenSource(ref _getTeamRequestsCancellationTokenSource);
            ResetCancellationTokenSource(ref _getExternalInvitesByTeamIdCancellationTokenSource);
            ResetCancellationTokenSource(ref _resendExternalInviteCancellationTokenSource);
            CancellationService.Cancel();
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            MessagingCenter.Subscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestAcceptedForNewTeamMember, (sender) => {
                try {
                    if (TargetTeam != null) {
                        ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                        ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, TargetTeam);
                        ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, TargetTeam);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }
            });
            MessagingCenter.Subscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestDeclinedForNewTeamMember, (sender) => {
                try {
                    if (TargetTeam != null) {
                        ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                        ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, TargetTeam);
                        ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, TargetTeam);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }
            });
            MessagingCenter.Subscribe<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped, (sender, args) => {
                try {
                    if (TargetTeam != null) {
                        ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                        ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, TargetTeam);
                        ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, TargetTeam);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }
            });

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;

            _stateService.ChangedTeams += OnStateServiceChangedTeams;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            MessagingCenter.Unsubscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestAcceptedForNewTeamMember);
            MessagingCenter.Unsubscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestDeclinedForNewTeamMember);
            MessagingCenter.Unsubscribe<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped);

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated;

            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        private Task ResolveIsRequestToTeamWasSentAsync() =>
            Task.Run(async () => {
                if (TargetTeam != null) {
                    ResetCancellationTokenSource(ref _resolveIsRequestToJoinTemWasSentCancellationTokenSource);
                    CancellationTokenSource cancellationTokenSource = _resolveIsRequestToJoinTemWasSentCancellationTokenSource;

                    try {
                        bool isRequestWasSent = await _teamService.ResolveIsRequestToJoinTeamWasSentAsync(TargetTeam.Id, cancellationTokenSource);

                        Device.BeginInvokeOnMainThread(() => IsRequestToJoinTeamWasSent = isRequestWasSent);
                    }
                    catch (OperationCanceledException) { }
                    catch (ObjectDisposedException) { }
                    catch (ServiceAuthenticationException) { }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);

                        await DialogService.ToastAsync(exc.Message);
                    }
                }
            });

        private Task GetTeamRequestsAsync(TeamDTO team) =>
            Task.Run(async () => {
                if (IsTeamManager(team)) {
                    ResetCancellationTokenSource(ref _getTeamRequestsCancellationTokenSource);
                    CancellationTokenSource cancellationTokenSource = _getTeamRequestsCancellationTokenSource;

                    try {
                        ///
                        /// Try to resolve who is owner and than resolve these request
                        /// 
                        GetTeamRequestsResponse getTeamRequestsResponse = await _teamService.GetTeamRequestsAsync(team.Id, cancellationTokenSource.Token);

                        if (getTeamRequestsResponse != null) {
                            Device.BeginInvokeOnMainThread(() => {
                                TeamRequests = (_teamFactory.CreateRequestItems(getTeamRequestsResponse)).ToObservableCollection();
                                //IsVisibleRequests = TeamRequests.Any();
                            });
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (ObjectDisposedException) { }
                    catch (ServiceAuthenticationException) { }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);
                        ///
                        /// Try to resolve who is owner and than resolve these request
                        /// 
                        //await DialogService.ToastAsync(exc.Message);
                    }
                }
            });

        private Task GetExternalInvitesByTeamIdAsync(TeamDTO team) =>
            Task.Run(async () => {
                if (IsTeamManager(team)) {
                    ResetCancellationTokenSource(ref _getExternalInvitesByTeamIdCancellationTokenSource);
                    CancellationTokenSource cancellationTokenSource = _getExternalInvitesByTeamIdCancellationTokenSource;

                    try {
                        List<ExternalInvite> externalInvites = await _teamService.GetExternalIvitesByTeamIdAsync(team.Id, cancellationTokenSource);

                        if (externalInvites != null) {
                            Device.BeginInvokeOnMainThread(() => {
                                ExternalInvites = externalInvites;
                            });
                        }
                        else {
                            throw new InvalidOperationException(TeamService.GET_TEAM_EXTERNAL_INVITES_ERROR_MESSAGE);
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (ObjectDisposedException) { }
                    catch (ServiceAuthenticationException) { }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);

                        await DialogService.ToastAsync(exc.Message);
                    }
                }
            });

        private Task GetTeamByIdAsync(long teamId) =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getTeamByIdCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getTeamByIdCancellationTokenSource;

                try {
                    TeamDTO team = await _teamService.GetTeamByIdAsync(teamId, cancellationTokenSource);
                    if (team != null) {
                        team.Members = team.Members != null ? team.Members : new TeamMember[] { };
                        team.Members.ForEach<TeamMember>(teamMember => teamMember.Team = team);

                        Device.BeginInvokeOnMainThread(() => {
                            TargetTeam = team;

                            MembersFromTeam = TargetTeam.Members.ToObservableCollection();
                        });
                    }
                    else {
                        throw new InvalidOperationException(TeamService.CANT_GET_TEAM_ERROR);
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

        private void ResolveIsSentRequestToJoinTeamButton() {
            try {
                if (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Fan) {
                    IsSentRequestToJoinTeamAvailable = false;
                }
                else {
                    TeamMember ownTeamMember = MembersFromTeam.FirstOrDefault(member => member.Member.Id == GlobalSettings.Instance.UserProfile.Id);
                    IsSentRequestToJoinTeamAvailable = ownTeamMember == null;
                    IsSentRequestToJoinTeamCommandAvailable = (DateTime.Now.Year - GlobalSettings.Instance.UserProfile.DateOfBirth.Year) > UserProfile.YOUNG_PLAYERS_AGE_LIMIT;

                    if (IsSentRequestToJoinTeamAvailable) {
                        ExecuteActionWithBusy(ResolveIsRequestToTeamWasSentAsync);
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                IsSentRequestToJoinTeamAvailable = false;
            }
        }

        private void ResolveTeamManagementControls(TeamDTO team) {
            try {
                IsTeamCanBeDeleted = team.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id;
                CanEndPartnershipWithOrganization = ((team.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id) && (team.Owner?.Type == ProfileType.Organization.ToString()) && (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach));
                //IsPossibleToAddMembers = (team.Owner?.Id == GlobalSettings.Instance.UserProfile.Id) || (team.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id);
                //IsActionsAvalable = (team.Owner?.Id == GlobalSettings.Instance.UserProfile.Id) || (team.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id);
                IsTeamManagementAvailable = IsTeamManager(team);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                IsTeamCanBeDeleted = false;
                CanEndPartnershipWithOrganization = false;
                //IsPossibleToAddMembers = false;
                //IsActionsAvalable = false;
                IsTeamManagementAvailable = false;
            }
        }

        private bool IsTeamManager(TeamDTO team) {
            bool result = false;

            try {
                if (team != null) {
                    TeamMember existingCoach = team.Members.FirstOrDefault<TeamMember>(teamMember => teamMember.Member.Type == FoundUserGroupDataItemFactory.COACH_TYPE_CONSTANT_VALUE && teamMember.Member.Id == GlobalSettings.Instance.UserProfile.Id);

                    if ((GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization)
                    && (existingCoach?.Member.Id == GlobalSettings.Instance.UserProfile.Id
                        || team.CoachId == GlobalSettings.Instance.UserProfile.Id
                        || team.OrganizationId == GlobalSettings.Instance.UserProfile.Id
                        || team.CreatedBy?.Id == GlobalSettings.Instance.UserProfile.Id
                        || team.Owner?.Id == GlobalSettings.Instance.UserProfile.Id)) {
                        result = true;
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                result = false;
            }

            return result;
        }

        private async void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) {
            try {
                if (TargetTeam != null) {
                    if (e.TeamInvites != null && (!e.TeamInvites.Any() || e.TeamInvites.Any(team => team.Id == TargetTeam.Id))) {
                        ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                        ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, TargetTeam);
                        ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, TargetTeam);
                    }
                }
            }
            catch (Exception exc) {
                Debugger.Break();

                await DialogService.ToastAsync(string.Format("{0}. {1}", StateService.CHANGED_INVITES_HANDLING_ERROR, exc.Message));
            }
        }

        private async void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) {
            try {
                if (TargetTeam != null) {
                    ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                    ExecuteActionWithBusy<TeamDTO>(GetTeamRequestsAsync, TargetTeam);
                    ExecuteActionWithBusy<TeamDTO>(GetExternalInvitesByTeamIdAsync, TargetTeam);
                }
            }
            catch (Exception exc) {
                Debugger.Break();

                await DialogService.ToastAsync(string.Format("{0}. {1}", StateService.CHANGED_TEAMS_HANDLING_ERROR, exc.Message));
            }
        }

        private void OnProfileSettingsEventsProfileUpdated(object sender, ProfileUpdatedArgs e) {
            if (TargetTeam != null && (TargetTeam.CoachId == GlobalSettings.Instance.UserProfile.Id || TargetTeam.OrganizationId == GlobalSettings.Instance.UserProfile.Id ||
                (MembersFromTeam != null && MembersFromTeam.Any(member => member.Member.Id == GlobalSettings.Instance.UserProfile.Id)))) {
                try {
                    ExecuteActionWithBusy<long>(GetTeamByIdAsync, TargetTeam.Id);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }
            }
        }
    }
}