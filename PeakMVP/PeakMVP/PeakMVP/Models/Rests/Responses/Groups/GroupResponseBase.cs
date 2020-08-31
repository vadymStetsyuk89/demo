using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Groups {
    public abstract class GroupResponseBase : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("ownerId")]
        public long OwnerId { get; set; }

        [JsonProperty("owner")]
        public ProfileDTO Owner { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("members")]
        public MemberDTO[] Members { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
