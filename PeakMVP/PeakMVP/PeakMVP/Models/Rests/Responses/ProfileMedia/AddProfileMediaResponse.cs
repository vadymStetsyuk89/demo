using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.ProfileMedia {
    public class AddProfileMediaResponse : IResponse {

        //[JsonProperty("order")]
        //public long Order { get; set; }

        //[JsonProperty("profileId")]
        //public long ProfileId { get; set; }

        //[JsonProperty("category")]
        //public string Category { get; set; }

        //[JsonProperty("url")]
        //public string Url { get; set; }

        //[JsonProperty("thumbnailUrl")]
        //public string ThumbnailUrl { get; set; }

        //[JsonProperty("name")]
        //public string Name { get; set; }

        //[JsonProperty("id")]
        //public long Id { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
