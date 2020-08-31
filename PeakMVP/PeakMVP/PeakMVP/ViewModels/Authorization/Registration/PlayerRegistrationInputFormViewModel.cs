using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using System;
using System.Linq;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public class PlayerRegistrationInputFormViewModel : CommonRegistrationInputFormViewModel {

        private static readonly string _VALIDATABLE_OBJECT_VALUE_PROPERTY_PATH = "Value";

        public override void Dispose() {
            base.Dispose();

            if (DateOfBirth != null) {
                DateOfBirth.PropertyChanged -= OnDateOfBirthValuePropertyChanged;
            }
        }

        public override RegistrationRequestDataModel BuildRegistrationDataModel() {
            RegistrationRequestDataModel registrationRequestDataModel = base.BuildRegistrationDataModel();
            registrationRequestDataModel.Type = ProfileType.Player.ToString();

            return registrationRequestDataModel;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            if (DateOfBirth != null) {
                DateOfBirth.PropertyChanged -= OnDateOfBirthValuePropertyChanged;
            }

            DateOfBirth.Value = new DateTime((DateTime.Now - TimeSpan.FromDays(_DAYS_PER_YEAR * UserProfile.YOUNG_PLAYERS_AGE_LIMIT)).Ticks);
            DateOfBirth.PropertyChanged += OnDateOfBirthValuePropertyChanged;
        }

        private void OnDateOfBirthValuePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == _VALIDATABLE_OBJECT_VALUE_PROPERTY_PATH) {
                if (((DateTime.Now - DateOfBirth.Value).TotalDays / _DAYS_PER_YEAR) <= UserProfile.YOUNG_PLAYERS_AGE_LIMIT) {
                    NavigationService.CurrentViewModelsNavigationStack.LastOrDefault().InitializeAsync(new ProfileTypeItem() { ProfileType = ProfileType.Parent });
                }
            }
        }
    }
}
