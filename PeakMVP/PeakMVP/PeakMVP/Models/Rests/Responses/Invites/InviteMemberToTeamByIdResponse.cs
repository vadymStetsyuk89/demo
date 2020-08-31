using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Invites {
    public class InviteMemberToTeamByIdResponse : IResponse {

        [JsonProperty("member")]
        public ProfileDTO Member { get; set; }

        [JsonProperty("team")]
        public TeamDTO Team { get; set; }

        [JsonProperty("update")]
        public string Update { get; set; }

        [JsonProperty("userToAdd")]
        public string[] UserToAdd { get; set; }

        //[JsonProperty("")]
        //public string[] Empty { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
