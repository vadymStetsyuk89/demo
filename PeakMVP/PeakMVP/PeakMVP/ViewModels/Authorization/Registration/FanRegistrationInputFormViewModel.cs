using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Validations.ValidationRules;
using System;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public class FanRegistrationInputFormViewModel : CommonRegistrationInputFormViewModel {

        public override void Dispose() {
            base.Dispose();
        }

        public override RegistrationRequestDataModel BuildRegistrationDataModel() {
            RegistrationRequestDataModel registrationRequestDataModel = base.BuildRegistrationDataModel();
            registrationRequestDataModel.Type = ProfileType.Fan.ToString();

            return registrationRequestDataModel;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            DateOfBirth.Value = new DateTime((DateTime.Now - TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION)).Ticks);
            DateOfBirth.Validations.Add(new DateRule<DateTime> { ValidationMessage = _TO_YOUNG_ERROR_MESSAGE, DaysRestriction = TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION) });
        }
    }
}
