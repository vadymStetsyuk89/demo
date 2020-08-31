using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Family {
    public class GetFamilyResponse : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("parentId")]
        public long ParentId { get; set; }

        [JsonProperty("members")]
        public ProfileDTO[] Members { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
