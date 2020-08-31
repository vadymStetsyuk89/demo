using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileContent;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent {
    public sealed class DashboardViewModel : TabbedViewModel {

        public DashboardViewModel() {
            ProfileType = GlobalSettings.Instance.UserProfile.ProfileType;

            ProfileContentViewModel = ViewModelLocator.Resolve<ProfileContentViewModel>();
        }

        public ProfileContentViewModel ProfileContentViewModel { get; private set; }

        ProfileType _profileType;
        public ProfileType ProfileType {
            get => _profileType;
            set => SetProperty(ref _profileType, value);
        }

        public override Task InitializeAsync(object navigationData) {
            ProfileContentViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            CancellationService?.Cancel();

            ProfileContentViewModel?.Dispose();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            CancellationService?.Cancel();
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.PROFILE_TITLE;
            TabIcon = NavigationContext.PROFILE_IMAGE_PATH;
            RelativeViewType = typeof(DashboardContentView);
        }
    }
}
