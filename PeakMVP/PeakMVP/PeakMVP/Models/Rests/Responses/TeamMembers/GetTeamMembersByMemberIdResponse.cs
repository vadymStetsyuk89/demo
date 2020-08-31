using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;

namespace PeakMVP.Models.Rests.Responses.TeamMembers {
    public class GetTeamMembersByMemberIdResponse : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("team")]
        public TeamDTO Team { get; set; }

        [JsonProperty("member")]
        public ProfileDTO Member { get; set; }

        [JsonProperty("teamMemberContact")]
        public List<TeamMemberContactInfo> ContactInfo { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("joined")]
        public DateTime? Joined { get; set; }

        [JsonProperty("assignments")]
        public AssignmentDTO[] Assignments { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
