using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling;
using PeakMVP.Services.Scheduling;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Events.Popups;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events.EventsPopups {
    public class AddLocationPopupViewModel : PopupBaseViewModel, IInputForm {

        protected readonly IValidationObjectFactory _validationObjectFactory;
        protected readonly ISchedulingService _schedulingService;

        private CancellationTokenSource _createLocationCancellationTokenSource = new CancellationTokenSource();

        private TeamMember _targetTeamMember;

        public AddLocationPopupViewModel(
            IValidationObjectFactory validationObjectFactory,
            ISchedulingService schedulingService) {
            _validationObjectFactory = validationObjectFactory;
            _schedulingService = schedulingService;

            ResetValidationObjects();
        }

        public ICommand SaveLocationCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _createLocationCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _createLocationCancellationTokenSource;

                try {
                    LocationDTO createdLocation = await _schedulingService.CreateLocationAsync(new CreateLocationDataModel() {
                        Address = Address.Value,
                        Link = Link.Value,
                        Name = Name.Value,
                        TeamId = _targetTeamMember.Team.Id
                    }, cancellationTokenSource);

                    if (createdLocation != null) {
                        ClosePopupCommand.Execute(null);

                        GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeNewLocationCreated(this, createdLocation);
                    }
                    else {
                        throw new InvalidOperationException(SchedulingService.CREATE_OPPONENT_COMMON_ERROR_MESSAGE);
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

        public override Type RelativeViewType => typeof(AddLocationPopupView);

        private ValidatableObject<string> _name;
        public ValidatableObject<string> Name {
            get => _name;
            set => SetProperty<ValidatableObject<string>>(ref _name, value);
        }

        private ValidatableObject<string> _address;
        public ValidatableObject<string> Address {
            get => _address;
            set => SetProperty<ValidatableObject<string>>(ref _address, value);
        }

        private ValidatableObject<string> _link;
        public ValidatableObject<string> Link {
            get => _link;
            set => SetProperty<ValidatableObject<string>>(ref _link, value);
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            Name.Validate();
            Address.Validate();
            Link.Validate();

            isValidResult = Name.IsValid && Address.IsValid && Link.IsValid;

            return isValidResult;
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _createLocationCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is TeamMember teamMemberDTO) {
                _targetTeamMember = teamMemberDTO;
            }

            return base.InitializeAsync(navigationData);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _createLocationCancellationTokenSource);
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
            }
        }

        private void ResetValidationObjects() {
            Name = _validationObjectFactory.GetValidatableObject<string>();
            Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Address = _validationObjectFactory.GetValidatableObject<string>();
            Address.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Link = _validationObjectFactory.GetValidatableObject<string>();
            Link.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }
    }
}
