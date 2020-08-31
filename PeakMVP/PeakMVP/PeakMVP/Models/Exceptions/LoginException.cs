using Newtonsoft.Json;

namespace PeakMVP.Models.Exceptions {
    public class LoginException {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
