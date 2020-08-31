using Newtonsoft.Json;

namespace PeakMVP.Models.Sockets.Messages {
    public class MarkFriendMessagesAsReadedSignalArgs {

        [JsonProperty("MessageIds")]
        public long[] MessageIds { get; set; }
    }
}
