using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Posts {
    public class PublishNewPostResponse : IResponse {

        //[JsonProperty("id")]
        //public long Id { get; set; }

        //[JsonProperty("author")]
        //public ProfileDTO Author { get; set; }

        //[JsonProperty("postPolicyType")]
        //public string PostPolicyType { get; set; }

        //[JsonProperty("postPolicyName")]
        //public string PostPolicyName { get; set; }

        //[JsonProperty("text")]
        //public string Text { get; set; }

        //[JsonProperty("media")]
        //public ProfileMediaDTO[] Media { get; set; }

        //[JsonProperty("publishTime")]
        //public DateTime PublishTime { get; set; }

        //[JsonProperty("comments")]
        //public CommentDTO[] Comments { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
