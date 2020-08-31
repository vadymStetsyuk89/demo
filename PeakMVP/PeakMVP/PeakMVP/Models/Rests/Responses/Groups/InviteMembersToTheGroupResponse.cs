using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Groups {
    public class InviteMembersToTheGroupResponse : IResponse {

        [JsonProperty("data")]
        public MemberDTO[] Data { get; set; }

        //[JsonProperty("")]
        //public string[] Empty { get; set; }

        [JsonProperty("id")]
        public string[] Id { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
