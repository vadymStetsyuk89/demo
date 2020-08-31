using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Teams {
    public class ExternalMemberTeamIntive
    {

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("invitedUserDisplayName")]
        public string InvitedUserDisplayName { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
