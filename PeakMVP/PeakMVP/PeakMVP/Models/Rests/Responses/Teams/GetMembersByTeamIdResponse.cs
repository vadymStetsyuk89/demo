using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class GetMembersByTeamIdResponse : IResponse {

        [JsonProperty("data")]
        public TeamMember[] Data { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
