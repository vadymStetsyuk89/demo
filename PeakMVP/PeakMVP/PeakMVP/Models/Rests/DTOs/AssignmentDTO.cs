using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class AssignmentDTO {
        [JsonProperty("teamMemberId")]
        public long TeamMemberId { get; set; }

        [JsonProperty("gameId")]
        public string GameId { get; set; }

        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
