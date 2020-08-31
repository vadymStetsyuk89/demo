using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Groups {
    public sealed class GroupRequestDeclineResponse : IResponse {
        [JsonProperty("statusCode")]
        public long StatusCode { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
