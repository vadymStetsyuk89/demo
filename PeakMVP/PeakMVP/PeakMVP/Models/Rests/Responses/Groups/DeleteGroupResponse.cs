using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Groups {
    public class DeleteGroupResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
