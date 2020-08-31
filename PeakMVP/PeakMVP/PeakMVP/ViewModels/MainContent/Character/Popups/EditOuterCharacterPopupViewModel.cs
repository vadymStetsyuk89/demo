using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.InitializeArguments.Profile;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Services.Profile;
using PeakMVP.Validations;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Character.Popups;
using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Character.Popups {
    public class EditOuterCharacterPopupViewModel : PopupBaseViewModel, IInputForm {

        private CancellationTokenSource _editOuterProfileCancellationTokenSource = new CancellationTokenSource();

        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IProfileService _profileService;

        private long _targetProfileId;
        private long _targetTeamId;

        public EditOuterCharacterPopupViewModel(
            IValidationObjectFactory validationObjectFactory,
            IProfileService profileService) {

            _validationObjectFactory = validationObjectFactory;
            _profileService = profileService;

            ResetInputForm();
        }

        public ICommand SaveSettingsCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _editOuterProfileCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _editOuterProfileCancellationTokenSource;

                try {
                    bool completion = await _profileService.UpdateOuterProfileInfoAsync(new OuterProfileEditDataModel() {
                        About = AboutYou.Value,
                        Sports = MySports.Value,
                        TeamId = _targetTeamId,
                        TeamMemberProfileId = _targetProfileId
                    }, cancellationTokenSource);

                    if (completion) {
                        ClosePopupCommand.Execute(null);

                        GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.OuterProfileInfoUpdatedInvoke(this);
                    }
                    else {
                        throw new InvalidOperationException(ProfileService.CANT_UPDATE_OUTER_PROFILE_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public override Type RelativeViewType => typeof(EditOuterCharacterPopupView);

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

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _editOuterProfileCancellationTokenSource);
        }

        public void ResetInputForm() {
            ResetValidationObjects();

            _targetProfileId = 0;
            _targetTeamId = 0;
        }

        public bool ValidateForm() => true;

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
            }
        }

        protected override void OnShowPopupCommand(object param) {
            base.OnShowPopupCommand(param);

            if (param is EditOuterProfileArgs editOuterProfileArgs) {
                AboutYou.Value = editOuterProfileArgs.TargetProfile.About;
                MySports.Value = editOuterProfileArgs.TargetProfile.MySports;
                _targetProfileId = editOuterProfileArgs.TargetProfile.Id;
                _targetTeamId = editOuterProfileArgs.RelatedTeamId;
            }
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _editOuterProfileCancellationTokenSource);
        }

        private void ResetValidationObjects() {
            AboutYou = _validationObjectFactory.GetValidatableObject<string>();
            MySports = _validationObjectFactory.GetValidatableObject<string>();
        }
    }
}
