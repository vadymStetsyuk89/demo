using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.TeamMembers {
    public class AddTeamMemberContactInfoResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
