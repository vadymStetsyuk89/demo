using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Scheduling {
    public interface ISchedulingService {

        Task<OpponentDTO> CreateOpponentAsync(CreateOpponentDataModel createOpponentDataModel, CancellationTokenSource cancellationTokenSource);

        Task<LocationDTO> CreateLocationAsync(CreateLocationDataModel createLocationDataModel, CancellationTokenSource cancellationTokenSource);

        Task<GameDTO> CreateNewGameAsync(ManageGameDataModel newGameDataModel, CancellationTokenSource cancellationTokenSource);

        Task<GameDTO> UpdateGameAsync(ManageGameDataModel updateGameDataModel, long gameId, CancellationTokenSource cancellationTokenSource);

        Task DeleteGameAsync(GameDTO targetGame, long relativeTeamId, CancellationTokenSource cancellationTokenSource);

        Task DeleteEventAsync(EventDTO targetEvent, long relativeTeamId, CancellationTokenSource cancellationTokenSource);

        Task<EventDTO> CreateNewEventAsync(ManageEventDataModel newEventDataModel, CancellationTokenSource cancellationTokenSource);

        Task<EventDTO> UpdateEventAsync(ManageEventDataModel updateEventDataModel, long eventId, CancellationTokenSource cancellationTokenSource);
    }
}
