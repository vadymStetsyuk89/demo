using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Scheduling {
    public class CreateUpdateGameBadResponse: IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }

        [JsonProperty("TeamId")]
        public string[] TeamId { get; set; }

        [JsonProperty("LocationId")]
        public string[] LocationId { get; set; }

        [JsonProperty("OpponentId")]
        public string[] OpponentId { get; set; }

        [JsonProperty("type")]
        public string[] Type { get; set; }

        [JsonProperty("durationInMinutes")]
        public string[] DurationInMinutes { get; set; }

        [JsonProperty("MinutesToArriveEarly")]
        public string[] MinutesToArriveEarly { get; set; }
    }
}
