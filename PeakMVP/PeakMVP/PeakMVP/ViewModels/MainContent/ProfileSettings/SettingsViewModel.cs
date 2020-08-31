using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Services.Identity;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.Profile;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileSettings.ProfileSettingsPopups;
using PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileSettings;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings {
    public class SettingsViewModel : TabbedViewModel, IProfileInfoDependent, IInputForm {

        private static readonly string PROFILE_SUCCESSFULLY_UPDATED_MESSAGE = "Profile successfully updated";
        private static readonly string PASWORD_MATCHING_WARNING = "New password does not match";

        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly IMediaPickerService _mediaPickerService;
        private readonly IIdentityService _identityService;
        private readonly IProfileService _profileService;

        private CancellationTokenSource _setProfileSettingsCancellationTokenSource = new CancellationTokenSource();

        public SettingsViewModel(
            IValidationObjectFactory validationObjectFactory,
            IMediaPickerService mediaPickerService,
            IProfileService profileService,
            IIdentityUtilService identityUtilService,
            IIdentityService identityService) {

            _validationObjectFactory = validationObjectFactory;
            _identityUtilService = identityUtilService;
            _mediaPickerService = mediaPickerService;
            _identityService = identityService;
            _profileService = profileService;

            switch (GlobalSettings.Instance.UserProfile.ProfileType) {
                case Models.DataItems.Autorization.ProfileType.Fan:
                    SelfInformationViewModel = ViewModelLocator.Resolve<FanSelfInformationViewModel>();
                    break;
                case Models.DataItems.Autorization.ProfileType.Player:
                    SelfInformationViewModel = ViewModelLocator.Resolve<PlayerSelfInformationViewModel>();
                    break;
                case Models.DataItems.Autorization.ProfileType.Parent:
                    SelfInformationViewModel = ViewModelLocator.Resolve<ParentSelfInformationViewModel>();
                    break;
                case Models.DataItems.Autorization.ProfileType.Organization:
                    SelfInformationViewModel = ViewModelLocator.Resolve<OrganizationSelfInformationViewModel>();
                    break;
                case Models.DataItems.Autorization.ProfileType.Coach:
                    SelfInformationViewModel = ViewModelLocator.Resolve<CoachSelfInformationViewModel>();
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            SelfInformationViewModel?.InitializeAsync(this);

            PickAvatarPopupViewModel = ViewModelLocator.Resolve<PickProfileAvatarPopupViewModel>();
            PickAvatarPopupViewModel.InitializeAsync(this);

            ResetInputForm();

            IsImpersonateLogBackAvailable = (GlobalSettings.Instance.UserProfile.ImpersonateProfile != null);
            IsNestedPullToRefreshEnabled = true;
        }

        public ICommand ImpersonateLogBackCommand => new Command(() => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            _identityUtilService.ImpersonateLogOut();

            UpdateBusyVisualState(busyKey, false);
        }, () => IsImpersonateLogBackAvailable);

        public ICommand LogoutCommand => new Command(() => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            _identityUtilService.LogOut();

            UpdateBusyVisualState(busyKey, false);
        });

        public ICommand SaveCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _setProfileSettingsCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _setProfileSettingsCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                if (ValidateForm()) {
                    SetProfileDataModel setProfileDataModel = SelfInformationViewModel.GetInputValues();
                    setProfileDataModel.Email = EmaiAddress.Value;
                    setProfileDataModel.Phone = PhoneNumber.Value;
                    setProfileDataModel.CurrentPassword = CurrentPassword.Value;
                    setProfileDataModel.NewPassword = NewPassword.Value;
                    setProfileDataModel.ConfirmNewPassword = RepeatNewPassword.Value;

                    SetProfileSettingsResponse setProfileSettingsResponse = await _profileService.SetProfileAsync(setProfileDataModel, cancellationTokenSource);

                    await _identityUtilService.ChargeUserProfileAsync(setProfileSettingsResponse, false);
                    ResolveProfileInfo();

                    GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdatedInvoke(this, new ProfileUpdatedArgs());

                    await DialogService.ToastAsync(PROFILE_SUCCESSFULLY_UPDATED_MESSAGE);
                    ResetPasswordsValidationObjects();
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            UpdateBusyVisualState(busyKey, false);
        });

        //private bool _isEmailFormAvailable;
        //public bool IsEmailFormAvailable {
        //    get => _isEmailFormAvailable;
        //    private set => SetProperty<bool>(ref _isEmailFormAvailable, value);
        //}

        private bool _isImpersonateLogBackAvailable;
        public bool IsImpersonateLogBackAvailable {
            get => _isImpersonateLogBackAvailable;
            set => SetProperty<bool>(ref _isImpersonateLogBackAvailable, value);
        }

        private PickAvatarPopupViewModelBase _pickAvatarPopupViewModel;
        public PickAvatarPopupViewModelBase PickAvatarPopupViewModel {
            get => _pickAvatarPopupViewModel;
            set => SetProperty(ref _pickAvatarPopupViewModel, value);
        }

        private ISelfInformationViewModel _selfInformationViewModel;
        public ISelfInformationViewModel SelfInformationViewModel {
            get => _selfInformationViewModel;
            private set => SetProperty(ref _selfInformationViewModel, value);
        }

        private string _avatar = "";
        public string Avatar {
            get => _avatar;
            private set => SetProperty<string>(ref _avatar, value);
        }

        private ValidatableObject<string> _emaiAddress;
        public ValidatableObject<string> EmaiAddress {
            get => _emaiAddress;
            set => SetProperty(ref _emaiAddress, value);
        }

        private ValidatableObject<string> _phoneNumber;
        public ValidatableObject<string> PhoneNumber {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private ValidatableObject<string> _currentPassword;
        public ValidatableObject<string> CurrentPassword {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        private ValidatableObject<string> _newPassword;
        public ValidatableObject<string> NewPassword {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        private ValidatableObject<string> _repeatNewPassword;
        public ValidatableObject<string> RepeatNewPassword {
            get => _repeatNewPassword;
            set => SetProperty(ref _repeatNewPassword, value);
        }

        public void ResetInputForm() {
            SelfInformationViewModel?.ResetInputForm();
            ResetValidationObjects();

            PickAvatarPopupViewModel?.ResetValues();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            CurrentPassword.Validate();
            NewPassword.Validate();
            RepeatNewPassword.Validate();
            EmaiAddress.Validate();
            PhoneNumber.Validate();

            //if (IsEmailFormAvailable) {
            //    EmaiAddress.Validate();
            //}
            bool newPasswordMatching = true;

            if (!string.IsNullOrEmpty(NewPassword.Value) || !string.IsNullOrEmpty(RepeatNewPassword.Value)) {
                newPasswordMatching = NewPassword.Value == RepeatNewPassword.Value;
                if (!newPasswordMatching) {
                    ///
                    /// Miss this await thing (in this case its ok)
                    /// 
                    DialogService.ToastAsync(PASWORD_MATCHING_WARNING);
                }
            }

            isValidResult = newPasswordMatching && EmaiAddress.IsValid && CurrentPassword.IsValid && NewPassword.IsValid && RepeatNewPassword.IsValid && PhoneNumber.IsValid && SelfInformationViewModel.ValidateForm();

            return isValidResult;
        }

        protected override Task NestedRefreshAction() =>
            Task.Run(async () => {
                try {
                    await _identityUtilService.ChargeUserProfileAsync(await _profileService.GetProfileAsync(), false);
                    Device.BeginInvokeOnMainThread(() => {
                        ResetInputForm();
                        ResolveProfileInfo();
                    });
                    ResolveProfileInfo();
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    await DialogService.ToastAsync(ex.Message);
                }
            });

        public void ResolveProfileInfo() =>
            Device.BeginInvokeOnMainThread(() => {
                SelfInformationViewModel.ResolveProfileInfo();
                EmaiAddress.Value = GlobalSettings.Instance.UserProfile.Contact.Email;
                PhoneNumber.Value = GlobalSettings.Instance.UserProfile.Contact.Phone;
                Avatar = GlobalSettings.Instance.UserProfile.Avatar?.Url;
            });

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _setProfileSettingsCancellationTokenSource);

            SelfInformationViewModel?.Dispose();
            PickAvatarPopupViewModel?.Dispose();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _setProfileSettingsCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ResolveProfileInfo();

            ///
            /// Assuming thta we logged under the Player U13
            /// 
            //IsEmailFormAvailable = (DateTime.Now.Year - GlobalSettings.Instance.UserProfile.DateOfBirth.Year) >= UserProfile.YOUNG_PLAYERS_AGE_LIMIT;
        }

        public override Task InitializeAsync(object navigationData) {

            SelfInformationViewModel?.InitializeAsync(navigationData);
            PickAvatarPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.SETTINGS_TITLE;
            TabIcon = NavigationContext.SETTINGS_IMAGE_PATH;
            RelativeViewType = typeof(SettingsView);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated;
        }

        private void ResetPasswordsValidationObjects() {
            CurrentPassword = _validationObjectFactory.GetValidatableObject<string>();
            NewPassword = _validationObjectFactory.GetValidatableObject<string>();
            MinLengthRule<string> minLengthRule = new MinLengthRule<string> { MinLength = 6, IsCanBeEmpty = true };
            minLengthRule.ValidationMessage = string.Format(MinLengthRule<string>.MIN_LENGTH_ERROR_MESSAGE, minLengthRule.MinLength);
            NewPassword.Validations.Add(minLengthRule);

            RepeatNewPassword = _validationObjectFactory.GetValidatableObject<string>();
        }

        private void ResetValidationObjects() {
            EmaiAddress = _validationObjectFactory.GetValidatableObject<string>();
            EmaiAddress.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            EmaiAddress.Validations.Add(new EmailRule<string> { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });
            if (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Player &&
                (DateTime.Now.Year - GlobalSettings.Instance.UserProfile.DateOfBirth.Year) <= UserProfile.YOUNG_PLAYERS_AGE_LIMIT) {
                EmaiAddress.Validations.Clear();
            }

            PhoneNumber = _validationObjectFactory.GetValidatableObject<string>();

            ResetPasswordsValidationObjects();
        }

        private void OnProfileSettingsEventsProfileUpdated(object sender, ProfileUpdatedArgs e) => ResolveProfileInfo();
    }
}
