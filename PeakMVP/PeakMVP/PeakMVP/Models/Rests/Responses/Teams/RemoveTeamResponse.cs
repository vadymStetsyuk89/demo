using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class RemoveTeamResponse : IResponse {

        //[JsonProperty("")]
        //public string[] Empty { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
