using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using System;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public class OrganizationRegistrationInputFormViewModel : CommonRegistrationInputFormViewModel {

        private ValidatableObject<string> _organizationName;
        public ValidatableObject<string> OrganizationName {
            get => _organizationName;
            set => SetProperty<ValidatableObject<string>>(ref _organizationName, value);
        }

        public override void Dispose() {
            base.Dispose();
        }

        public override bool ValidateForm() {
            bool isValidResult = base.ValidateForm();

            OrganizationName.Validate();

            isValidResult = isValidResult && OrganizationName.IsValid;

            return isValidResult;
        }

        public override RegistrationRequestDataModel BuildRegistrationDataModel() {
            RegistrationRequestDataModel registrationRequestDataModel = base.BuildRegistrationDataModel();
            registrationRequestDataModel.Type = ProfileType.Organization.ToString();
            registrationRequestDataModel.DisplayName = OrganizationName.Value;

            return registrationRequestDataModel;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            OrganizationName = _validationObjectFactory.GetValidatableObject<string>();
            OrganizationName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            DateOfBirth.Value = new DateTime((DateTime.Now - TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION)).Ticks);
            DateOfBirth.Validations.Add(new DateRule<DateTime> { ValidationMessage = _TO_YOUNG_ERROR_MESSAGE, DaysRestriction = TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION) });
        }
    }
}
