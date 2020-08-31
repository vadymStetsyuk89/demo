using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Invites {
    public class InvitesResponse : IResponse {
        [JsonProperty("groupInvites")]
        public GroupDTO[] GroupInvites { get; set; }

        [JsonProperty("teamInvites")]
        public TeamDTO[] TeamInvites { get; set; }

        [JsonProperty("friendshipInvites")]
        public FriendshipDTO[] FriendshipInvites { get; set; }

        [JsonProperty("childInvites")]
        public ChildInvitesDTO[] ChildInvites { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
