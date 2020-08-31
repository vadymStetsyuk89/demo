using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.TeamMembers {
    public class GetTeamMembersResponse : IResponse {

        [JsonProperty("data")]
        public TeamMember[] TeamMembers { get; set; }

        [JsonProperty("nextPageId")]
        public string NextPageId { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
