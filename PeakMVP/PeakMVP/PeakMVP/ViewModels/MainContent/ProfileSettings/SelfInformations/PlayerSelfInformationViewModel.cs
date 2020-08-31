using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations {
    public class PlayerSelfInformationViewModel : SelfInformationBase {

        public PlayerSelfInformationViewModel(
            IValidationObjectFactory validationObjectFactory,
            IProfileService profileService,
            IMediaPickerService mediaPickerService,
            IFileDTOBuilder fileDTOBuilder,
            IIdentityUtilService identityUtilService,
            IProfileMediaService profileMediaService)
            : base(
                  validationObjectFactory,
                  profileService,
                  mediaPickerService,
                  fileDTOBuilder,
                  identityUtilService,
                  profileMediaService) {

            ResetInputForm();
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

        public override SetProfileDataModel GetInputValues() {
            SetProfileDataModel setProfileDataModel = base.GetInputValues();
            setProfileDataModel.FirstName = FirstName.Value;
            setProfileDataModel.LastName = LastName.Value;

            return setProfileDataModel;
        }

        public override bool ValidateForm() {
            bool isValidResult = base.ValidateForm();

            FirstName.Validate();
            LastName.Validate();

            isValidResult = isValidResult && FirstName.IsValid && LastName.IsValid;

            return isValidResult;
        }

        public override void ResolveProfileInfo() {
            base.ResolveProfileInfo();

            FirstName.Value = GlobalSettings.Instance.UserProfile.FirstName;
            LastName.Value = GlobalSettings.Instance.UserProfile.LastName;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }
    }
}
