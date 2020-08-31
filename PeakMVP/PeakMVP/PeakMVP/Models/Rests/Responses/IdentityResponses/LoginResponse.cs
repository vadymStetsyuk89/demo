using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.IdentityResponses {
    public class LoginResponse : IResponse {

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("expiredIn")]
        public long ExpiredIn { get; set; }

        [JsonProperty("user")]
        public string[] User { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
