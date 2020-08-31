using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.Validation;
using PeakMVP.Helpers.AppEvents.Events;
using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling;
using PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Services.Scheduling;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class CreateNewEventViewModel : EventManagingViewModelBase {

        private CancellationTokenSource _createEventCancellationTokenSource = new CancellationTokenSource();

        public CreateNewEventViewModel(
            IValidationObjectFactory validationObjectFactory,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems,
            ISchedulingService schedulingService)
            : base(
                 validationObjectFactory,
                 teamActionsManagmentDataItems,
                 schedulingService) {
            GameManagingHeader = CREATE_NEW_EVENT_HEADER;
            IsSaveAndCreateAnotherCommandAvalilable = true;

            SaveCommand = new Command(async () => {
                Guid busyKey = Guid.NewGuid();
                SetBusy(busyKey, true);

                ResetCancellationTokenSource(ref _createEventCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _createEventCancellationTokenSource;

                EventDTO eventAction = await CreateNewEventAsync(cancellationTokenSource);

                if (eventAction != null) {
                    await NavigationService.GoBackAsync();
                }

                SetBusy(busyKey, false);
            });
        }

        public ICommand SaveAndCreateAnotherCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            ResetCancellationTokenSource(ref _createEventCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _createEventCancellationTokenSource;

            EventDTO eventAction = await CreateNewEventAsync(cancellationTokenSource);

            if (eventAction != null) {
                ResetInputForm();
            }

            SetBusy(busyKey, false);
        });

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _createEventCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is TeamMember teamMemberDTO) {
                _targetTeamMember = teamMemberDTO;

                Locations = _targetTeamMember.Team.Locations.ToObservableCollection();
            }

            return base.InitializeAsync(navigationData);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _createEventCancellationTokenSource);
        }

        private Task<EventDTO> CreateNewEventAsync(CancellationTokenSource cancellationTokenSource) =>
            Task<EventDTO>.Run(async () => {
                EventDTO createdEvent = null;

                if (ValidateForm()) {
                    try {
                        createdEvent = await _schedulingService.CreateNewEventAsync(
                            new ManageEventDataModel() {
                                ShortLabel = ShortLabel.Value?.Trim(),
                                RepeatsUntil = RepeatUntil.Value,
                                RepeatingType = (SelectedRepeating.Value == null) ? ActionRepeatsDataItem.NOT_REPEAT_VALUE : SelectedRepeating.Value.Value,
                                Assignments = Assignments.Select<AssignmentViewModel, AssignmentDataModel>(aVM => aVM.BuildDataModel()).ToArray(),
                                DurationInMinutes = (string.IsNullOrEmpty(Duration.Value) || string.IsNullOrWhiteSpace(Duration.Value)) ? default(long) : long.Parse(Duration.Value.Trim()),
                                Name = NameOfEvent.Value?.Trim(),
                                IsTimeTbd = TimeTBD,
                                LocationDetails = LocationDetails.Value?.Trim(),
                                LocationId = SelectedLocation.Value.Id,
                                Notes = Notes.Value?.Trim(),
                                NotifyTeam = ToNotifyYourTeam,
                                StartDate = Date.Value + Time.Value,
                                TeamId = _targetTeamMember.Team.Id,
                            }, cancellationTokenSource);

                        if (createdEvent != null) {
                            Device.BeginInvokeOnMainThread(() => GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeEventCreated(this, new EventManagmentArgs() { Event = createdEvent, TeamMember = _targetTeamMember }));
                        }
                        else {
                            throw new InvalidOperationException(SchedulingService.CREATE_NEW_GAME_COMMON_ERROR_MESSAGE);
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (ObjectDisposedException) { }
                    catch (ServiceAuthenticationException) { }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);

                        Device.BeginInvokeOnMainThread(async () => await DialogService.ToastAsync(exc.Message));
                    }
                }

                return createdEvent;
            });
    }
}
