using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Arguments.AppEventsArguments.TeamEvents;
using PeakMVP.Models.Exceptions;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using PeakMVP.Views.CompoundedViews.MainContent.Teams;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Teams {
    public class TeamsViewModel : TabbedViewModel {

        private readonly ITeamService _teamService;

        private CancellationTokenSource _deleteTeamCancellationTokenSource = new CancellationTokenSource();

        public TeamsViewModel(
            ITeamService teamService) {
             
            _teamService = teamService;

            TeamMemberProviderViewModel = ViewModelLocator.Resolve<TeamMemberProviderViewModel>();
            TeamMemberProviderViewModel.InitializeAsync(this);

            IsNestedPullToRefreshEnabled = true;
        }

        public ICommand DeleteTeamCommand => new Command(async (object param) => {
            if (param is TeamMemberViewModel teamMemberToDelete) {
                ResetCancellationTokenSource(ref _deleteTeamCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _deleteTeamCancellationTokenSource;

                Guid busyKey = Guid.NewGuid();
                UpdateBusyVisualState(busyKey, true);

                try {
                    bool deleteCompletion = await _teamService.RemoveTeamByIdAsync(teamMemberToDelete.Data.Team.Id, cancellationTokenSource);

                    if (!deleteCompletion) {
                        throw new InvalidOperationException(string.Format(TeamService.REMOVE_TEAM_BY_ID_COMMON_ERROR_MESSAGE, teamMemberToDelete.Team));
                    }

                    GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeletedInvoke(this, teamMemberToDelete.Data.Team);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }

                UpdateBusyVisualState(busyKey, false);
            }
        });

        public ICommand ViewTeamMembersCommand => new Command((object param) => {
            if (param is TeamMemberViewModel targetTeamMember) {
                GlobalSettings.Instance.AppMessagingEvents.TeamEvents.ViewMembersFromTargetTeamInvoke(this, new ViewMembersFromTargetTeamArgs() { TargetTeamMember = targetTeamMember.Data });
            }
        });

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

            TeamMemberProviderViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _deleteTeamCancellationTokenSource);

            TeamMemberProviderViewModel?.Dispose();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _deleteTeamCancellationTokenSource);
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.TEAMS_TITLE;
            TabIcon = NavigationContext.TEAMS_IMAGE_PATH;
            RelativeViewType = typeof(TeamsView);
        }
    }
}
