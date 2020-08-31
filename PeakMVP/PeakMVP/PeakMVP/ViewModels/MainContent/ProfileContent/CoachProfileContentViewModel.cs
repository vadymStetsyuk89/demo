using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Services.Invites;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.Sports;
using PeakMVP.Services.TeamMembers;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent.Popups;
using PeakMVP.ViewModels.MainContent.Teams;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class CoachProfileContentViewModel : UserTypeDependentProfileContentBaseViewModel {

        private readonly ISportService _sportService;
        private readonly ISportsFactory _sportsFactory;
        private readonly IInviteService _inviteService;
        private readonly ITeamMemberFactory _teamMemberFactory;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IStateService _stateService;

        public CoachProfileContentViewModel(
            IInviteService inviteService,
            ISportService sportService,
            ISportsFactory sportsFactory,
            ITeamMemberFactory teamMemberFactory,
            ITeamMemberService teamMemberService,
            IValidationObjectFactory validationObjectFactory,
            IStateService stateService) {

            _sportService = sportService;
            _sportsFactory = sportsFactory;
            _inviteService = inviteService;
            _teamMemberFactory = teamMemberFactory;
            _teamMemberService = teamMemberService;
            _validationObjectFactory = validationObjectFactory;
            _stateService = stateService;

            AddTeamPopupViewModel = ViewModelLocator.Resolve<AddTeamPopupViewModel>();
            AddTeamPopupViewModel.InitializeAsync(this);

            TeamMemberProviderViewModel = ViewModelLocator.Resolve<TeamMemberProviderViewModel>();
            TeamMemberProviderViewModel.InitializeAsync(this);
        }

        private AddTeamPopupViewModel _addTeamPopupViewModel;
        public AddTeamPopupViewModel AddTeamPopupViewModel {
            get => _addTeamPopupViewModel;
            private set {
                _addTeamPopupViewModel?.Dispose();
                SetProperty<AddTeamPopupViewModel>(ref _addTeamPopupViewModel, value);
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

        public override Task AskToRefreshAsync() => Task.Run(async () => {
            await TeamMemberProviderViewModel?.AskToRefreshAsync();
            await AddTeamPopupViewModel?.AskToRefreshAsync();
        });

        protected override void TakeIntent() {
            base.TakeIntent();

            TeamMemberProviderViewModel?.AskToRefreshAsync();
        }

        public override Task InitializeAsync(object navigationData) {

            AddTeamPopupViewModel?.InitializeAsync(navigationData);
            TeamMemberProviderViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            TeamMemberProviderViewModel?.Dispose();
            AddTeamPopupViewModel?.Dispose();
        }
    }
}
