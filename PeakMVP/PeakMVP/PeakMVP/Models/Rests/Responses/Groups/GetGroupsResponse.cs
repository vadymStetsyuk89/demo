using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Groups {
    public class GetGroupsResponse : IResponse  {

        [JsonProperty("data")]
        public GroupDTO[] Data { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
