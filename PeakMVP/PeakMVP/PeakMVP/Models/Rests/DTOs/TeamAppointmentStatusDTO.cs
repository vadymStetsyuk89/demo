using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class TeamAppointmentStatusDTO {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("profile")]
        public ProfileDTO Profile { get; set; }

        [JsonProperty("team")]
        public TeamDTO Team { get; set; }
    }
}
