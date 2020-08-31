using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.AppNavigation.Character;
using PeakMVP.Models.Arguments.AppEventsArguments.TeamEvents;
using PeakMVP.Models.DataItems.MainContent.Teams;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.DataItems.MainContent.Teams;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.Character;
using PeakMVP.ViewModels.MainContent.Messenger.Arguments;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using PeakMVP.ViewModels.MainContent.Teams;
using PeakMVP.Views.CompoundedViews.MainContent.Members;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Members {
    public class MembersViewModel : TabbedViewModel {

        CancellationTokenSource _getTeamMembersCancellationTokenSource = new CancellationTokenSource();

        private readonly ITeamMembersDataItems _teamMembersDataItems;
        private readonly ITeamService _teamService;

        public MembersViewModel(
            ITeamMembersDataItems teamMembersDataItems,
            ITeamService teamService) {

            _teamMembersDataItems = teamMembersDataItems;
            _teamService = teamService;

            Filters = _teamMembersDataItems.BuildTeamMemberFilterItems();

            TeamMemberProviderViewModel = ViewModelLocator.Resolve<TeamMemberProviderViewModel>();
            TeamMemberProviderViewModel.InitializeAsync(this);
            TeamMemberProviderViewModel.TeamMembersUpdated += OnTeamMemberProviderViewModelTeamMembersUpdated;

            IsNestedPullToRefreshEnabled = true;
        }

        public ICommand ViewTeamMemberCharacterInfoCommand => new Command(async (object param) => {
            if (param is TeamMember teamMember && SelectedTeam != null) {
                await NavigationService.NavigateToAsync<CharacterDetailInfoViewModel>(new ViewTeamMemberCharacterInfoArgs() {
                    TargetMember = teamMember.Member,
                    TargetTeam = SelectedTeam.Data.Team,
                    RelativeTeamMember = teamMember
                });
            }
        });

        private TeamMemberViewModel _selectedTeam;
        public TeamMemberViewModel SelectedTeam {
            get => _selectedTeam;
            set {
                SetProperty<TeamMemberViewModel>(ref _selectedTeam, value);

                ResolveFilteredMembers(value, SelectedFilter);
            }
        }

        private TeamMember[] _filteredMembers;
        public TeamMember[] FilteredMembers {
            get => _filteredMembers;
            private set => SetProperty<TeamMember[]>(ref _filteredMembers, value);
        }

        private List<TeamMemberFilterDataItem> _filters;
        public List<TeamMemberFilterDataItem> Filters {
            get => _filters;
            private set {
                SetProperty<List<TeamMemberFilterDataItem>>(ref _filters, value);

                SelectedFilter = _filters.FirstOrDefault();
            }
        }

        private TeamMemberFilterDataItem _selectedFilter;
        public TeamMemberFilterDataItem SelectedFilter {
            get => _selectedFilter;
            set {
                SetProperty<TeamMemberFilterDataItem>(ref _selectedFilter, value);

                ResolveFilteredMembers(SelectedTeam, value);
            }
        }

        private TeamMemberProviderViewModel _teamMemberProviderViewModel;
        public TeamMemberProviderViewModel TeamMemberProviderViewModel {
            get => _teamMemberProviderViewModel;
            private set {
                _teamMemberProviderViewModel?.Dispose();
                _teamMemberProviderViewModel = value;
            }
        }

        protected override Task NestedRefreshAction() => TeamMemberProviderViewModel.AskToRefreshAsync();

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ViewMembersFromTargetTeamArgs viewMembersFromTargetTeamArgs) {
                try {
                    SelectedTeam = TeamMemberProviderViewModel.BuildTeamMemberViewModels(new TeamMember[] { viewMembersFromTargetTeamArgs.TargetTeamMember })?.FirstOrDefault<TeamMemberViewModel>();
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    Debugger.Break();
                }
            }
            else if (navigationData is StartOuterConversationArgs startOuterConversationArgs) {
                try {
                    ///
                    /// TODO: resolve it dynamicaly
                    /// 
                    ((ModeActionBarViewModel)((ContentPageBaseViewModel)NavigationService.LastPageViewModel).ActionBarViewModel).SelectedMode.SelectedBarItemIndex = 4;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    Debugger.Break();
                }
            }

            TeamMemberProviderViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            if (TeamMemberProviderViewModel != null) {
                TeamMemberProviderViewModel.TeamMembersUpdated -= OnTeamMemberProviderViewModelTeamMembersUpdated;
            }

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
            TeamMemberProviderViewModel?.Dispose();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            if (TeamMemberProviderViewModel != null) {
                TeamMemberProviderViewModel.TeamMembersUpdated -= OnTeamMemberProviderViewModelTeamMembersUpdated;
            }
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.MEMBERS_TITLE;
            TabIcon = NavigationContext.MEMBERS_IMAGE_PATH;
            RelativeViewType = typeof(MembersView);
        }

        private async void ResolveFilteredMembers(TeamMemberViewModel targetTeam, TeamMemberFilterDataItem filterValue) {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            if (targetTeam != null && filterValue != null) {
                ResetCancellationTokenSource(ref _getTeamMembersCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getTeamMembersCancellationTokenSource;

                try {
                    FilteredMembers = await _teamService.GetFilteredMembersAsync(targetTeam.Data.Team.Id, filterValue.Filter, cancellationTokenSource);
                }
                catch (Exception exc) {
                    await DialogService.ToastAsync(exc.Message);
                    FilteredMembers = null;
                }
            }
            else {
                FilteredMembers = null;
            }

            UpdateBusyVisualState(busyKey, false);
        }

        private void OnTeamMemberProviderViewModelTeamMembersUpdated(object sender, EventArgs e) {
            if (SelectedTeam != null) {
                TeamMemberViewModel targetTeamMember = TeamMemberProviderViewModel.TeamMembers.FirstOrDefault<TeamMemberViewModel>(teamMember => teamMember.Data.Team.Id == SelectedTeam.Data.Team.Id);
                SelectedTeam = targetTeamMember != null ? targetTeamMember : TeamMemberProviderViewModel.TeamMembers.FirstOrDefault();
            }
            else {
                SelectedTeam = TeamMemberProviderViewModel.TeamMembers.FirstOrDefault();
            }
        }
    }
}
