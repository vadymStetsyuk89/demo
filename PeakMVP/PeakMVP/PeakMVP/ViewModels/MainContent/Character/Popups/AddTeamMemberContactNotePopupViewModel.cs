using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.TeamMembers;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Character.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Character.Popups {
    public class AddTeamMemberContactNotePopupViewModel : PopupBaseViewModel, IInputForm {

        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly ITeamMemberService _teamMemberService;

        private TeamMember _targetTeamMember;

        private CancellationTokenSource _addContactNoteCancellationTokenSource = new CancellationTokenSource();

        public event EventHandler<TeamMemberContactInfo> TeamMemberContactCreated = delegate { };

        public AddTeamMemberContactNotePopupViewModel(
            IValidationObjectFactory validationObjectFactory,
            ITeamMemberService teamMemberService) {

            _validationObjectFactory = validationObjectFactory;
            _teamMemberService = teamMemberService;

            ResetInputForm();
        }

        public ICommand AddContactCommand => new Command(async () => {
            if (ValidateForm()) {
                Guid busyKey = Guid.NewGuid();
                UpdateBusyVisualState(busyKey, true);

                ResetCancellationTokenSource(ref _addContactNoteCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _addContactNoteCancellationTokenSource;

                try {
                    TeamMemberContactInfo teamMemberContactInfo = new TeamMemberContactInfo() {
                        City = City.Value,
                        Email = Email.Value,
                        FirstName = FirstName.Value,
                        GuardianType = GuardianTitle.Value,
                        LastName = LastName.Value,
                        Phones = new List<ContactPhoneNumber>() { new ContactPhoneNumber() {
                                Name = string.Format("{0} {1}", FirstName.Value, LastName.Value),
                                Phone = Phone.Value
                            }
                        },
                        State = State.Value,
                        Street = StreetName.Value,
                        ZipCode = Int32.Parse(Zip.Value)
                    };

                    if (await _teamMemberService.AddContactInfoAsync(_targetTeamMember.Id, teamMemberContactInfo, cancellationTokenSource)) {
                        GlobalSettings.Instance.AppMessagingEvents.TeamMemberEvents.AddedTeamMemberContactInfoInvoke(this, new EventArgs());
                    }
                    else {
                        throw new InvalidOperationException(TeamMemberService.CANT_ADD_CONTACT_INFO_COMMON_WARNING);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(TeamMemberService.CANT_ADD_CONTACT_INFO_COMMON_WARNING);
                }

                UpdateBusyVisualState(busyKey, false);

                ClosePopupCommand.Execute(null);
            }
        });

        public override Type RelativeViewType => typeof(AddTeamMemberContactNotePopupView);

        private ValidatableObject<string> _guardianTitle;
        public ValidatableObject<string> GuardianTitle {
            get => _guardianTitle;
            set => SetProperty<ValidatableObject<string>>(ref _guardianTitle, value);
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

        private ValidatableObject<string> _streetName;
        public ValidatableObject<string> StreetName {
            get => _streetName;
            set => SetProperty<ValidatableObject<string>>(ref _streetName, value);
        }

        private ValidatableObject<string> _city;
        public ValidatableObject<string> City {
            get => _city;
            set => SetProperty<ValidatableObject<string>>(ref _city, value);
        }

        private ValidatableObject<string> _state;
        public ValidatableObject<string> State {
            get => _state;
            set => SetProperty<ValidatableObject<string>>(ref _state, value);
        }

        private ValidatableObject<string> _zip;
        public ValidatableObject<string> Zip {
            get => _zip;
            set => SetProperty<ValidatableObject<string>>(ref _zip, value);
        }

        private ValidatableObject<string> _phone;
        public ValidatableObject<string> Phone {
            get => _phone;
            set => SetProperty<ValidatableObject<string>>(ref _phone, value);
        }

        private ValidatableObject<string> _email;
        public ValidatableObject<string> Email {
            get => _email;
            set => SetProperty<ValidatableObject<string>>(ref _email, value);
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _addContactNoteCancellationTokenSource);
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            isValidResult = GuardianTitle.Validate() && FirstName.Validate() && LastName.Validate() && StreetName.Validate() && City.Validate() && State.Validate() && Zip.Validate() && Phone.Validate() && Email.Validate();

            return isValidResult;
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _addContactNoteCancellationTokenSource);
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
            }
        }

        protected override void OnShowPopupCommand(object param) {
            base.OnShowPopupCommand(param);

            if (param is TeamMember teamMember) {
                _targetTeamMember = teamMember;
            }
            else {
                Debugger.Break();
                ClosePopupCommand.Execute(null);
            }
        }

        private void ResetValidationObjects() {
            GuardianTitle = _validationObjectFactory.GetValidatableObject<string>();
            GuardianTitle.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            FirstName = _validationObjectFactory.GetValidatableObject<string>();
            FirstName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LastName = _validationObjectFactory.GetValidatableObject<string>();
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            StreetName = _validationObjectFactory.GetValidatableObject<string>();
            StreetName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            City = _validationObjectFactory.GetValidatableObject<string>();
            City.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            State = _validationObjectFactory.GetValidatableObject<string>();
            State.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Zip = _validationObjectFactory.GetValidatableObject<string>();
            Zip.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Phone = _validationObjectFactory.GetValidatableObject<string>();
            Phone.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            Email = _validationObjectFactory.GetValidatableObject<string>();
            Email.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            Email.Validations.Add(new EmailRule<string>() { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });
        }
    }

    ///
    /// TODO: temporary class implementation
    /// 
    //public class TeamMemberContact {

    //    public string GuardianTitle { get; set; }

    //    public string FirstName { get; set; }

    //    public string LastName { get; set; }

    //    public string StreetName { get; set; }

    //    public string City { get; set; }

    //    public string State { get; set; }

    //    public string Zip { get; set; }

    //    public string Phone { get; set; }

    //    public string Email { get; set; }
    //}
}
