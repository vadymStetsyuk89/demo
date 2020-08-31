using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs.TeamMembership;

namespace PeakMVP.Models.Sockets.StateArgs {
    public class ChangedTeamsSignalArgs {
        [JsonProperty("data")]
        public TeamMember[] Data { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }
    }
}
