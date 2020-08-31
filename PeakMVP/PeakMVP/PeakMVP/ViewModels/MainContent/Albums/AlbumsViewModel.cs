using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.Base;
using PeakMVP.Views.CompoundedViews.MainContent.Albums;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.Albums {
    public class AlbumsViewModel : TabbedViewModel {

        public AlbumsViewModel() {
            ManagePicturesViewModel = ViewModelLocator.Resolve<ManagePicturesViewModel>();
            ManagePicturesViewModel.InitializeAsync(this);

            ManageVideosViewModel = ViewModelLocator.Resolve<ManageVideosViewModel>();
            ManageVideosViewModel.InitializeAsync(this);

            IsNestedPullToRefreshEnabled = true;
        }

        private ManagePicturesViewModel _managePicturesViewModel;
        public ManagePicturesViewModel ManagePicturesViewModel {
            get => _managePicturesViewModel;
            private set {
                _managePicturesViewModel?.Dispose();
                _managePicturesViewModel = value;
            }
        }

        private ManageVideosViewModel _manageVideosViewModel;
        public ManageVideosViewModel ManageVideosViewModel {
            get => _manageVideosViewModel;
            private set {
                _manageVideosViewModel?.Dispose();
                _manageVideosViewModel = value;
            }
        }

        protected override Task NestedRefreshAction() =>
            Task.Run(async() => {
                await ManagePicturesViewModel.AskToRefreshAsync();
                await ManageVideosViewModel.AskToRefreshAsync();
            });

        public override void Dispose() {
            base.Dispose();

            ManagePicturesViewModel?.Dispose();
            ManageVideosViewModel?.Dispose();
        }

        public override Task InitializeAsync(object navigationData) {

            ManagePicturesViewModel?.InitializeAsync(navigationData);
            ManageVideosViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.ALBUMS_TITLE;
            TabIcon = NavigationContext.ALBUMS_IMAGE_PATH;
            RelativeViewType = typeof(AlbumsView);
        }
    }
}
