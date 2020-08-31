using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Services.Identity;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Profile;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent.Arguments;
using PeakMVP.ViewModels.MainContent.ProfileContent.Popups;
using PeakMVP.ViewModels.MainContent.ProfileSettings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class ParentProfileContentViewModel : UserTypeDependentProfileContentBaseViewModel, IProfileInfoDependent {

        private readonly IChildrenFactory _childrenFactory;
        private readonly IProfileService _profileService;
        private readonly IIdentityUtilService _identityUtilService;

        private CancellationTokenSource _getProfileCancellationTokenSource = new CancellationTokenSource();

        public ParentProfileContentViewModel(
            IValidationObjectFactory validationObjectFactory,
            IIdentityService identityService,
            IChildrenFactory childrenFactory,
            IProfileService profileService,
            IIdentityUtilService identityUtilService) {

            _childrenFactory = childrenFactory;
            _profileService = profileService;
            _identityUtilService = identityUtilService;

            AddChildrenPopupViewModel = ViewModelLocator.Resolve<AddChildrenPopupViewModel>();
            AddChildrenPopupViewModel.InitializeAsync(this);
        }

        public ICommand AddChildCommand => new Command(() => {
            UpdatePopupScopeVisibility(true);
            AddChildrenPopupViewModel.IsPopupVisible = true;
        });

        public ICommand OverviewChildCommand => new Command(async (object param) => {
            if (param is ChildItemViewModel) {
                await NavigationService.NavigateToAsync<ProfileInfoViewModel>(param);
            }
        });

        public ICommand EditChildProfileCommand => new Command(async (object param) => {
            if (param is ChildItemViewModel) {
                await NavigationService.NavigateToAsync<ChildSettingsUpdateViewModel>(param);
            }
        });

        string _displayName;
        public string DisplayName {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        ObservableCollection<ChildItemViewModel> _pickedChildren = new ObservableCollection<ChildItemViewModel>();
        public ObservableCollection<ChildItemViewModel> PickedChildren {
            get => _pickedChildren;
            set {
                _pickedChildren?.ForEach(cIVM => cIVM.Dispose());
                SetProperty(ref _pickedChildren, value);
            }
        }

        private AddChildrenPopupViewModel _addChildrenPopupViewModel;
        public AddChildrenPopupViewModel AddChildrenPopupViewModel {
            get => _addChildrenPopupViewModel;
            private set {
                _addChildrenPopupViewModel?.Dispose();
                SetProperty<AddChildrenPopupViewModel>(ref _addChildrenPopupViewModel, value);
            }
        }

        public void ResolveProfileInfo() {
            DisplayName = GlobalSettings.Instance.UserProfile.DisplayName;
        }

        public override Task AskToRefreshAsync() =>
            Task.Run(() => ExecuteActionWithBusy(GetProfileAsync));

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getProfileCancellationTokenSource);

            AddChildrenPopupViewModel?.Dispose();
            PickedChildren?.ForEach(cVM => cVM.Dispose());
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getProfileCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetProfileAsync);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ChildrenUpdated += OnProfileSettingsEventsChildrenUpdated;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ChildrenUpdated -= OnProfileSettingsEventsChildrenUpdated;
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ChildCreatedArgs) {
                GetChildren();
            }

            AddChildrenPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        private void GetChildren() {
            PickedChildren?.ForEach(cVM => cVM.Dispose());

            var children = GlobalSettings.Instance.UserProfile?.Children;
            if (children != null && children.Any()) {
                PickedChildren = (_childrenFactory.MakeChildrenItems(children)).ToObservableCollection();
            }
            else {
                PickedChildren = new ObservableCollection<ChildItemViewModel>();
            }

            OnPropertyChanged(nameof(PickedChildren));
        }

        private Task GetProfileAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getProfileCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getProfileCancellationTokenSource;

                try {
                    GetProfileResponse getProfileResponse = await _profileService.GetProfileAsync();

                    if (!await _identityUtilService.ChargeUserProfileAsync(await _profileService.GetProfileAsync(), false)) {
                        throw new InvalidOperationException(ProfileService.CANT_RESOLVE_PROFILE_COMMON_ERROR_MESSAGE);
                    }
                    else {
                        Device.BeginInvokeOnMainThread(() => {
                            ResolveProfileInfo();
                            GetChildren();
                        });
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private async void OnProfileSettingsEventsChildrenUpdated(object sender, EventArgs e) => await GetProfileAsync();
    }
}
