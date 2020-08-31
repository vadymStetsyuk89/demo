using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class DeleteGameDataModel {

        [JsonProperty("gameId")]
        public long GameId { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
