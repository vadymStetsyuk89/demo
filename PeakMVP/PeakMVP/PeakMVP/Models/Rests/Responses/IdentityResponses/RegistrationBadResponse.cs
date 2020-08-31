using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Responses.IdentityResponses {
    public class RegistrationBadResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }

        [JsonProperty("dateOfBirth")]
        public string[] DateOfBirth { get; set; }
    }
}
