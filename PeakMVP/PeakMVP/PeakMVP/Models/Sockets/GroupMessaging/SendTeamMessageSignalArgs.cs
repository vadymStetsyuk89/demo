using Newtonsoft.Json;

namespace PeakMVP.Models.Sockets.GroupMessaging {
    public class SendTeamMessageSignalArgs {

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
