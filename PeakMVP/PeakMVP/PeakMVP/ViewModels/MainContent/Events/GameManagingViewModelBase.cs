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
    public abstract class GameManagingViewModelBase : LoggedContentPageViewModel, IInputForm {

        protected static readonly string CREATE_NEW_GAME_HEADER = "New game";
        protected static readonly string EDIT_GAME_HEADER = "Edit game";
        //protected static readonly string _DATE_LIMIT_VALUE_ERROR_MESSAGE = "Date limit value is {0:dd MMM yy}";

        protected readonly ISchedulingService _schedulingService;
        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly ITeamActionsManagmentDataItems _teamActionsManagmentDataItems;

        protected TeamMember _targetTeamMember;

        public GameManagingViewModelBase(
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

            AddOpponentPopupViewModel = ViewModelLocator.Resolve<AddOpponentPopupViewModel>();
            AddOpponentPopupViewModel.InitializeAsync(this);

            AddLocationPopupViewModel = ViewModelLocator.Resolve<AddLocationPopupViewModel>();
            AddLocationPopupViewModel.InitializeAsync(this);

            ActionVenues = _teamActionsManagmentDataItems.BuildActionVenueDataItems();
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

        private AddOpponentPopupViewModel _addOpponentPopupViewModel;
        public AddOpponentPopupViewModel AddOpponentPopupViewModel {
            get => _addOpponentPopupViewModel;
            private set {
                _addOpponentPopupViewModel?.Dispose();
                SetProperty<AddOpponentPopupViewModel>(ref _addOpponentPopupViewModel, value);
            }
        }

        private AddLocationPopupViewModel _addLocationPopupViewModel;
        public AddLocationPopupViewModel AddLocationPopupViewModel {
            get => _addLocationPopupViewModel;
            private set {
                _addLocationPopupViewModel?.Dispose();
                SetProperty<AddLocationPopupViewModel>(ref _addLocationPopupViewModel, value);
            }
        }

        private ValidatableObject<DateTime> _date;
        public ValidatableObject<DateTime> Date {
            get => _date;
            private set => SetProperty<ValidatableObject<DateTime>>(ref _date, value);
        }

        private ValidatableObject<TimeSpan> _time;
        public ValidatableObject<TimeSpan> Time {
            get => _time;
            private set => SetProperty<ValidatableObject<TimeSpan>>(ref _time, value);
        }

        private bool _timeTBD;
        public bool TimeTBD {
            get => _timeTBD;
            set => SetProperty<bool>(ref _timeTBD, value);
        }

        private ValidatableObject<string> _duration;
        public ValidatableObject<string> Duration {
            get => _duration;
            private set => SetProperty<ValidatableObject<string>>(ref _duration, value);
        }

        private ValidatableObject<OpponentDTO> _selectedOpponent;
        public ValidatableObject<OpponentDTO> SelectedOpponent {
            get => _selectedOpponent;
            private set => SetProperty<ValidatableObject<OpponentDTO>>(ref _selectedOpponent, value);
        }

        private ValidatableObject<LocationDTO> _selectedLocation;
        public ValidatableObject<LocationDTO> SelectedLocation {
            get => _selectedLocation;
            private set => SetProperty<ValidatableObject<LocationDTO>>(ref _selectedLocation, value);
        }

        private ValidatableObject<string> _locationDetails;
        public ValidatableObject<string> LocationDetails {
            get => _locationDetails;
            private set => SetProperty<ValidatableObject<string>>(ref _locationDetails, value);
        }

        private ValidatableObject<string> _arrival;
        public ValidatableObject<string> Arrival {
            get => _arrival;
            private set => SetProperty<ValidatableObject<string>>(ref _arrival, value);
        }

        private ValidatableObject<ActionVenueDataItem> _selectedActionVenue;
        public ValidatableObject<ActionVenueDataItem> SelectedActionVenue {
            get => _selectedActionVenue;
            private set => SetProperty<ValidatableObject<ActionVenueDataItem>>(ref _selectedActionVenue, value);
        }

        private ValidatableObject<string> _uniformDescription;
        public ValidatableObject<string> UniformDescription {
            get => _uniformDescription;
            private set => SetProperty<ValidatableObject<string>>(ref _uniformDescription, value);
        }

        private ValidatableObject<string> _notes;
        public ValidatableObject<string> Notes {
            get => _notes;
            private set => SetProperty<ValidatableObject<string>>(ref _notes, value);
        }

        private bool _isGameCanceled;
        public bool IsGameCanceled {
            get => _isGameCanceled;
            set => SetProperty<bool>(ref _isGameCanceled, value);
        }

        private bool _isTowardStandings;
        public bool IsTowardStandings {
            get => _isTowardStandings;
            set => SetProperty<bool>(ref _isTowardStandings, value);
        }

        private ObservableCollection<AssignmentViewModel> _assignments = new ObservableCollection<AssignmentViewModel>();
        public ObservableCollection<AssignmentViewModel> Assignments {
            get => _assignments;
            private set {
                _assignments?.ForEach(aVM => aVM.Dispose());
                SetProperty<ObservableCollection<AssignmentViewModel>>(ref _assignments, value);
            }
        }

        private bool _toNotifyYourTeam;
        public bool ToNotifyYourTeam {
            get => _toNotifyYourTeam;
            set => SetProperty<bool>(ref _toNotifyYourTeam, value);
        }

        private ObservableCollection<OpponentDTO> _opponents = new ObservableCollection<OpponentDTO>();
        public ObservableCollection<OpponentDTO> Opponents {
            get => _opponents;
            protected set => SetProperty<ObservableCollection<OpponentDTO>>(ref _opponents, value);
        }

        private ObservableCollection<LocationDTO> _locations = new ObservableCollection<LocationDTO>();
        public ObservableCollection<LocationDTO> Locations {
            get => _locations;
            protected set => SetProperty<ObservableCollection<LocationDTO>>(ref _locations, value);
        }

        private List<ActionVenueDataItem> _actionVenues;
        public List<ActionVenueDataItem> ActionVenues {
            get => _actionVenues;
            private set => SetProperty<List<ActionVenueDataItem>>(ref _actionVenues, value);
        }

        public override void Dispose() {
            base.Dispose();

            AddOpponentPopupViewModel?.Dispose();
            AddLocationPopupViewModel?.Dispose();
            _assignments?.ForEach(aVM => aVM.Dispose());
        }

        public void ResetInputForm() {
            TimeTBD = false;
            IsTowardStandings = false;
            IsGameCanceled = false;

            Assignments?.ForEach<AssignmentViewModel>(aVM => aVM.ResetInputForm());

            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            Date.Validate();
            Time.Validate();
            Duration.Validate();
            SelectedOpponent.Validate();
            SelectedLocation.Validate();
            LocationDetails.Validate();
            SelectedActionVenue.Validate();
            UniformDescription.Validate();
            Arrival.Validate();
            Notes.Validate();

            Assignments.ForEach(aVM => aVM.ValidateForm());
            bool assignmentsValidity = !Assignments.Any(aVM => !aVM.IsFormValid);

            isValidResult = Date.IsValid && Time.IsValid
                && Duration.IsValid && SelectedOpponent.IsValid
                && SelectedLocation.IsValid && LocationDetails.IsValid
                && SelectedActionVenue.IsValid && UniformDescription.IsValid
                && Arrival.IsValid && Notes.IsValid
                && assignmentsValidity;

            return isValidResult;
        }

        public override Task InitializeAsync(object navigationData) {
            AddOpponentPopupViewModel?.InitializeAsync(navigationData);
            AddLocationPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.ActionAssignmentRemoved += OnScheduleEventsActionAssignmentRemoved;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewOpponentCreated += OnScheduleEventsNewOpponentCreated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewLocationCreated += OnScheduleEventsNewLocationCreated;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.ActionAssignmentRemoved -= OnScheduleEventsActionAssignmentRemoved;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewOpponentCreated -= OnScheduleEventsNewOpponentCreated;
            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.NewLocationCreated -= OnScheduleEventsNewLocationCreated;
        }

        private void OnScheduleEventsActionAssignmentRemoved(object sender, AssignmentViewModel e) =>
            Assignments?.Remove(e);

        private void ResetValidationObjects() {
            Date = _validationObjectFactory.GetValidatableObject<DateTime>();
            ///
            /// TODO: define and use 'backing date rule'
            /// 
            Date.Value = DateTime.Now;

            Time = _validationObjectFactory.GetValidatableObject<TimeSpan>();
            Time.Value = DateTime.Now.TimeOfDay;

            Duration = _validationObjectFactory.GetValidatableObject<string>();
            ///
            /// TODO: define and use 'backing time rule'
            /// 

            SelectedOpponent = _validationObjectFactory.GetValidatableObject<OpponentDTO>();
            SelectedOpponent.Validations.Add(new IsNotNullOrEmptyRule<OpponentDTO> { ValidationMessage = IsNotNullOrEmptyRule<OpponentDTO>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            SelectedLocation = _validationObjectFactory.GetValidatableObject<LocationDTO>();
            SelectedLocation.Validations.Add(new IsNotNullOrEmptyRule<LocationDTO> { ValidationMessage = IsNotNullOrEmptyRule<LocationDTO>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            LocationDetails = _validationObjectFactory.GetValidatableObject<string>();

            SelectedActionVenue = _validationObjectFactory.GetValidatableObject<ActionVenueDataItem>();

            UniformDescription = _validationObjectFactory.GetValidatableObject<string>();

            Arrival = _validationObjectFactory.GetValidatableObject<string>();

            Notes = _validationObjectFactory.GetValidatableObject<string>();
        }

        private void OnScheduleEventsNewOpponentCreated(object sender, OpponentDTO newOpponent) {
            if (newOpponent.TeamId == _targetTeamMember.Team.Id && !(_targetTeamMember.Team.Opponents.Any<OpponentDTO>(oDTO => oDTO.Id == newOpponent.Id))) {
                _targetTeamMember.Team.Opponents.Add(newOpponent);
                Opponents.Add(newOpponent);
            }
        }

        private void OnScheduleEventsNewLocationCreated(object sender, LocationDTO newLocation) {
            if (newLocation.TeamId == _targetTeamMember.Team.Id && !(_targetTeamMember.Team.Locations.Any<LocationDTO>(lDTO => lDTO.Id == newLocation.Id))) {
                _targetTeamMember.Team.Locations.Add(newLocation);
                Locations.Add(newLocation);
            }
        }
    }
}
