using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Services.DataItems.Autorization;
using PeakMVP.ViewModels.Authorization.Popups;
using PeakMVP.ViewModels.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Authorization {
    public class HomeViewModel : ContentPageBaseViewModel {

        private IFooterDataItems<FooterDataItem> _footerDataItems;

        public HomeViewModel(IFooterDataItems<FooterDataItem> footerDataItems) {
            _footerDataItems = footerDataItems;

            FooterNavigation = _footerDataItems.BuildDataItems();

            HamburgerMenuPopupViewModel = ViewModelLocator.Resolve<HamburgerMenuPopupViewModel>();
            HamburgerMenuPopupViewModel.InitializeAsync(this);
        }

        public ICommand OnLogoTapCommand => new Command(async () => await DialogService.ToastAsync("On logo tap"));

        public ICommand OnHamburgerTapCommand => new Command(() => HamburgerMenuPopupViewModel.ShowPopupCommand.Execute(null));

        public ICommand CreateProfileCommand => new Command(async () => await NavigationService.NavigateToAsync<CreateProfileViewModel>());

        private HamburgerMenuPopupViewModel _hamburgerMenuPopupViewModel;
        public HamburgerMenuPopupViewModel HamburgerMenuPopupViewModel {
            get => _hamburgerMenuPopupViewModel;
            private set {
                _hamburgerMenuPopupViewModel?.Dispose();

                SetProperty<HamburgerMenuPopupViewModel>(ref _hamburgerMenuPopupViewModel, value);
            }
        }

        public IEnumerable<FooterDataItem> _footerNavigation;
        public IEnumerable<FooterDataItem> FooterNavigation {
            get => _footerNavigation;
            set => SetProperty<IEnumerable<FooterDataItem>>(ref _footerNavigation, value);
        }

        public FooterDataItem _selectedFooterNavigationItem;
        public FooterDataItem SelectedFooterNavigationItem {
            get => _selectedFooterNavigationItem;
            set {
                SetProperty<FooterDataItem>(ref _selectedFooterNavigationItem, value);

                //
                // TODO: navigate to the appropriate view. Also set view model type for data item.
                //
                DialogService.ToastAsync(string.Format("Navigate to the {0}", value.Title));
            }
        }

        public List<object> _playersProfiles = new List<object>() { new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object() };
        public List<object> PlayersProfiles {
            get => _playersProfiles;
            set => SetProperty<List<object>>(ref _playersProfiles, value);
        }

        public override Task InitializeAsync(object navigationData) {
            HamburgerMenuPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            HamburgerMenuPopupViewModel?.Dispose();
        }
    }
}
