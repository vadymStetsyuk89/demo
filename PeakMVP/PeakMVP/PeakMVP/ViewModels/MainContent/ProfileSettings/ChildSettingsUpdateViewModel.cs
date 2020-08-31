using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Services.Profile;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using PeakMVP.ViewModels.MainContent.ProfileSettings.ProfileSettingsPopups;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings {
    public class ChildSettingsUpdateViewModel : LoggedContentPageViewModel, IInputForm {

        private static readonly string CANT_RESOLVE_PROFILE_VALUES_COMMON_ERROR_MESSAGE = "Can't resolve profile values";

        protected readonly IValidationObjectFactory _validationObjectFactory;
        protected readonly IProfileService _profileService;
        private readonly IProfileFactory _profileFactory;

        private CancellationTokenSource _getChildProfileCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _updateChildSettingsCancellationTokenSource = new CancellationTokenSource();

        public ChildSettingsUpdateViewModel(
            IValidationObjectFactory validationObjectFactory,
            IProfileService profileService,
            IProfileFactory profileFactory) {

            _validationObjectFactory = validationObjectFactory;
            _profileService = profileService;
            _profileFactory = profileFactory;

            ActionBarViewModel = ViewModelLocator.Resolve<ModeActionBarViewModel>();
            ActionBarViewModel.InitializeAsync(this);
            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = false;

            PickAvatarPopupViewModel = ViewModelLocator.Resolve<PickChildAvatarPopupViewModel>();
            PickAvatarPopupViewModel.InitializeAsync(this);

            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

            IsPullToRefreshEnabled = false;

            ResetValidationObjects();
        }

        public ICommand SaveCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _updateChildSettingsCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _updateChildSettingsCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                if (ValidateForm()) {
                    SetProfileDataModel setProfileDataModel = new SetProfileDataModel() {
                        FirstName = FirstName.Value,
                        LastName = LastName.Value,
                        About = AboutYou.Value,
                        MySports = MySports.Value,
                        ProfileId = TargetChildProfile.Id,
                        Phone = PhoneNumber.Value
                    };

                    SetProfileSettingsResponse setProfileSettingsResponse = await _profileService.SetProfileAsync(setProfileDataModel, cancellationTokenSource);

                    if (setProfileSettingsResponse != null) {
                        GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ChildrenUpdatedInvoke(this);

                        await NavigationService.GoBackAsync();
                    }
                    else {
                        throw new InvalidOperationException(ProfileService.SET_PROFILE_SETTINGS_COMMON_ERROR_MESSAGE);
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        });

        public ICommand ResetCommand => new Command(() => ResolveChildProfileValues(TargetChildProfile));

        private PickAvatarPopupViewModelBase _pickAvatarPopupViewModel;
        public PickAvatarPopupViewModelBase PickAvatarPopupViewModel {
            get => _pickAvatarPopupViewModel;
            set => SetProperty(ref _pickAvatarPopupViewModel, value);
        }

        private ProfileDTO _targetChildProfile;
        public ProfileDTO TargetChildProfile {
            get => _targetChildProfile;
            set => SetProperty<ProfileDTO>(ref _targetChildProfile, value);
        }

        private string _title = "";
        public string Title {
            get => _title;
            private set => SetProperty<string>(ref _title, value);
        }

        private string _avatar = "";
        public string Avatar {
            get => _avatar;
            private set => SetProperty<string>(ref _avatar, value);
        }

        private ValidatableObject<string> _firstName;
        public ValidatableObject<string> FirstName {
            get => _firstName;
            set => SetProperty<ValidatableObject<string>>(ref _firstName, value);
        }

        private ValidatableObject<string> _lastName;
        public ValidatableObject<string> LastName {
            get => _lastName;
            set => SetProperty<ValidatableObject<string>>(ref _lastName, value);
        }

        private ValidatableObject<string> _phoneNumber;
        public ValidatableObject<string> PhoneNumber {
            get => _phoneNumber;
            set => SetProperty<ValidatableObject<string>>(ref _phoneNumber, value);
        }

        private ValidatableObject<string> _aboutYou;
        public ValidatableObject<string> AboutYou {
            get => _aboutYou;
            set => SetProperty<ValidatableObject<string>>(ref _aboutYou, value);
        }

        private ValidatableObject<string> _mySports;
        public ValidatableObject<string> MySports {
            get => _mySports;
            set => SetProperty<ValidatableObject<string>>(ref _mySports, value);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is ChildItemViewModel childItemViewModel) {
                GetChildProfileAsync(childItemViewModel.Profile.ShortId);
            }

            PickAvatarPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            PickAvatarPopupViewModel?.Dispose();

            ResetCancellationTokenSource(ref _getChildProfileCancellationTokenSource);
            ResetCancellationTokenSource(ref _updateChildSettingsCancellationTokenSource);
        }

        public void ResetInputForm() {
            ResetValidationObjects();

            ResolveChildProfileValues(TargetChildProfile);
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            FirstName.Validate();
            LastName.Validate();
            AboutYou.Validate();
            MySports.Validate();
            PhoneNumber.Validate();

            isValidResult = FirstName.IsValid && LastName.IsValid && PhoneNumber.IsValid;

            return isValidResult;
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getChildProfileCancellationTokenSource);
            ResetCancellationTokenSource(ref _updateChildSettingsCancellationTokenSource);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ChildrenUpdated += OnProfileSettingsEventsChildrenUpdated;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ChildrenUpdated -= OnProfileSettingsEventsChildrenUpdated;
        }

        private async void ResolveChildProfileValues(ProfileDTO profile) {
            try {
                Title = string.Format("{0} {1}'s profile settings", profile.FirstName, profile.LastName);
                Avatar = profile.Avatar?.Url;
                FirstName.Value = profile.FirstName;
                LastName.Value = profile.LastName;
                AboutYou.Value = profile.About;
                MySports.Value = profile.MySports;
                PhoneNumber.Value = profile.Contact?.Phone;
            }
            catch (Exception exc) {
                Debugger.Break();
                Crashes.TrackError(exc);

                ResetValidationObjects();
                Title = null;
                Avatar = null;

                await DialogService.ToastAsync(CANT_RESOLVE_PROFILE_VALUES_COMMON_ERROR_MESSAGE);
            }
        }

        private Task GetChildProfileAsync(string profileShortId) =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getChildProfileCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getChildProfileCancellationTokenSource;

                try {
                    ProfileDTO childProfile = _profileFactory.BuildProfileDTO(await _profileService.GetProfileByShortIdAsync(profileShortId, cancellationTokenSource.Token));

                    if (childProfile != null) {
                        TargetChildProfile = childProfile;

                        Device.BeginInvokeOnMainThread(() => ResolveChildProfileValues(TargetChildProfile));
                    }
                    else {
                        throw new InvalidOperationException(ProfileService.CANT_RESOLVE_PROFILE_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private void ResetValidationObjects() {
            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            PhoneNumber = _validationObjectFactory.GetValidatableObject<string>();

            AboutYou = _validationObjectFactory.GetValidatableObject<string>();

            MySports = _validationObjectFactory.GetValidatableObject<string>();
        }

        private async void OnProfileSettingsEventsChildrenUpdated(object sender, EventArgs e) => await GetChildProfileAsync(TargetChildProfile.ShortId);
    }
}
