using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Services.Invites;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent.Popups;
using PeakMVP.ViewModels.MainContent.Teams;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class OrganizationProfileContentViewModel : UserTypeDependentProfileContentBaseViewModel, IProfileInfoDependent {

        private readonly IInviteService _inviteService;

        public OrganizationProfileContentViewModel(
            IInviteService inviteService,
            IValidationObjectFactory validationObjectFactory) {

            _inviteService = inviteService;

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
                SetProperty(ref _addTeamPopupViewModel, value);
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

        string _displayName;
        public string DisplayName {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        public override Task AskToRefreshAsync() =>
            Task.Run(async () => {
                Device.BeginInvokeOnMainThread(() => ResolveProfileInfo());
                await TeamMemberProviderViewModel?.AskToRefreshAsync();
                await AddTeamPopupViewModel?.AskToRefreshAsync();
            });

        public override void Dispose() {
            base.Dispose();

            TeamMemberProviderViewModel?.Dispose();
            AddTeamPopupViewModel?.Dispose();
        }

        public void ResolveProfileInfo() => DisplayName = GlobalSettings.Instance.UserProfile.DisplayName;

        protected override void TakeIntent() {
            base.TakeIntent();

            ResolveProfileInfo();
            TeamMemberProviderViewModel?.AskToRefreshAsync();
        }

        public override Task InitializeAsync(object navigationData) {
            AddTeamPopupViewModel?.InitializeAsync(navigationData);
            TeamMemberProviderViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated;
        }

        private void OnProfileSettingsEventsProfileUpdated(object sender, ProfileUpdatedArgs e) => ResolveProfileInfo();
    }
}
