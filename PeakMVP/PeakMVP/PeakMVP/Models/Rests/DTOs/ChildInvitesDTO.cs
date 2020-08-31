using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class ChildInvitesDTO {
        [JsonProperty("groupInvites")]
        public GroupDTO[] GroupInvites { get; set; }

        [JsonProperty("teamInvites")]
        public TeamDTO[] TeamInvites { get; set; }

        [JsonProperty("friendshipInvites")]
        public FriendshipDTO[] FriendshipInvites { get; set; }

        [JsonProperty("child")]
        public ProfileDTO Child { get; set; }
    }
}
