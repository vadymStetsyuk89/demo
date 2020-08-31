using PeakMVP.Factories.Validation;
using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Services.Scheduling;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.Events.EventsPopups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Events {
    public abstract class EventManagingViewModelBase : LoggedContentPageViewModel, IInputForm {

        protected static readonly string CREATE_NEW_EVENT_HEADER = "New event";
        protected static readonly string EDIT_EVENT_HEADER = "Edit event";
        //protected static readonly string _DATE_LIMIT_VALUE_ERROR_MESSAGE = "Date limit value is {0:dd MMM yy}";

        protected readonly ISchedulingService _schedulingService;
        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly ITeamActionsManagmentDataItems _teamActionsManagmentDataItems;

        protected TeamMember _targetTeamMember;

        public EventManagingViewModelBase(
            IValidationObjectFactory validationObjectFactory,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems,
            ISchedulingService schedulingService) {
            _validationObjectFactory = validationObjectFactory;
            _teamActionsManagmentDataItems = teamActionsManagmentDataItems;
            _schedulingService = schedulingService;

            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

            ActionBarViewModel = ViewModelLocator.Resolve<ModeActionBarViewModel>();
            ActionBarViewModel.InitializeAsync(this);
            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = false;

            AddLocationPopupViewModel = ViewModelLocator.Resolve<AddLocationPopupViewModel>();
            AddLocationPopupViewModel.InitializeAsync(this);

            Repeatings = _teamActionsManagmentDataItems.BuildRepeatingsDataItems();
            ResetValidationObjects();
        }

        public ICommand AddAssignmentCommand => new Command(async () => {
            AssignmentViewModel assignmentViewModel = ViewModelLocator.Resolve<AssignmentViewModel>();
            await assignmentViewModel.InitializeAsync(_targetTeamMember);

            Assignments.Add(assignmentViewModel);
        });

        public ICommand SaveCommand { get; protected set; }

        public ICommand CancelCommand => new Command(async () => {
            Dispose();
            await NavigationService.GoBackAsync();
        });

        private AddLocationPopupViewModel _addLocationPopupViewModel;
        public AddLocationPopupViewModel AddLocationPopupViewModel {
            get => _addLocationPopupViewModel;
            private set {
                _addLocationPopupViewModel?.Dispose();
                SetProperty<AddLocationPopupViewModel>(ref _addLocationPopupViewModel, value);
            }
        }

        private string _gameManagingHeader;
        public string GameManagingHeader {
            get => _gameManagingHeader;
            protected set => SetProperty<string>(ref _gameManagingHeader, value);
        }

        private bool _isSaveAndCreateAnotherCommandAvalilable;
        public bool IsSaveAndCreateAnotherCommandAvalilable {
            get => _isSaveAndCreateAnotherCommandAvalilable;
            protected set => SetProperty<bool>(ref _isSaveAndCreateAnotherCommandAvalilable, value);
        }

        private ValidatableObject<string> _nameOfEvent;
        public ValidatableObject<string> NameOfEvent {
            get => _nameOfEvent;
            private set => SetProperty<ValidatableObject<string>>(ref _nameOfEvent, value);
        }

        private ValidatableObject<string> _shortLabel;
        public ValidatableObject<string> ShortLabel {
            get => _shortLabel;
            private set => SetProperty<ValidatableObject<string>>(ref _shortLabel, value);
        }

        private ValidatableObject<DateTime> _date;
        public ValidatableObject<DateTime> Date {
            get => _date;
            private set => SetProperty<ValidatableObject<DateTime>>(ref _date, value);
        }

        private bool _timeTBD;
        public bool TimeTBD {
            get => _timeTBD;
            set => SetProperty<bool>(ref _timeTBD, value);
        }

        private ValidatableObject<TimeSpan> _time;
        public ValidatableObject<TimeSpan> Time {
            get => _time;
            private set => SetProperty<ValidatableObject<TimeSpan>>(ref _time, value);
        }

        private ValidatableObject<string> _duration;
        public ValidatableObject<string> Duration {
            get => _duration;
            private set => SetProperty<ValidatableObject<string>>(ref _duration, value);
        }

        private ValidatableObject<ActionRepeatsDataItem> _selectedRepeating;
        public ValidatableObject<ActionRepeatsDataItem> SelectedRepeating {
            get => _selectedRepeating;
            private set => SetProperty<ValidatableObject<ActionRepeatsDataItem>>(ref _selectedRepeating, value);
        }

        private ValidatableObject<DateTime> _repeatUntil;
        public ValidatableObject<DateTime> RepeatUntil {
            get => _repeatUntil;
            private set => SetProperty<ValidatableObject<DateTime>>(ref _repeatUntil, value);
        }

        private bool _isRepeatUntilAvailable;
        public bool IsRepeatUntilAvailable {
            get => _isRepeatUntilAvailable;
            set => SetProperty<bool>(ref _isRepeatUntilAvailable, value);
        }

        private ValidatableObject<string> _locationDetails;
        public ValidatableObject<string> LocationDetails {
            get => _locationDetails;
            private set => SetProperty<ValidatableObject<string>>(ref _locationDetails, value);
        }

        private ValidatableObject<LocationDTO> _selectedLocation;
        public ValidatableObject<LocationDTO> SelectedLocation {
            get => _selectedLocation;
            private set => SetProperty<ValidatableObject<LocationDTO>>(ref _selectedLocation, value);
        }

        private ValidatableObject<string> _notes;
        public ValidatableObject<string> Notes {
            get => _notes;
            private set => SetProperty<ValidatableObject<string>>(ref _notes, value);
        }

        private bool _isCanceled;
        public bool IsCanceled {
            get => _isCanceled;
            set => SetProperty<bool>(ref _isCanceled, value);
        }

        private bool _toNotifyYourTeam;
        public bool ToNotifyYourTeam {
            get => _toNotifyYourTeam;
            set => SetProperty<bool>(ref _toNotifyYourTeam, value);
        }

        private ObservableCollection<AssignmentViewModel> _assignments = new ObservableCollection<AssignmentViewModel>();
        public ObservableCollection<AssignmentViewModel> Assignments {
            get => _assignments;
            private set {
                _assignments?.ForEach(aVM => aVM.Dispose());
                SetProperty<ObservableCollection<AssignmentViewModel>>(ref _assignments, value);
            }
        }

        private ObservableCollection<LocationDTO> _locations = new ObservableCollection<LocationDTO>();
        public ObservableCollection<LocationDTO> Locations {
            get => _locations;
            protected set => SetProperty<ObservableCollection<LocationDTO>>(ref _locations, value);
        }

        private List<ActionRepeatsDataItem> _repeatings;
        public List<ActionRepeatsDataItem> Repeatings {
            get => _repeatings;
            private set => SetProperty<List<ActionRepeatsDataItem>>(ref _repeatings, value);
        }

        public override void Dispose() {
            base.Dispose();

            if (SelectedRepeating != null) {
                SelectedRepeating.PropertyChanged -= OnSelectedRepeatingPropertyChanged;
            }

            AddLocationPopupViewModel?.Dispose();
            _assignments?.ForEach(aVM => aVM.Dispose());
        }

        public void ResetInputForm() {
            TimeTBD = false;
            IsCanceled = false;
            ToNotifyYourTeam = false;
            IsRepeatUntilAvailable = false;

            Assignments?.ForEach<AssignmentViewModel>(aVM => aVM.ResetInputForm());

            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            NameOfEvent.Validate();
            ShortLabel.Validate();
            Date.Validate();
            Time.Validate();
            Duration.Validate();
            LocationDetails.Validate();
            SelectedLocation.Validate();
            SelectedRepeating.Validate();
            RepeatUntil.Validate();
            Notes.Validate();

            Assignments.ForEach(aVM => aVM.ValidateForm());
            bool assignmentsValidity = !Assignments.Any(aVM => !aVM.IsFormValid);

            isValidResult = NameOfEvent.IsValid && ShortLabel.IsValid && Date.IsValid
                && Time.IsValid && Duration.IsValid && LocationDetails.IsValid
                && SelectedLocation.IsValid && SelectedRepeating.IsValid && RepeatUntil.IsValid
                && Notes.IsValid && assignmentsValidity;

            return isValidResult;
        }

        public override Task InitializeAsync(object navigationData) {
            AddLocationPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.ActionAssignmentRemoved += OnScheduleEventsActionAssignmentRemoved;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewLocationCreated += OnScheduleEventsNewLocationCreated;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            if (SelectedRepeating != null) {
                SelectedRepeating.PropertyChanged -= OnSelectedRepeatingPropertyChanged;
            }

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.ActionAssignmentRemoved -= OnScheduleEventsActionAssignmentRemoved;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewLocationCreated -= OnScheduleEventsNewLocationCreated;
        }

        private void OnScheduleEventsActionAssignmentRemoved(object sender, AssignmentViewModel e) =>
            Assignments?.Remove(e);

        private void ResetValidationObjects() {
            NameOfEvent = _validationObjectFactory.GetValidatableObject<string>();
            NameOfEvent.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            ShortLabel = _validationObjectFactory.GetValidatableObject<string>();

            Date = _validationObjectFactory.GetValidatableObject<DateTime>();
            ///
            /// TODO: define and use 'backing date rule'
            /// 
            Date.Value = DateTime.Now;

            Time = _validationObjectFactory.GetValidatableObject<TimeSpan>();
            Time.Value = DateTime.Now.TimeOfDay;

            Duration = _validationObjectFactory.GetValidatableObject<string>();
            Duration.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            ///
            /// TODO: define and use 'backing time rule'
            /// 

            LocationDetails = _validationObjectFactory.GetValidatableObject<string>();

            SelectedLocation = _validationObjectFactory.GetValidatableObject<LocationDTO>();
            SelectedLocation.Validations.Add(new IsNotNullOrEmptyRule<LocationDTO> { ValidationMessage = IsNotNullOrEmptyRule<LocationDTO>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            if (SelectedRepeating != null) {
                SelectedRepeating.PropertyChanged -= OnSelectedRepeatingPropertyChanged;
            }
            SelectedRepeating = _validationObjectFactory.GetValidatableObject<ActionRepeatsDataItem>();
            SelectedRepeating.PropertyChanged += OnSelectedRepeatingPropertyChanged;

            RepeatUntil = _validationObjectFactory.GetValidatableObject<DateTime>();
            RepeatUntil.Value = DateTime.Now;

            Notes = _validationObjectFactory.GetValidatableObject<string>();
        }

        private void OnScheduleEventsNewLocationCreated(object sender, LocationDTO newLocation) {
            if (newLocation.TeamId == _targetTeamMember.Team.Id && !(_targetTeamMember.Team.Locations.Any<LocationDTO>(lDTO => lDTO.Id == newLocation.Id))) {
                _targetTeamMember.Team.Locations.Add(newLocation);
                Locations.Add(newLocation);
            }
        }

        private void OnSelectedRepeatingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == ValidatableObject<ActionRepeatsDataItem>.VALUE_PROPERTY_NAME) {
                if (((ValidatableObject<ActionRepeatsDataItem>)sender).Value.Value == ActionRepeatsDataItem.NOT_REPEAT_VALUE) {
                    IsRepeatUntilAvailable = false;
                    RepeatUntil.Value = DateTime.Now;
                }
                else {
                    IsRepeatUntilAvailable = true;
                }
            }
        }
    }
}
