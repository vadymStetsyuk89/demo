using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.InitializeArguments.Teams;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Teams;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Teams {
    public class InviteExternalMembersViewModel : LoggedContentPageViewModel, IInputForm, IBuildFormModel {

        protected readonly IValidationObjectFactory _validationObjectFactory;

        private TeamDTO _targetTeam;

        public InviteExternalMembersViewModel(
            IValidationObjectFactory validationObjectFactory) {

            _validationObjectFactory = validationObjectFactory;

            ActionBarViewModel = ViewModelLocator.Resolve<ExternalInviteActionBarViewModel>();
            ActionBarViewModel.InitializeAsync(this);

            ResetInputForm();
        }

        public ICommand EntryCommand => new Command(() => {
            if (!string.IsNullOrEmpty(FirstName.Value) && !string.IsNullOrWhiteSpace(FirstName.Value)
            && !string.IsNullOrEmpty(LastName.Value) && !string.IsNullOrEmpty(LastName.Value)
            && !string.IsNullOrEmpty(Email.Value) && !string.IsNullOrEmpty(LastName.Value)) {
                ((ExternalInviteActionBarViewModel)ActionBarViewModel).ResolveExecutionAvailability(true);
            }
            else {
                ((ExternalInviteActionBarViewModel)ActionBarViewModel).ResolveExecutionAvailability(false);
            }
        });

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

        private ValidatableObject<string> _email;
        public ValidatableObject<string> Email {
            get => _email;
            set => SetProperty<ValidatableObject<string>>(ref _email, value);
        }

        public void ResetInputForm() => ResetValidationObjects();

        public bool ValidateForm() {
            bool isValidResult = false;

            FirstName.Validate();
            LastName.Validate();
            Email.Validate();

            isValidResult = FirstName.IsValid && LastName.IsValid && Email.IsValid;

            return isValidResult;
        }

        public object BuildFormModel() {
            ExternalMemberTeamIntive externalInvite = null;

            try {
                externalInvite = new ExternalMemberTeamIntive() {
                    Email = Email.Value,
                    InvitedUserDisplayName = string.Format("{0} {1}", FirstName.Value, LastName.Value),
                    TeamId = _targetTeam.Id
                };
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
            }

            return externalInvite;
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is InviteExternalMembersArgs inviteExternalMembersArgs) {
                ResetInputForm();
                _targetTeam = inviteExternalMembersArgs.TargetTeam;
            }

            return base.InitializeAsync(navigationData);
        }

        private void ResetValidationObjects() {
            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Email = _validationObjectFactory.GetValidatableObject<string>();
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            Email.Validations.Add(new EmailRule<string> { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });
        }
    }
}
