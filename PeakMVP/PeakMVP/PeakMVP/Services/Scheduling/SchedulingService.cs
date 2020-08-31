using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling;
using PeakMVP.Models.Rests.Requests.Scheduling;
using PeakMVP.Models.Rests.Responses.Scheduling;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Scheduling {
    public class SchedulingService : ISchedulingService {

        public static readonly string CREATE_OPPONENT_COMMON_ERROR_MESSAGE = "Can't create new opponent";
        public static readonly string CREATE_LOCATION_COMMON_ERROR_MESSAGE = "Can't create new location";
        public static readonly string CREATE_NEW_GAME_COMMON_ERROR_MESSAGE = "Can't create new game";
        public static readonly string DELETE_GAME_COMMON_ERROR_MESSAGE = "Can't delete game";
        public static readonly string UPDATE_GAME_COMMON_ERROR_MESSAGE = "Can't update game";
        public static readonly string DELETE_EVENT_COMMON_ERROR_MESSAGE = "Can't delete event";
        public static readonly string UPDATE_EVENT_COMMON_ERROR_MESSAGE = "Can't update gaeventme";
        public static readonly string CREATE_NEW_EVENT_COMMON_ERROR_MESSAGE = "Can't create new event";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        public SchedulingService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService) {
            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
        }

        public Task<OpponentDTO> CreateOpponentAsync(CreateOpponentDataModel createOpponentDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task<OpponentDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                OpponentDTO createdOpponent = null;

                CreateOpponentRequest createOpponentRequest = new CreateOpponentRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.ScheduleEndpoints.CreateNewOpponent,
                    Data = createOpponentDataModel
                };

                try {
                    CreateOpponentResponse createOpponentResponse = await _requestProvider.PostAsync<CreateOpponentRequest, CreateOpponentResponse>(createOpponentRequest);

                    if (createOpponentResponse != null) {
                        createdOpponent = BuildOpponent(createOpponentResponse);
                    }
                    else {
                        throw new InvalidOperationException(CREATE_OPPONENT_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return createdOpponent;

            }, cancellationTokenSource.Token);

        public Task<LocationDTO> CreateLocationAsync(CreateLocationDataModel createLocationDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task<LocationDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                LocationDTO createdLocation = null;

                CreateLocationRequest createLocationRequest = new CreateLocationRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = createLocationDataModel,
                    Url = GlobalSettings.Instance.Endpoints.ScheduleEndpoints.CreateNewLocation
                };

                try {
                    CreateLocationResponse createLocationResponse = await _requestProvider.PostAsync<CreateLocationRequest, CreateLocationResponse>(createLocationRequest);

                    if (createLocationResponse != null) {
                        createdLocation = BuildLocation(createLocationResponse);
                    }
                    else {
                        throw new InvalidOperationException(CREATE_LOCATION_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return createdLocation;
            }, cancellationTokenSource.Token);

        public Task<GameDTO> CreateNewGameAsync(ManageGameDataModel newGameDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task<GameDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GameDTO createdGame = null;

                CreateGameRequest createGameRequest = new CreateGameRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = newGameDataModel,
                    Url = GlobalSettings.Instance.Endpoints.ScheduleEndpoints.CreateNewGame
                };

                try {
                    CreateGameResponse createGameResponse = await _requestProvider.PostAsync<CreateGameRequest, CreateGameResponse>(createGameRequest);

                    if (createGameResponse != null) {
                        createdGame = BuildGame(createGameResponse);
                    }
                    else {
                        throw new InvalidOperationException(CREATE_NEW_GAME_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (HttpRequestExceptionEx exc) {
                    CreateUpdateGameBadResponse createGameBadResponse = JsonConvert.DeserializeObject<CreateUpdateGameBadResponse>(exc.Message);

                    string output = string.Format("{0} {1} {2} {3} {4} {5} {6}",
                        createGameBadResponse.Errors?.FirstOrDefault(),
                        createGameBadResponse.LocationId?.FirstOrDefault(),
                        createGameBadResponse.OpponentId?.FirstOrDefault(),
                        createGameBadResponse.TeamId?.FirstOrDefault(),
                        createGameBadResponse.Type?.FirstOrDefault(),
                        createGameBadResponse.DurationInMinutes?.FirstOrDefault(),
                        createGameBadResponse.MinutesToArriveEarly?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? CREATE_NEW_GAME_COMMON_ERROR_MESSAGE : output.Trim();

                    throw new InvalidOperationException(output);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return createdGame;
            }, cancellationTokenSource.Token);

        public Task DeleteGameAsync(GameDTO targetGame, long relativeTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                DeleteGameRequest deleteGameRequest = new DeleteGameRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ScheduleEndpoints.DeleteGame, targetGame.Id),
                    Data = new DeleteGameDataModel() {
                        GameId = targetGame.Id,
                        TeamId = relativeTeamId
                    }
                };

                try {
                    DeleteGameResponse deleteGameResponse = await _requestProvider.PostAsync<DeleteGameRequest, DeleteGameResponse>(deleteGameRequest);

                    if (deleteGameResponse == null) {
                        throw new InvalidOperationException(DELETE_GAME_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (HttpRequestExceptionEx exc) {
                    DeleteGameResponse deleteGameBadResponse = JsonConvert.DeserializeObject<DeleteGameResponse>(exc.Message);

                    string output = string.Format("{0}",
                        deleteGameBadResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? DELETE_GAME_COMMON_ERROR_MESSAGE : output.Trim();

                    throw new InvalidOperationException(output);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }
            }, cancellationTokenSource.Token);

        public Task<GameDTO> UpdateGameAsync(ManageGameDataModel updateGameDataModel, long gameId, CancellationTokenSource cancellationTokenSource) =>
            Task<GameDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GameDTO updatedGame = null;

                UpdateGameRequest updateGameRequest = new UpdateGameRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = updateGameDataModel,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ScheduleEndpoints.UpdateGame, gameId)
                };

                try {
                    UpdateGameResponse updateGameResponse = await _requestProvider.PostAsync<UpdateGameRequest, UpdateGameResponse>(updateGameRequest);

                    if (updateGameResponse != null) {
                        updatedGame = BuildGame(updateGameResponse);
                    }
                    else {
                        throw new InvalidOperationException(UPDATE_GAME_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (HttpRequestExceptionEx exc) {
                    CreateUpdateGameBadResponse createGameBadResponse = JsonConvert.DeserializeObject<CreateUpdateGameBadResponse>(exc.Message);

                    string output = string.Format("{0} {1} {2} {3} {4} {5} {6}",
                        createGameBadResponse.Errors?.FirstOrDefault(),
                        createGameBadResponse.LocationId?.FirstOrDefault(),
                        createGameBadResponse.OpponentId?.FirstOrDefault(),
                        createGameBadResponse.TeamId?.FirstOrDefault(),
                        createGameBadResponse.Type?.FirstOrDefault(),
                        createGameBadResponse.DurationInMinutes?.FirstOrDefault(),
                        createGameBadResponse.MinutesToArriveEarly?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? UPDATE_GAME_COMMON_ERROR_MESSAGE : output.Trim();

                    throw new InvalidOperationException(output);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return updatedGame;
            }, cancellationTokenSource.Token);

        public Task DeleteEventAsync(EventDTO targetEvent, long relativeTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                DeleteEventRequest deleteEventRequest = new DeleteEventRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ScheduleEndpoints.DeleteEvent, targetEvent.Id),
                    Data = new DeleteEventDataModel() {
                        EventId = targetEvent.Id,
                        TeamId = relativeTeamId
                    }
                };

                try {
                    DeleteEventResponse deleteEventResponse = await _requestProvider.PostAsync<DeleteEventRequest, DeleteEventResponse>(deleteEventRequest);

                    if (deleteEventResponse == null) {
                        throw new InvalidOperationException(DELETE_EVENT_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (HttpRequestExceptionEx exc) {
                    DeleteEventResponse deleteGameBadResponse = JsonConvert.DeserializeObject<DeleteEventResponse>(exc.Message);

                    string output = string.Format("{0}",
                        deleteGameBadResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? DELETE_EVENT_COMMON_ERROR_MESSAGE : output.Trim();

                    throw new InvalidOperationException(output);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }
            }, cancellationTokenSource.Token);

        public Task<EventDTO> CreateNewEventAsync(ManageEventDataModel newEventDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task<EventDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                EventDTO createdEvent = null;

                CreateEventRequest createEventRequest = new CreateEventRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = newEventDataModel,
                    Url = GlobalSettings.Instance.Endpoints.ScheduleEndpoints.CreateNewEvent
                };

                try {
                    CreateEventResponse createEventResponse = await _requestProvider.PostAsync<CreateEventRequest, CreateEventResponse>(createEventRequest);

                    if (createEventResponse != null) {
                        createdEvent = BuildEvent(createEventResponse);
                    }
                    else {
                        throw new InvalidOperationException(CREATE_NEW_EVENT_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (HttpRequestExceptionEx exc) {
                    CreateEventResponse createGameBadResponse = JsonConvert.DeserializeObject<CreateEventResponse>(exc.Message);

                    string output = string.Format("{0}",
                        createGameBadResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? CREATE_NEW_EVENT_COMMON_ERROR_MESSAGE : output.Trim();

                    throw new InvalidOperationException(output);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return createdEvent;
            }, cancellationTokenSource.Token);

        public Task<EventDTO> UpdateEventAsync(ManageEventDataModel updateEventDataModel, long eventId, CancellationTokenSource cancellationTokenSource) =>
            Task<GameDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                EventDTO updatedEvent = null;

                UpdateEventRequest updateEventRequest = new UpdateEventRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = updateEventDataModel,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ScheduleEndpoints.UpdateEvent, eventId)
                };

                try {
                    UpdateEventResponse updateEventResponse = await _requestProvider.PostAsync<UpdateEventRequest, UpdateEventResponse>(updateEventRequest);

                    if (updateEventResponse != null) {
                        updatedEvent = BuildEvent(updateEventResponse);
                    }
                    else {
                        throw new InvalidOperationException(UPDATE_EVENT_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (HttpRequestExceptionEx exc) {
                    UpdateEventResponse createGameBadResponse = JsonConvert.DeserializeObject<UpdateEventResponse>(exc.Message);

                    string output = string.Format("{0}",
                        createGameBadResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? UPDATE_EVENT_COMMON_ERROR_MESSAGE : output.Trim();

                    throw new InvalidOperationException(output);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return updatedEvent;
            }, cancellationTokenSource.Token);

        private EventDTO BuildEvent(UpdateEventResponse data) {
            try {
                return new EventDTO() {
                    Assignments = data.Assignments,
                    TeamId = data.TeamId,
                    DurationInMinutes = data.DurationInMinutes,
                    Id = data.Id,
                    IsCanceled = data.IsCanceled,
                    IsTimeTbd = data.IsTimeTbd,
                    Location = data.Location,
                    LocationDetails = data.LocationDetails,
                    Notes = data.Notes,
                    StartDate = data.StartDate,
                    Name = data.Name,
                    RepeatingType = data.RepeatingType,
                    RepeatsUntil = data.RepeatsUntil,
                    ShortLabel = data.ShortLabel
                };
            }
            catch (Exception exc) {
                throw new InvalidOperationException("SchedulingService BuildEvent()", exc);
            }
        }

        private EventDTO BuildEvent(CreateEventResponse data) {
            try {
                return new EventDTO() {
                    Assignments = data.Assignments,
                    TeamId = data.TeamId,
                    DurationInMinutes = data.DurationInMinutes,
                    Id = data.Id,
                    IsCanceled = data.IsCanceled,
                    IsTimeTbd = data.IsTimeTbd,
                    Location = data.Location,
                    LocationDetails = data.LocationDetails,
                    Notes = data.Notes,
                    StartDate = data.StartDate,
                    Name = data.Name,
                    RepeatingType = data.RepeatingType,
                    RepeatsUntil = data.RepeatsUntil,
                    ShortLabel = data.ShortLabel
                };
            }
            catch (Exception exc) {
                throw new InvalidOperationException("SchedulingService BuildEvent()", exc);
            }
        }

        private GameDTO BuildGame(CreateGameResponse data) {
            try {
                return new GameDTO() {
                    Assignments = data.Assignments,
                    TeamId = data.TeamId,
                    DurationInMinutes = data.DurationInMinutes,
                    Id = data.Id,
                    IsCanceled = data.IsCanceled,
                    IsTimeTbd = data.IsTimeTbd,
                    Location = data.Location,
                    LocationDetails = data.LocationDetails,
                    MinutesToArriveEarly = data.MinutesToArriveEarly,
                    Notes = data.Notes,
                    NotForStandings = data.NotForStandings,
                    Opponent = data.Opponent,
                    StartDate = data.StartDate,
                    Type = data.Type,
                    Uniform = data.Uniform
                };
            }
            catch (Exception exc) {
                throw new InvalidOperationException("SchedulingService BuildGame()", exc);
            }
        }

        private GameDTO BuildGame(UpdateGameResponse data) {
            try {
                return new GameDTO() {
                    Assignments = data.Assignments,
                    TeamId = data.TeamId,
                    DurationInMinutes = data.DurationInMinutes,
                    Id = data.Id,
                    IsCanceled = data.IsCanceled,
                    IsTimeTbd = data.IsTimeTbd,
                    Location = data.Location,
                    LocationDetails = data.LocationDetails,
                    MinutesToArriveEarly = data.MinutesToArriveEarly,
                    Notes = data.Notes,
                    NotForStandings = data.NotForStandings,
                    Opponent = data.Opponent,
                    StartDate = data.StartDate,
                    Type = data.Type,
                    Uniform = data.Uniform
                };
            }
            catch (Exception exc) {
                throw new InvalidOperationException("SchedulingService BuildGame()", exc);
            }
        }

        private OpponentDTO BuildOpponent(CreateOpponentResponse data) {
            try {
                return new OpponentDTO() {
                    ContactName = data.ContactName,
                    Email = data.Email,
                    Id = data.Id,
                    Name = data.Name,
                    Notes = data.Notes,
                    Phone = data.Phone,
                    TeamId = data.TeamId
                };
            }
            catch (Exception exc) {
                throw new InvalidOperationException("SchedulingService BuildOpponent()", exc);
            }
        }

        private LocationDTO BuildLocation(CreateLocationResponse data) {
            try {
                return new LocationDTO() {
                    Address = data.Address,
                    Id = data.Id,
                    Link = data.Link,
                    Name = data.Name,
                    Notes = data.Notes,
                    TeamId = data.TeamId
                };
            }
            catch (Exception exc) {
                throw new InvalidOperationException("SchedulingService BuildLocation()", exc);
            }
        }
    }
}
