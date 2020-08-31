using PeakMVP.Factories.Validation;
using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling;
using PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class AssignmentViewModel : NestedViewModel, IInputForm {

        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly ITeamActionsManagmentDataItems _teamActionsManagmentDataItems;

        public AssignmentViewModel(
            IValidationObjectFactory validationObjectFactory,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems) {
            _validationObjectFactory = validationObjectFactory;
            _teamActionsManagmentDataItems = teamActionsManagmentDataItems;

            AssignmentStatuses = _teamActionsManagmentDataItems.BuildAssignmentStatusDataItems();

            ResetValidationObjects();
        }

        public ICommand RemoveAssignmentCommand => new Command(() => GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeActionAssignmentRemoved(this, this));

        public bool IsFormValid { get; private set; }

        private TeamMember _targetTeamMember;
        public TeamMember TargetTeamMember {
            get => _targetTeamMember;
            private set => SetProperty<TeamMember>(ref _targetTeamMember, value);
        }

        private ValidatableObject<string> _assignmentDescription;
        public ValidatableObject<string> AssignmentDescription {
            get => _assignmentDescription;
            private set => SetProperty<ValidatableObject<string>>(ref _assignmentDescription, value);
        }

        private ValidatableObject<AssignmentStatusDataItem> _selectedAssignmentStatus;
        public ValidatableObject<AssignmentStatusDataItem> SelectedAssignmentStatus {
            get => _selectedAssignmentStatus;
            private set => SetProperty<ValidatableObject<AssignmentStatusDataItem>>(ref _selectedAssignmentStatus, value);
        }

        private ValidatableObject<TeamMember> _selectedAssignedMember;
        public ValidatableObject<TeamMember> SelectedAssignedMember {
            get => _selectedAssignedMember;
            private set => SetProperty<ValidatableObject<TeamMember>>(ref _selectedAssignedMember, value);
        }

        private List<AssignmentStatusDataItem> _assignmentStatuses;
        public List<AssignmentStatusDataItem> AssignmentStatuses {
            get => _assignmentStatuses;
            private set => SetProperty<List<AssignmentStatusDataItem>>(ref _assignmentStatuses, value);
        }

        private List<TeamMember> _members;
        public List<TeamMember> Members {
            get => _members;
            private set => SetProperty<List<TeamMember>>(ref _members, value);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is TeamMember teamMemberDTO) {
                TargetTeamMember = teamMemberDTO;
                Members = teamMemberDTO.Team.Members.ToList();
            }
            else if (navigationData is AssignmentDTO assignment) {
                AssignmentDescription.Value = assignment.Description;
                SelectedAssignmentStatus.Value = AssignmentStatuses.FirstOrDefault<AssignmentStatusDataItem>(aSDI => aSDI.Status == assignment.Type);
                SelectedAssignedMember.Value = Members.FirstOrDefault<TeamMember>(tMDTO => tMDTO.Id == assignment.TeamMemberId);
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            if (SelectedAssignmentStatus != null) {
                SelectedAssignmentStatus.PropertyChanged -= OnSelectedAssignmentStatusPropertyChanged;
            }
        }

        public AssignmentDataModel BuildDataModel() {
            AssignmentDataModel assignmentDataModel = new AssignmentDataModel {
                Description = AssignmentDescription.Value.Trim(),
                Type = SelectedAssignmentStatus.Value.Status,
                TeamId = TargetTeamMember.Team.Id
            };

            if (SelectedAssignmentStatus.Value.Status == AssignmentStatusDataItem.ASSIGNED_STATUS_VALUE) {
                assignmentDataModel.TeamMemberId = SelectedAssignedMember.Value.Id;
            }

            return assignmentDataModel;
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            AssignmentDescription.Validate();
            SelectedAssignmentStatus.Validate();

            if (SelectedAssignmentStatus.Value != null && SelectedAssignmentStatus.Value.Status == AssignmentStatusDataItem.ASSIGNED_STATUS_VALUE) {
                SelectedAssignedMember.Validate();
            }

            isValidResult = AssignmentDescription.IsValid && SelectedAssignmentStatus.IsValid && SelectedAssignedMember.IsValid;
            IsFormValid = isValidResult;

            return isValidResult;
        }

        private void ResetValidationObjects() {
            AssignmentDescription = _validationObjectFactory.GetValidatableObject<string>();
            AssignmentDescription.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });


            if (SelectedAssignmentStatus != null) {
                SelectedAssignmentStatus.PropertyChanged -= OnSelectedAssignmentStatusPropertyChanged;
            }
            SelectedAssignmentStatus = _validationObjectFactory.GetValidatableObject<AssignmentStatusDataItem>();
            SelectedAssignmentStatus.Validations.Add(new IsNotNullOrEmptyRule<AssignmentStatusDataItem> { ValidationMessage = IsNotNullOrEmptyRule<AssignmentStatusDataItem>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            SelectedAssignmentStatus.PropertyChanged += OnSelectedAssignmentStatusPropertyChanged;

            SelectedAssignedMember = _validationObjectFactory.GetValidatableObject<TeamMember>();
            SelectedAssignedMember.Validations.Add(new IsNotNullOrEmptyRule<TeamMember> { ValidationMessage = IsNotNullOrEmptyRule<TeamMember>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }

        private void OnSelectedAssignmentStatusPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == ValidatableObject<AssignmentStatusDataItem>.VALUE_PROPERTY_NAME) {
                SelectedAssignedMember.Value = null;
            }
        }
    }
}
