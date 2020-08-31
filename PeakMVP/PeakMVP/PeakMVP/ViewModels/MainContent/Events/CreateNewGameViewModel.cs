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
    public class CreateNewGameViewModel : GameManagingViewModelBase {

        private CancellationTokenSource _createGameCancellationTokenSource = new CancellationTokenSource();

        public CreateNewGameViewModel(
            IValidationObjectFactory validationObjectFactory,
            ITeamActionsManagmentDataItems teamActionsManagmentDataItems,
            ISchedulingService schedulingService)
            : base(
                 validationObjectFactory,
                 teamActionsManagmentDataItems,
                 schedulingService) {
            GameManagingHeader = CREATE_NEW_GAME_HEADER;
            IsSaveAndCreateAnotherCommandAvalilable = true;

            SaveCommand = new Command(async () => {
                Guid busyKey = Guid.NewGuid();
                SetBusy(busyKey, true);

                ResetCancellationTokenSource(ref _createGameCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _createGameCancellationTokenSource;

                GameDTO createdGame = await CreateNewGameAsync(cancellationTokenSource);

                if (createdGame != null) {
                    await NavigationService.GoBackAsync();
                }

                SetBusy(busyKey, false);
            });
        }

        public ICommand SaveAndCreateAnotherCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            ResetCancellationTokenSource(ref _createGameCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _createGameCancellationTokenSource;

            GameDTO createdGame = await CreateNewGameAsync(cancellationTokenSource);

            if (createdGame != null) {
                ResetInputForm();
            }

            SetBusy(busyKey, false);
        });

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is TeamMember teamMemberDTO) {
                _targetTeamMember = teamMemberDTO;

                Opponents = _targetTeamMember.Team.Opponents.ToObservableCollection();
                Locations = _targetTeamMember.Team.Locations.ToObservableCollection();
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _createGameCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _createGameCancellationTokenSource);
        }

        private Task<GameDTO> CreateNewGameAsync(CancellationTokenSource cancellationTokenSource) =>
            Task<GameDTO>.Run(async () => {
                GameDTO createdGame = null;

                if (ValidateForm()) {
                    try {
                        createdGame = await _schedulingService.CreateNewGameAsync(
                            new ManageGameDataModel() {
                                Assignments = Assignments.Select<AssignmentViewModel, AssignmentDataModel>(aVM => aVM.BuildDataModel()).ToArray(),
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
                            }, cancellationTokenSource);

                        if (createdGame != null) {
                            Device.BeginInvokeOnMainThread(() => GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeNewGameCreated(this, new GameManagmentArgs() { Game = createdGame, TeamMember = _targetTeamMember }));
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

                return createdGame;
            });
    }
}
