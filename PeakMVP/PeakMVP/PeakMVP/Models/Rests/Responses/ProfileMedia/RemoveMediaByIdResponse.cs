using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.ProfileMedia {
    public class RemoveMediaByIdResponse : IResponse {

        //[JsonProperty("ProfileMediaId")]
        //public string[] ProfileMediaId { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
