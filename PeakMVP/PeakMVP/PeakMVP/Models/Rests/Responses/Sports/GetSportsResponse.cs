using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Sports {
    public class GetSportsResponse : IResponse {

        [JsonProperty("data")]
        public SportDTO[] Sports { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
