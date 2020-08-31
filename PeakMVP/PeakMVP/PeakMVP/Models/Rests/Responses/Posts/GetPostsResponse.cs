using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Posts {
    public class GetPostsResponse : IResponse {

        [JsonProperty("data")]
        public PostDTO[] Posts { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
