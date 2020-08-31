using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class GameDTO : TeamActionBaseDTO {

        [JsonProperty("opponent")]
        public OpponentDTO Opponent { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uniform")]
        public string Uniform { get; set; }

        [JsonProperty("minutesToArriveEarly")]
        public long MinutesToArriveEarly { get; set; }

        [JsonProperty("notForStandings")]
        public bool NotForStandings { get; set; }
    }
}
