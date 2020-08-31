using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Responses.Posts {
    public class PublishNewPostBadResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }

        [JsonProperty("text")]
        public string[] Text { get; set; }
    }
}
