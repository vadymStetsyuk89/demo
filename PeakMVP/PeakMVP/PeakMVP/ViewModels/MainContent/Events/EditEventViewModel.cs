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
using PeakMVP.ViewModels.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class EditEventViewModel : EventManagingViewModelBase {

        private EventDTO _targetEvent;

        private CancellationTokenSource _updateEventCancellationTokenSource = new CancellationTokenSource();

        public EditEventViewModel(
            IValidationObjectFactory validationObjectFactory,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems,
            ISchedulingService schedulingService)
            : base(
                 validationObjectFactory,
                 teamActionsManagmentDataItems,
                 schedulingService) {
            GameManagingHeader = EDIT_EVENT_HEADER;

            SaveCommand = new Command(async () => {
                Guid busyKey = Guid.NewGuid();
                SetBusy(busyKey, true);

                ResetCancellationTokenSource(ref _updateEventCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _updateEventCancellationTokenSource;

                EventDTO createdEvent = await UpdateEventAsync(cancellationTokenSource);

                if (createdEvent != null) {
                    await NavigationService.GoBackAsync();
                }

                SetBusy(busyKey, false);
            });
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is TeamMember teamMemberDTO) {
                _targetTeamMember = teamMemberDTO;

                Locations = _targetTeamMember.Team.Locations.ToObservableCollection();
            }
            else if (navigationData is EventDTO eventAction) {
                InitializeEventDTO(eventAction);
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _updateEventCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _updateEventCancellationTokenSource);
        }

        private async void InitializeEventDTO(EventDTO eventAction) {
            _targetEvent = eventAction;

            await eventAction.Assignments.ForEachAsync<AssignmentDTO>(async (assignment) => {
                AssignmentViewModel assignmentViewModel = ViewModelLocator.Resolve<AssignmentViewModel>();
                await assignmentViewModel.InitializeAsync(_targetTeamMember);
                await assignmentViewModel.InitializeAsync(assignment);

                Assignments.Add(assignmentViewModel);
            });

            ShortLabel.Value = eventAction.ShortLabel;
            RepeatUntil.Value = eventAction.RepeatsUntil;
            SelectedRepeating.Value = Repeatings.FirstOrDefault(r => r.Value == eventAction.RepeatingType);
            Duration.Value = eventAction.DurationInMinutes.ToString();
            NameOfEvent.Value = eventAction.Name;
            TimeTBD = eventAction.IsTimeTbd;
            LocationDetails.Value = eventAction.LocationDetails;
            SelectedLocation.Value = Locations.FirstOrDefault<LocationDTO>(lDTO => lDTO.Id == eventAction.Location.Id);
            Notes.Value = eventAction.Notes;
            Date.Value = eventAction.StartDate;
        }

        private Task<EventDTO> UpdateEventAsync(CancellationTokenSource cancellationTokenSource) =>
            Task<EventDTO>.Run(async () => {
                EventDTO eventDTO = null;

                if (ValidateForm()) {
                    try {
                        eventDTO = await _schedulingService.UpdateEventAsync(
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
                            }, _targetEvent.Id, cancellationTokenSource);

                        if (eventDTO != null) {
                            Device.BeginInvokeOnMainThread(() => GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeEventUpdated(this, new EventManagmentArgs() { Event = eventDTO, TeamMember = _targetTeamMember }));
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

                return eventDTO;
            });
    }

}
