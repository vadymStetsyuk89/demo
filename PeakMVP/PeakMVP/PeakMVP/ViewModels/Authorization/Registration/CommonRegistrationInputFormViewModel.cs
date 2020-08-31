using PeakMVP.Factories.Validation;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using System;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public abstract class CommonRegistrationInputFormViewModel : ViewModelBase, IRegistrationInputForm<RegistrationRequestDataModel> {

        protected readonly IValidationObjectFactory _validationObjectFactory;

        protected static readonly int _ADULT_AGE_RESTRICTION = 14;
        protected static readonly int _DAYS_PER_YEAR = 366;

        protected static readonly string _LOGIN_DETAILS_COMMON_TITLE = "Login Details";
        protected static readonly string _TO_YOUNG_ERROR_MESSAGE = "Too young";
        protected static readonly string _DATE_LIMIT_VALUE_ERROR_MESSAGE = "Date limit value is {0:dd MMM yy}";

        public CommonRegistrationInputFormViewModel() {
            _validationObjectFactory = ViewModelLocator.Resolve<IValidationObjectFactory>();

            ResetValidationObjects();
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

        private ValidatableObject<string> _phoneNumber;
        public ValidatableObject<string> PhoneNumber {
            get => _phoneNumber;
            set => SetProperty<ValidatableObject<string>>(ref _phoneNumber, value);
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

        private string _loginDetailsTitle = _LOGIN_DETAILS_COMMON_TITLE;
        public string LoginDetailsTitle {
            get => _loginDetailsTitle;
            protected set => SetProperty<string>(ref _loginDetailsTitle, value);
        }

        public override void Dispose() {
            base.Dispose();
        }

        public virtual RegistrationRequestDataModel BuildRegistrationDataModel() {
            RegistrationRequestDataModel registrationRequestDataModel = new RegistrationRequestDataModel();

            registrationRequestDataModel.User.UserName = UserName.Value;
            registrationRequestDataModel.User.Password = Password.Value;

            registrationRequestDataModel.FirstName = FirstName.Value;
            registrationRequestDataModel.LastName = LastName.Value;
            registrationRequestDataModel.Contact.Email = Email.Value;
            registrationRequestDataModel.Contact.Phone = PhoneNumber.Value;

            registrationRequestDataModel.Address.Zip = ZipCode.Value;

            registrationRequestDataModel.DateOfBirth = DateOfBirth.Value;

            return registrationRequestDataModel;
        }

        public virtual bool ValidateForm() {
            bool isValidResult = false;

            FirstName.Validate();
            LastName.Validate();
            ZipCode.Validate();
            Email.Validate();
            UserName.Validate();
            Password.Validate();
            DateOfBirth.Validate();
            PhoneNumber.Validate();

            isValidResult = FirstName.IsValid
                && LastName.IsValid
                && ZipCode.IsValid
                && Email.IsValid
                && UserName.IsValid
                && Password.IsValid
                && DateOfBirth.IsValid
                && PhoneNumber.IsValid;

            return isValidResult;
        }

        public virtual void ResetInputForm() {
            ResetValidationObjects();
        }

        protected virtual void ResetValidationObjects() {
            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            ZipCode = _validationObjectFactory.GetValidatableObject<string>();
            ZipCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            ZipCode.Validations.Add(new MinLengthRule<string> { ValidationMessage = string.Format(MinLengthRule<string>.MIN_LENGTH_ERROR_MESSAGE, MinLengthRule<string>.MIN_DEFAULT_VALUE) });

            PhoneNumber = _validationObjectFactory.GetValidatableObject<string>();

            Email = _validationObjectFactory.GetValidatableObject<string>();
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            Email.Validations.Add(new EmailRule<string> { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });

            UserName = _validationObjectFactory.GetValidatableObject<string>();
            UserName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Password = _validationObjectFactory.GetValidatableObject<string>();
            Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            DateOfBirth = _validationObjectFactory.GetValidatableObject<DateTime>();
            DateOfBirth.Validations.Add(new TomorrowDateLimitRule<DateTime>() { ValidationMessage = string.Format(_DATE_LIMIT_VALUE_ERROR_MESSAGE, DateTime.Now) });
            DateOfBirth.Value = DateTime.Now;

            //#if DEBUG
            //            Email.Value = "@mailinator.com";
            //#endif
        }
    }
}
