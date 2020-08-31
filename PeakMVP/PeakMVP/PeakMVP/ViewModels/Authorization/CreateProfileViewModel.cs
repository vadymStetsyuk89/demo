using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Services.DataItems.Autorization;
using PeakMVP.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Authorization {
    public sealed class CreateProfileViewModel : ContentPageBaseViewModel {

        public CreateProfileViewModel(ICreateProfileDataItems<ProfileTypeItem> createProfileDataItems) {
            ProfileTypeItems = createProfileDataItems.BuildDataItems();
        }

        public ICommand LoginCommand => new Command(async () => await NavigationService.NavigateToAsync<LoginViewModel>());

        public ICommand ContinueCommand => new Command(async () => {
            await NavigationService.NavigateToAsync<RegistrationViewModel>(ProfileTypeItem);
        });

        public ICommand RequestToScrollToEndCommand { get; set; }

        ObservableCollection<ProfileTypeItem> _profileTypeItems;
        public ObservableCollection<ProfileTypeItem> ProfileTypeItems {
            get => _profileTypeItems;
            set => SetProperty(ref _profileTypeItems, value);
        }

        object _selectedItem;
        public object SelectedItem {
            get => _selectedItem;
            set {
                if (SetProperty(ref _selectedItem, value)) {
                    if (value != null) {
                        AllowedContinue = true;
                        ProfileTypeItem = value as ProfileTypeItem;
                        RequestToScrollToEndCommand?.Execute(null);
                    }
                    else {
                        AllowedContinue = false;
                    }
                }
            }
        }

        ProfileTypeItem _profileTypeItem;
        public ProfileTypeItem ProfileTypeItem {
            get { return _profileTypeItem; }
            set { SetProperty(ref _profileTypeItem, value); }
        }

        bool _allowedContinue;
        public bool AllowedContinue {
            get { return _allowedContinue; }
            set { SetProperty(ref _allowedContinue, value); }
        }

        public override Task InitializeAsync(object navigationData) {
            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ProfileTypeItems.Clear();
        }
    }
}
