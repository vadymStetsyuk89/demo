using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.Authorization.Popups;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Authorization.Popups {
    public class HamburgerMenuPopupViewModel : PopupBaseViewModel {

        public ICommand LoginCommand => new Command(async () => {
            ClosePopupCommand.Execute(null);

            await NavigationService.NavigateToAsync<LoginViewModel>();
        });

        public ICommand RegisterCommand => new Command(async () => {
            ClosePopupCommand.Execute(null);

            await NavigationService.NavigateToAsync<CreateProfileViewModel>();
        });

        public override Type RelativeViewType => typeof(NavigateToAthorizationPopup);
    }
}
