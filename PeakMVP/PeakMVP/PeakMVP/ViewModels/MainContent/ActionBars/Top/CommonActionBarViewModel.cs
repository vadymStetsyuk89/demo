using PeakMVP.Models.DataItems.MainContent;
using PeakMVP.Models.DataItems.MainContent.MenuOptions;
using PeakMVP.Services.DataItems.MainContent;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Search;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ActionBars.Top {
    public class CommonActionBarViewModel : ViewModelBase, IActionbarViewModel {

        private readonly IMenuOptionsDataItems _menuOptionsDataItems;
        private readonly IIdentityUtilService _identityUtilService;

        public CommonActionBarViewModel(
            IMenuOptionsDataItems menuOptionsDataItems,
            IIdentityUtilService identityUtilService) {

            _menuOptionsDataItems = menuOptionsDataItems;
            _identityUtilService = identityUtilService;

            CommonActionBarSearchViewModel = ViewModelLocator.Resolve<CommonActionBarSearchViewModel>();
        }

        public ICommand ToogleMenuCommand => new Command(() => {
            RelativeContentPageBaseViewModel.IsMenuVisible = !RelativeContentPageBaseViewModel.IsMenuVisible;
        });

        public ICommand LogoCommand => new Command(async () => {
            if (!(string.IsNullOrEmpty(GlobalSettings.Instance.UserProfile.AccesToken))) {
                await NavigationService.NavigateToAsync<DashboardViewModel>();
            }
        });

        public ContentPageBaseViewModel RelativeContentPageBaseViewModel { get; private set; }

        public CommonActionBarSearchViewModel CommonActionBarSearchViewModel { get; private set; }

        bool _hasBackButton;
        public bool HasBackButton {
            get { return _hasBackButton; }
            private set { SetProperty(ref _hasBackButton, value); }
        }

        IEnumerable<MenuOptionDataItem> _menuOptionsList;
        public IEnumerable<MenuOptionDataItem> MenuOptionsList {
            get => _menuOptionsList;
            private set => SetProperty(ref _menuOptionsList, value);
        }

        MenuOptionDataItem _selectedMenuOption;
        public MenuOptionDataItem SelectedMenuOption {
            get => _selectedMenuOption;
            set {
                SetProperty(ref _selectedMenuOption, value);

                if (value != null) {
                    if (value.TargetViewModelType.FullName != RelativeContentPageBaseViewModel.GetType().FullName) {
                        OnMenuOptionSelected(value);
                    }

                    RelativeContentPageBaseViewModel.IsMenuVisible = false;
                    SelectedMenuOption = null;
                }
            }
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is ContentPageBaseViewModel contentPageBaseViewModel) {
                RelativeContentPageBaseViewModel = contentPageBaseViewModel;

                MenuOptionsList = _menuOptionsDataItems.ResolveMenuOptions(GlobalSettings.Instance.UserProfile.ProfileType);
                MenuOptionsList.ForEach(mO => mO.IsSelected = (mO.TargetViewModelType.FullName == RelativeContentPageBaseViewModel.GetType().FullName));

                HasBackButton = NavigationService.CanVisibleBackButton;
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            CommonActionBarSearchViewModel?.Dispose();
        }

        private async void OnMenuOptionSelected(MenuOptionDataItem selectedOption) {
            if (selectedOption.TargetViewModelType.FullName != typeof(object).FullName) {
                if (selectedOption is LogOutMenuOptionDataItem) {
                    _identityUtilService.LogOut();
                }
                else if (selectedOption is ImpersonateLogBackMenuOptionDataItem) {
                    _identityUtilService.ImpersonateLogOut();
                }
                else {
                    await NavigationService.NavigateToAsync(selectedOption.TargetViewModelType);
                }
            }
            else {
                await DialogService.ToastAsync(string.Format("Navigate to {0} in developing.", selectedOption.Title));
            }
        }
    }
}
