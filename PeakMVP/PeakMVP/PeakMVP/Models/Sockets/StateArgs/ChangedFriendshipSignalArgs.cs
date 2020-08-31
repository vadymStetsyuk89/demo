using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Sockets.StateArgs {
    public class ChangedFriendshipSignalArgs {
        [JsonProperty("data")]
        public FriendshipDTO[] Data { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }
    }
}
