using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class ApproveTeamRequestsResponse : IResponse {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sport")]
        public object Sport { get; set; }

        [JsonProperty("coachId")]
        public object CoachId { get; set; }

        [JsonProperty("owner")]
        public ProfileDTO Owner { get; set; }

        [JsonProperty("createdBy")]
        public TeamMember CreatedBy { get; set; }

        [JsonProperty("coach")]
        public object Coach { get; set; }

        [JsonProperty("organizationId")]
        public object OrganizationId { get; set; }

        [JsonProperty("organization")]
        public object Organization { get; set; }

        [JsonProperty("locations")]
        public object[] Locations { get; set; }

        [JsonProperty("opponents")]
        public object[] Opponents { get; set; }

        [JsonProperty("games")]
        public object[] Games { get; set; }

        [JsonProperty("events")]
        public object[] Events { get; set; }

        [JsonProperty("members")]
        public MemberDTO[] Members { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
