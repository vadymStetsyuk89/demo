using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.Search;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Search {

    public class SearchResponse : IResponse {

        [JsonProperty("profiles")]
        public ProfilesNodeDTO Profiles { get; set; }

        [JsonProperty("teams")]
        public TeamDTO[] Teams { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
