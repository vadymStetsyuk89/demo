using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public class CommentDTO {
        [JsonProperty("author")]
        public ProfileDTO Author { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
