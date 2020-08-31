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
    public class EditGameViewModel : GameManagingViewModelBase {

        private GameDTO _targetGame;
        private CancellationTokenSource _updateGameCancellationTokenSource = new CancellationTokenSource();

        public EditGameViewModel(
            IValidationObjectFactory validationObjectFactory,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems,
            ISchedulingService schedulingService)
            : base(
                 validationObjectFactory,
                 teamActionsManagmentDataItems,
                 schedulingService) {
            GameManagingHeader = EDIT_GAME_HEADER;

            SaveCommand = new Command(async () => {
                Guid busyKey = Guid.NewGuid();
                SetBusy(busyKey, true);

                ResetCancellationTokenSource(ref _updateGameCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _updateGameCancellationTokenSource;

                GameDTO createdGame = await UpdateGameAsync(cancellationTokenSource);

                if (createdGame != null) {
                    await NavigationService.GoBackAsync();
                }

                SetBusy(busyKey, false);
            });
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is TeamMember teamMemberDTO) {
                _targetTeamMember = teamMemberDTO;

                Opponents = _targetTeamMember.Team.Opponents.ToObservableCollection();
                Locations = _targetTeamMember.Team.Locations.ToObservableCollection();
            }
            else if (navigationData is GameDTO game) {
                InitializeGameDTO(game);
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _updateGameCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _updateGameCancellationTokenSource);
        }

        private async void InitializeGameDTO(GameDTO game) {
            _targetGame = game;

            await game.Assignments.ForEachAsync(async (assignment) => {
                AssignmentViewModel assignmentViewModel = ViewModelLocator.Resolve<AssignmentViewModel>();
                await assignmentViewModel.InitializeAsync(_targetTeamMember);
                await assignmentViewModel.InitializeAsync(assignment);

                Assignments.Add(assignmentViewModel);
            });

            Duration.Value = game.DurationInMinutes.ToString();
            IsGameCanceled = game.IsCanceled;
            TimeTBD = game.IsTimeTbd;
            LocationDetails.Value = game.LocationDetails;
            SelectedLocation.Value = Locations.FirstOrDefault(lDTO => lDTO.Id == game.Location.Id);
            Arrival.Value = game.MinutesToArriveEarly.ToString();
            Notes.Value = game.Notes;
            IsTowardStandings = game.NotForStandings;
            SelectedOpponent.Value = Opponents.FirstOrDefault(oDTO => oDTO.Id == game.Opponent.Id);
            Date.Value = game.StartDate;
            Time.Value = game.StartDate.TimeOfDay;
            SelectedActionVenue.Value = ActionVenues.FirstOrDefault(aVDI => aVDI.ActionVenue == game.Type);
            UniformDescription.Value = game.Uniform;
        }

        private Task<GameDTO> UpdateGameAsync(CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                GameDTO updatedGame = null;

                if (ValidateForm()) {
                    try {
                        updatedGame = await _schedulingService.UpdateGameAsync(
                            new ManageGameDataModel() {
                                Assignments = Assignments.Select(aVM => aVM.BuildDataModel()).ToArray(),
                                DurationInMinutes = (string.IsNullOrEmpty(Duration.Value) || string.IsNullOrWhiteSpace(Duration.Value)) ? default(long) : long.Parse(Duration.Value.Trim()),
                                IsCanceled = IsGameCanceled,
                                IsTimeTbd = TimeTBD,
                                LocationDetails = LocationDetails.Value?.Trim(),
                                LocationId = SelectedLocation.Value.Id,
                                MinutesToArriveEarly = (string.IsNullOrEmpty(Arrival.Value) || string.IsNullOrWhiteSpace(Arrival.Value)) ? default(long) : long.Parse(Arrival.Value.Trim()),
                                Notes = Notes.Value?.Trim(),
                                NotForStandings = IsTowardStandings,
                                NotifyTeam = ToNotifyYourTeam,
                                OpponentId = SelectedOpponent.Value.Id,
                                StartDate = Date.Value + Time.Value,
                                TeamId = _targetTeamMember.Team.Id,
                                Type = (SelectedActionVenue.Value == null) ? ActionVenueDataItem.UNKNOWN_VENUE_VALUE : SelectedActionVenue.Value?.ActionVenue,
                                Uniform = UniformDescription.Value?.Trim()
                            }, _targetGame.Id, cancellationTokenSource);

                        if (updatedGame != null) {
                            Device.BeginInvokeOnMainThread(() => GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeGameUpdated(this, new GameManagmentArgs() { Game = updatedGame, TeamMember = _targetTeamMember }));
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

                return updatedGame;
            });
    }
}
