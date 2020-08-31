using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses {
    public class CommonResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
