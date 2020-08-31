using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class UserDTO {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
