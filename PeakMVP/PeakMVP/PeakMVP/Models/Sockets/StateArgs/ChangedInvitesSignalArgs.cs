using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Sockets.StateArgs {
    public class ChangedInvitesSignalArgs {

        [JsonProperty("groupInvites")]
        public GroupDTO[] GroupInvites { get; set; }

        [JsonProperty("teamInvites")]
        public TeamDTO[] TeamInvites { get; set; }

        [JsonProperty("friendshipInvites")]
        public FriendshipDTO[] FriendshipInvites { get; set; }

        [JsonProperty("childInvites")]
        public ChildInvitesDTO[] ChildInvites { get; set; }
    }
}
