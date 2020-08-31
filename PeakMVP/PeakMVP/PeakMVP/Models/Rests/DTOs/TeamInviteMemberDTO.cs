using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public class TeamInviteMemberDTO {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("member")]
        public ProfileDTO Member { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("joined")]
        public DateTime Joined { get; set; }

        [JsonProperty("assignments")]
        public AssignmentDTO[] Assignments { get; set; }
    }
}
