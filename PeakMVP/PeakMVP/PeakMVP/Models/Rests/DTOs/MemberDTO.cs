using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class MemberDTO {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("profile")]
        public ProfileDTO Profile { get; set; }

        [JsonProperty("group")]
        public GroupDTO Group { get; set; }
    }
}