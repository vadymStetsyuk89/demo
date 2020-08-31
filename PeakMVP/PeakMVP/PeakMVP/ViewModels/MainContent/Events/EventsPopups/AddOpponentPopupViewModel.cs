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
    public class AddOpponentPopupViewModel : PopupBaseViewModel, IInputForm {

        protected readonly IValidationObjectFactory _validationObjectFactory;
        protected readonly ISchedulingService _schedulingService;

        private CancellationTokenSource _createOpponentCancellationTokenSource = new CancellationTokenSource();

        private TeamMember _targetTeamMember;

        public AddOpponentPopupViewModel(
            IValidationObjectFactory validationObjectFactory,
            ISchedulingService schedulingService) {
            _validationObjectFactory = validationObjectFactory;
            _schedulingService = schedulingService;

            ResetValidationObjects();
        }

        public ICommand SaveOpponentCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _createOpponentCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _createOpponentCancellationTokenSource;

                try {
                    OpponentDTO createdOpponent = await _schedulingService.CreateOpponentAsync(new CreateOpponentDataModel() {
                        ContactName = ContactName.Value,
                        Email = Email.Value,
                        Name = Name.Value,
                        Phone = Phone.Value,
                        TeamId = _targetTeamMember.Team.Id
                    }, cancellationTokenSource);

                    if (createdOpponent != null) {
                        ClosePopupCommand.Execute(null);

                        GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeNewOpponentCreated(this, createdOpponent);
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

        public override Type RelativeViewType => typeof(AddOpponentPopupView);

        private ValidatableObject<string> _name;
        public ValidatableObject<string> Name {
            get => _name;
            set => SetProperty<ValidatableObject<string>>(ref _name, value);
        }

        private ValidatableObject<string> _contactName;
        public ValidatableObject<string> ContactName {
            get => _contactName;
            set => SetProperty<ValidatableObject<string>>(ref _contactName, value);
        }

        private ValidatableObject<string> _email;
        public ValidatableObject<string> Email {
            get => _email;
            set => SetProperty<ValidatableObject<string>>(ref _email, value);
        }

        private ValidatableObject<string> _phone;
        public ValidatableObject<string> Phone {
            get => _phone;
            set => SetProperty<ValidatableObject<string>>(ref _phone, value);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is TeamMember teamMemberDTO) {
                _targetTeamMember = teamMemberDTO;
            }

            return base.InitializeAsync(navigationData);
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            Name.Validate();
            ContactName.Validate();
            Email.Validate();
            Phone.Validate();

            isValidResult = Name.IsValid && ContactName.IsValid && Email.IsValid && Phone.IsValid;

            return isValidResult;
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _createOpponentCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _createOpponentCancellationTokenSource);
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

            ContactName = _validationObjectFactory.GetValidatableObject<string>();
            ContactName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Email = _validationObjectFactory.GetValidatableObject<string>();
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            Email.Validations.Add(new EmailRule<string> { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });

            Phone = _validationObjectFactory.GetValidatableObject<string>();
        }
    }
}
