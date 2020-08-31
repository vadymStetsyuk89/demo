using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class ExternalInvite {

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
