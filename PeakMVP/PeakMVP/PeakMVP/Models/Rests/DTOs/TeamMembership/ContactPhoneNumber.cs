using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs.TeamMembership {
    public class ContactPhoneNumber {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
