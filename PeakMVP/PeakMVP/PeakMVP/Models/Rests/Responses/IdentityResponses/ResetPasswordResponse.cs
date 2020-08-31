using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.IdentityResponses {
    public class ResetPasswordResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }

        [JsonProperty("Identificator")]
        public string[] Identificator { get; set; }
    }
}
