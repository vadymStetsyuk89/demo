using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Posts {
    public class PublishCommentDataModel {
        [JsonProperty("postId")]
        public long PostId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
