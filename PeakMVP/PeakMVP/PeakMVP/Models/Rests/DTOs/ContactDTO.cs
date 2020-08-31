using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class ContactDTO {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
