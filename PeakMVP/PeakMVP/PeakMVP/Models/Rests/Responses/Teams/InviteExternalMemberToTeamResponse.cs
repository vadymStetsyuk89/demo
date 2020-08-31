using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class InviteExternalMemberToTeamResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }

        [JsonProperty("model")]
        public string[] Model { get; set; }

        [JsonProperty("email")]
        public string[] Email { get; set; }
    }
}
