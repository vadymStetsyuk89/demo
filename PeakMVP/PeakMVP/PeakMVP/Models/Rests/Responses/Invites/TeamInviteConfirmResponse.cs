using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;

namespace PeakMVP.Models.Rests.Responses.Invites {
    public class TeamInviteConfirmResponse : IResponse {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        public TeamDTO Team { get; set; }

        [JsonProperty("member")]
        public MemberDTO Member { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("joined")]
        public DateTime Joined { get; set; }

        [JsonProperty("assignments")]
        public AssignmentDTO[] Assignments { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
