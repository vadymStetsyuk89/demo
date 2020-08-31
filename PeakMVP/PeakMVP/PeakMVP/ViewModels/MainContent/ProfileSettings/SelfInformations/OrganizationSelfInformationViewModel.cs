using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations {
    public class OrganizationSelfInformationViewModel : SelfInformationBase {

        public OrganizationSelfInformationViewModel(
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

        private ValidatableObject<string> _organizationName;
        public ValidatableObject<string> OrganizationName {
            get => _organizationName;
            set => SetProperty<ValidatableObject<string>>(ref _organizationName, value);
        }

        public override SetProfileDataModel GetInputValues() {
            SetProfileDataModel setProfileDataModel = base.GetInputValues();
            setProfileDataModel.DisplayName = OrganizationName.Value;

            ///
            /// Necessary for api, so use the same values...
            /// 
            setProfileDataModel.FirstName = GlobalSettings.Instance.UserProfile.FirstName;
            setProfileDataModel.LastName = GlobalSettings.Instance.UserProfile.LastName;

            return setProfileDataModel;
        }

        public override bool ValidateForm() {
            bool isValidResult = base.ValidateForm();

            OrganizationName.Validate();

            isValidResult = isValidResult && OrganizationName.IsValid;

            return isValidResult;
        }

        public override void ResolveProfileInfo() {
            base.ResolveProfileInfo();

            OrganizationName.Value = GlobalSettings.Instance.UserProfile.DisplayName;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            OrganizationName = _validationObjectFactory.GetValidatableObject<string>();
            OrganizationName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }
    }
}
