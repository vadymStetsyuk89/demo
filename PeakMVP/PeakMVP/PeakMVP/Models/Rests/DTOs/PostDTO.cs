using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public class PostDTO {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("author")]
        public ProfileDTO Author { get; set; }

        [JsonProperty("postPolicyType")]
        public string PostPolicyType { get; set; }

        [JsonProperty("postPolicyName")]
        public string PostPolicyName { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("media")]
        public ProfileMediaDTO[] Media { get; set; }

        [JsonProperty("publishTime")]
        public DateTime PublishTime { get; set; }

        [JsonProperty("comments")]
        public CommentDTO[] Comments { get; set; }
    }
}
