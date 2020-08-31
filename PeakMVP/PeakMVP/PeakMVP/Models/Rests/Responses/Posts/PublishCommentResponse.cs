using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;

namespace PeakMVP.Models.Rests.Responses.Posts {
    public class PublishCommentResponse : IResponse {

        [JsonProperty("author")]
        public ProfileDTO Author { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
