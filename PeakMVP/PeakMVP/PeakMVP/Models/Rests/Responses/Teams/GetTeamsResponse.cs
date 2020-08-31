using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class GetTeamsResponse : IResponse {

        [JsonProperty("data")]
        public TeamDTO[] Data { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
