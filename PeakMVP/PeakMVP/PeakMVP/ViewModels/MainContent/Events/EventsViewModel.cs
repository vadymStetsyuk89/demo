using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Helpers;
using PeakMVP.Helpers.AppEvents.Events;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Events.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class EventsViewModel : TabbedViewModel, IEventManagementDependent {

        private readonly ITeamMemberService _teamMemberService;
        private readonly ITeamActionsManagmentDataItems _teamActionsManagmentDataItems;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getTeamMembersCancellationTokenSource = new CancellationTokenSource();

        public EventsViewModel(
            ITeamMemberService teamMemberService,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems,
            IStateService stateService) {
            _teamMemberService = teamMemberService;
            _teamActionsManagmentDataItems = teamActionsManagmentDataItems;
            _stateService = stateService;

            GameEventActions = _teamActionsManagmentDataItems.BuildActionCreatingDataItems(new Command(async (object args) => {
                ContentPageBaseViewModel relativeBasePage = NavigationService.LastPageViewModel as ContentPageBaseViewModel;
                Guid busyKey;

                if (relativeBasePage != null) {
                    busyKey = Guid.NewGuid();
                    relativeBasePage.SetBusy(busyKey, true);
                    await Task.Delay(AppConsts.DELAY_STUB);
                }

                if (((TeamActionManagmentDataItem)args).ActionTitle == TeamActionsManagmentDataItems.NEW_GAME_ACTION_TITLE) {
                    await NavigationService.NavigateToAsync<CreateNewGameViewModel>(SelectedTeam);
                } else if (((TeamActionManagmentDataItem)args).ActionTitle == TeamActionsManagmentDataItems.NEW_EVENT_ACTION_TITLE) {
                    await NavigationService.NavigateToAsync<CreateNewEventViewModel>(SelectedTeam);
                } else {
                    Debugger.Break();
                }

                if (relativeBasePage != null) {
                    relativeBasePage.SetBusy(busyKey, false);
                }
            }));

            IsNestedPullToRefreshEnabled = true;
        }

        public ICommand ToggleEventsVisualizationCommand => new Command((object args) => {
            SelectedPossibleVisualizationIndex = int.Parse(args.ToString());
        });

        private bool _isEventsManagementAvailable = (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization);
        public bool IsEventsManagementAvailable {
            get => _isEventsManagementAvailable;
            private set => SetProperty<bool>(ref _isEventsManagementAvailable, value);
        }

        private List<NestedViewModel> _possibleVisualizations = new List<NestedViewModel>() {
            ViewModelLocator.Resolve<ListViewingOfEventsAndGamesContentViewModel>(),
            ViewModelLocator.Resolve<CalendarViewingOfEventsAndGamesContentViewModel>()
        };
        public List<NestedViewModel> PossibleVisualizations {
            get => _possibleVisualizations;
            private set {
                _possibleVisualizations?.ForEach(nVM => nVM.Dispose());

                SetProperty<List<NestedViewModel>>(ref _possibleVisualizations, value);
            }
        }

        private int _selectedPossibleVisualizationIndex;
        public int SelectedPossibleVisualizationIndex {
            get => _selectedPossibleVisualizationIndex;
            set {
                SetProperty<int>(ref _selectedPossibleVisualizationIndex, value);

                HandleSelectedTeam(SelectedTeam);
            }
        }

        private ObservableCollection<TeamMember> _teams;
        public ObservableCollection<TeamMember> Teams {
            get => _teams;
            set => SetProperty<ObservableCollection<TeamMember>>(ref _teams, value);
        }

        private TeamMember _selectedTeam;
        public TeamMember SelectedTeam {
            get => _selectedTeam;
            set {
                SetProperty<TeamMember>(ref _selectedTeam, value);

                HandleSelectedTeam(value);
            }
        }

        private List<TeamActionManagmentDataItem> _gameEventActions;
        public List<TeamActionManagmentDataItem> GameEventActions {
            get => _gameEventActions;
            private set => SetProperty<List<TeamActionManagmentDataItem>>(ref _gameEventActions, value);
        }

        protected override Task NestedRefreshAction() => GetTeamMembersAsync();

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
            PossibleVisualizations?.ForEach(nVM => nVM.Dispose());
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            GetTeamMembersAsync();
        }

        public override Task InitializeAsync(object navigationData) {
            PossibleVisualizations?.ForEach(nVM => nVM.InitializeAsync(navigationData));

            return base.InitializeAsync(navigationData);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.GameUpdated += OnScheduleEventsGameUpdated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewGameCreated += OnScheduleEventsNewGameCreated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.EventCreated += OnScheduleEventsEventCreated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.EventUpdated += OnScheduleEventsEventUpdated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;

            _stateService.ChangedTeams += OnStateServiceChangedTeams;

            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted, (sender, teamId) => GetTeamMembersAsync());
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, teamId) => GetTeamMembersAsync());
            MessagingCenter.Subscribe<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped, (sender, team) => GetTeamMembersAsync());
            MessagingCenter.Subscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestDeclinedForNewTeamMember, (sender) => GetTeamMembersAsync());
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.GameUpdated -= OnScheduleEventsGameUpdated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewGameCreated -= OnScheduleEventsNewGameCreated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.EventCreated -= OnScheduleEventsEventCreated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.EventUpdated -= OnScheduleEventsEventUpdated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;

            _stateService.ChangedTeams -= OnStateServiceChangedTeams;

            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);
            MessagingCenter.Unsubscribe<object, TeamDTO>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.PartnershipWithOrganizationStopped);
            MessagingCenter.Unsubscribe<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestDeclinedForNewTeamMember);
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.EVENTS_TITLE;
            TabIcon = NavigationContext.EVENTS_IMAGE_PATH;
            RelativeViewType = typeof(EventsView);
        }

        private Task GetTeamMembersAsync(TeamMember teamTryToSelect = null) =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getTeamMembersCancellationTokenSource;

                try {
                    List<TeamMember> teanmMembers = await _teamMemberService.GetTeamMembersAsync(cancellationTokenSource.Token, GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent);
                    ObservableCollection<TeamMember> resolvedTeams = teanmMembers != null ? teanmMembers.ToObservableCollection() : new ObservableCollection<TeamMember>();

                    Device.BeginInvokeOnMainThread(() => {
                        Teams = resolvedTeams;

                        if (teamTryToSelect != null) {
                            TeamMember targetTeam = Teams.FirstOrDefault(tMDTO => tMDTO.Id == teamTryToSelect.Id);

                            if (targetTeam != null) {
                                SelectedTeam = targetTeam;
                            } else {
                                SelectedTeam = Teams.FirstOrDefault();
                            }
                        } else {
                            SelectedTeam = Teams.FirstOrDefault();
                        }
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

        private async void HandleSelectedTeam(TeamMember teamMember) {
            try {
                await PossibleVisualizations[SelectedPossibleVisualizationIndex].InitializeAsync(new ExtractEventsAndGamesForSelectedTeamArgs() { TargetTeamMember = teamMember });
            }
            catch (Exception exc) {
                //Debugger.Break();
            }
        }

        private void OnScheduleEventsGameUpdated(object sender, GameManagmentArgs e) => GetTeamMembersAsync(e.TeamMember);

        private void OnScheduleEventsNewGameCreated(object sender, GameManagmentArgs e) => GetTeamMembersAsync(e.TeamMember);

        private void OnScheduleEventsEventCreated(object sender, EventManagmentArgs e) => GetTeamMembersAsync(e.TeamMember);

        private void OnScheduleEventsEventUpdated(object sender, EventManagmentArgs e) => GetTeamMembersAsync(e.TeamMember);

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => GetTeamMembersAsync();

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => GetTeamMembersAsync();

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => GetTeamMembersAsync();
    }
}
