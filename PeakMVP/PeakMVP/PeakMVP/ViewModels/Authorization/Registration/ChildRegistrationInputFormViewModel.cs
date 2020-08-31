using PeakMVP.Factories.Validation;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using System;
using System.ComponentModel;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public class ChildRegistrationInputFormViewModel : ViewModelBase, IRegistrationInputForm<RegistrationRequestDataModel> {

        protected static readonly string _CHILD_LOGIN_DETAILS_COMMON_TITLE = "Child Login Details";
        private static readonly string _VALIDATABLE_OBJECT_VALUE_PROPERTY_PATH = "Value";
        private static readonly string _DATE_LIMIT_VALUE_ERROR_MESSAGE = "Date limit value is {0:dd MMM yy}";
        private static readonly int _DAYS_PER_YEAR = 366;

        private IValidationObjectFactory _validationObjectFactory;

        public ChildRegistrationInputFormViewModel() {
            _validationObjectFactory = ViewModelLocator.Resolve<IValidationObjectFactory>();

            ResetValidationObjects();
        }

        private string _loginDetailsTitle = _CHILD_LOGIN_DETAILS_COMMON_TITLE;
        public string LoginDetailsTitle {
            get => _loginDetailsTitle;
            protected set => SetProperty<string>(ref _loginDetailsTitle, value);
        }

        private string _childFullName;
        public string ChildFullName {
            get => _childFullName;
            private set => SetProperty<string>(ref _childFullName, value);
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

        private ValidatableObject<string> _zipCode;
        public ValidatableObject<string> ZipCode {
            get => _zipCode;
            set => SetProperty<ValidatableObject<string>>(ref _zipCode, value);
        }

        private ValidatableObject<string> _email;
        public ValidatableObject<string> Email {
            get => _email;
            set => SetProperty<ValidatableObject<string>>(ref _email, value);
        }

        private ValidatableObject<DateTime> _dateOfBirth;
        public ValidatableObject<DateTime> DateOfBirth {
            get => _dateOfBirth;
            set => SetProperty<ValidatableObject<DateTime>>(ref _dateOfBirth, value);
        }

        private ValidatableObject<string> _userName;
        public ValidatableObject<string> UserName {
            get => _userName;
            set => SetProperty<ValidatableObject<string>>(ref _userName, value);
        }

        private ValidatableObject<string> _password;
        public ValidatableObject<string> Password {
            get => _password;
            set => SetProperty<ValidatableObject<string>>(ref _password, value);
        }

        private bool _isEmailInputEnabled;
        public bool IsEmailInputEnabled {
            get => _isEmailInputEnabled;
            private set {
                SetProperty<bool>(ref _isEmailInputEnabled, value);
                ResetEmailValidationObject();
            }
        }

        public bool IsFormValid { get; private set; }

        public override void Dispose() {
            base.Dispose();

            if (FirstName != null) {
                FirstName.PropertyChanged -= OnFirstLastNameValuePropertyChanged;
            }

            if (LastName != null) {
                LastName.PropertyChanged -= OnFirstLastNameValuePropertyChanged;
            }

            if (DateOfBirth != null) {
                DateOfBirth.PropertyChanged -= OnDateOfBirthValuePropertyChanged;
            }
        }

        public bool ValidateForm() {
            FirstName.Validate();
            LastName.Validate();
            ZipCode.Validate();
            DateOfBirth.Validate();
            UserName.Validate();
            Password.Validate();

            if (IsEmailInputEnabled) {
                Email.Validate();
            }

            bool validationResult = (FirstName.IsValid && LastName.IsValid && ZipCode.IsValid && DateOfBirth.IsValid && Email.IsValid && UserName.IsValid && Password.IsValid);
            IsFormValid = validationResult;

            return validationResult;
        }

        public void ResetInputForm() =>
            ResetValidationObjects();

        public RegistrationRequestDataModel BuildRegistrationDataModel() =>
            new RegistrationRequestDataModel() {
                FirstName = FirstName.Value,
                LastName = LastName.Value,
                Address = new AddressDTO() { Zip = ZipCode.Value },
                Contact = new ContactDTO() { Email = Email.Value },
                DateOfBirth = DateOfBirth.Value,
                User = new UserDTO() {
                    UserName = UserName.Value,
                    Password = Password.Value
                },
                Type = ProfileType.Player.ToString()
            };

        private void ResetValidationObjects() {
            if (FirstName != null) {
                FirstName.PropertyChanged -= OnFirstLastNameValuePropertyChanged;
            }

            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            FirstName.PropertyChanged += OnFirstLastNameValuePropertyChanged;

            if (LastName != null) {
                LastName.PropertyChanged -= OnFirstLastNameValuePropertyChanged;
            }

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            LastName.PropertyChanged += OnFirstLastNameValuePropertyChanged;

            ZipCode = _validationObjectFactory.GetValidatableObject<string>();
            ZipCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            ZipCode.Validations.Add(new MinLengthRule<string> { ValidationMessage = string.Format(MinLengthRule<string>.MIN_LENGTH_ERROR_MESSAGE, MinLengthRule<string>.MIN_DEFAULT_VALUE) });

            ResetEmailValidationObject();

            if (DateOfBirth != null) {
                DateOfBirth.PropertyChanged -= OnDateOfBirthValuePropertyChanged;
            }

            DateOfBirth = _validationObjectFactory.GetValidatableObject<DateTime>();
            DateOfBirth.Validations.Add(new TomorrowDateLimitRule<DateTime>() { ValidationMessage = string.Format(_DATE_LIMIT_VALUE_ERROR_MESSAGE, DateTime.Now) });
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
            if (e.PropertyName == _VALIDATABLE_OBJECT_VALUE_PROPERTY_PATH) {
                IsEmailInputEnabled = ((DateTime.Now - DateOfBirth.Value).TotalDays / _DAYS_PER_YEAR) >= UserProfile.YOUNG_PLAYERS_AGE_LIMIT;
            }
        }

        private void OnFirstLastNameValuePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == _VALIDATABLE_OBJECT_VALUE_PROPERTY_PATH) {
                ChildFullName = string.Format("{0} {1}", FirstName.Value, LastName.Value);
            }
        }
    }
}
