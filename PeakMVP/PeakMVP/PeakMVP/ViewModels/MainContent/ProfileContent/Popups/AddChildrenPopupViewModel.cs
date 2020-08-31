using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Services.Identity;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Profile;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.ProfileContent.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileContent.Popups;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileContent.Popups {
    public class AddChildrenPopupViewModel : PopupBaseViewModel, IInputForm {

        private static readonly int _DAYS_PER_YEAR = 366;

        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IIdentityService _identityService;
        private readonly IProfileService _profileService;
        private readonly IIdentityUtilService _identityUtilService;

        private CancellationTokenSource _addNewChildrenCancellationTokenSource = new CancellationTokenSource();

        public AddChildrenPopupViewModel(
            IValidationObjectFactory validationObjectFactory,
            IIdentityService identityService,
            IProfileService profileService,
            IIdentityUtilService identityUtilService) {

            _validationObjectFactory = validationObjectFactory;
            _identityService = identityService;
            _profileService = profileService;
            _identityUtilService = identityUtilService;

            ResetValidationObjects();
        }

        public ICommand CreateChildCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).SetBusy(busyKey, true);

            ResetCancellationTokenSource(ref _addNewChildrenCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _addNewChildrenCancellationTokenSource;

            if (ValidateForm()) {
                try {

                    RegistrationRequestDataModel registrationRequestDataModel = new RegistrationRequestDataModel() {
                        User = new UserDTO {
                            UserName = UserName.Value,
                            Password = Password.Value
                        },
                        Contact = new ContactDTO {
                            Email = (IsEmailAddressVisible) ? Email.Value : null,
                            Phone = PhonenNumber.Value
                        },
                        FirstName = FirstName.Value,
                        LastName = LastName.Value,
                        Address = new AddressDTO {
                            Zip = ZipCode.Value
                        },
                        Type = ProfileType.Player.ToString(),
                        DateOfBirth = DateOfBirth.Value,
                        ParentId = GlobalSettings.Instance.UserProfile.Id
                    };

                    RegistrationResponse registrationResponse = await _identityService.RegistrationAsync(registrationRequestDataModel, cancellationTokenSource);

                    if (registrationResponse != null) {
                        GetProfileResponse getProfileResponse = await _profileService.GetProfileAsync();

                        if (getProfileResponse != null) {
                            await _identityUtilService.ChargeUserProfileAsync(getProfileResponse, false);

                            ChildCreatedArgs childCreatedArgs = new ChildCreatedArgs();
                            await NavigationService.CurrentViewModelsNavigationStack.ForEachAsync(vMB => vMB.InitializeAsync(childCreatedArgs));
                        }

                        ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).IsPopupsVisible = false;
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");

                    await DialogService.ToastAsync(ex.Message);
                }
            }

            ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).SetBusy(busyKey, false);
        });

        public override Type RelativeViewType => typeof(ParentAddChildrenPopup);

        ValidatableObject<string> _firstName;
        public ValidatableObject<string> FirstName {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        ValidatableObject<string> _lastName;
        public ValidatableObject<string> LastName {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        ValidatableObject<string> _zipCode;
        public ValidatableObject<string> ZipCode {
            get => _zipCode;
            set => SetProperty(ref _zipCode, value);
        }

        ValidatableObject<string> _phonenNumber;
        public ValidatableObject<string> PhonenNumber {
            get => _phonenNumber;
            set => SetProperty(ref _phonenNumber, value);
        }

        private ValidatableObject<DateTime> _dateOfBirth;
        public ValidatableObject<DateTime> DateOfBirth {
            get => _dateOfBirth;
            set => SetProperty<ValidatableObject<DateTime>>(ref _dateOfBirth, value);
        }

        ValidatableObject<string> _email;
        public ValidatableObject<string> Email {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        ValidatableObject<string> _userName;
        public ValidatableObject<string> UserName {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        ValidatableObject<string> _password;
        public ValidatableObject<string> Password {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        bool _isEmailAddressVisible = false;
        public bool IsEmailAddressVisible {
            get { return _isEmailAddressVisible; }
            set {
                SetProperty(ref _isEmailAddressVisible, value);
                ResetEmailValidationObject();
            }
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _addNewChildrenCancellationTokenSource);
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            FirstName.Validate();
            LastName.Validate();
            ZipCode.Validate();
            DateOfBirth.Validate();
            UserName.Validate();
            Password.Validate();
            PhonenNumber.Validate();

            if (IsEmailAddressVisible) {
                Email.Validate();
            }

            isValidResult = FirstName.IsValid && LastName.IsValid && ZipCode.IsValid && DateOfBirth.IsValid && UserName.IsValid && Password.IsValid && Email.IsValid && PhonenNumber.IsValid;

            return isValidResult;
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _addNewChildrenCancellationTokenSource);
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (IsPopupVisible) {
                ResetInputForm();
            }

            Dispose();
        }

        private void ResetValidationObjects() {

            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            ZipCode = _validationObjectFactory.GetValidatableObject<string>();
            ZipCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            ZipCode.Validations.Add(new MinLengthRule<string> { ValidationMessage = string.Format(MinLengthRule<string>.MIN_LENGTH_ERROR_MESSAGE, MinLengthRule<string>.MIN_DEFAULT_VALUE) });

            PhonenNumber = _validationObjectFactory.GetValidatableObject<string>();

            ResetEmailValidationObject();

            if (DateOfBirth != null) {
                DateOfBirth.PropertyChanged -= OnDateOfBirthValuePropertyChanged;
            }

            DateOfBirth = _validationObjectFactory.GetValidatableObject<DateTime>();
            DateOfBirth.Validations.Add(new TomorrowDateLimitRule<DateTime>() { ValidationMessage = string.Format(TomorrowDateLimitRule<DateTime>.DATE_LIMIT_VALUE_ERROR_MESSAGE, DateTime.Now) });
            DateOfBirth.Value = DateTime.Now;
            DateOfBirth.PropertyChanged += OnDateOfBirthValuePropertyChanged;

            UserName = _validationObjectFactory.GetValidatableObject<string>();
            UserName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Password = _validationObjectFactory.GetValidatableObject<string>();
            Password.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }

        private void ResetEmailValidationObject() {
            Email = _validationObjectFactory.GetValidatableObject<string>();
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            Email.Validations.Add(new EmailRule<string> { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });
        }

        private void OnDateOfBirthValuePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(DateOfBirth.Value)) {
                IsEmailAddressVisible = ((DateTime.Now - DateOfBirth.Value).TotalDays / _DAYS_PER_YEAR) >= UserProfile.YOUNG_PLAYERS_AGE_LIMIT;
            }
        }
    }
}
