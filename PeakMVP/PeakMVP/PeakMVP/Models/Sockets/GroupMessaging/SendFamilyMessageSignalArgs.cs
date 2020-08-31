using Newtonsoft.Json;

namespace PeakMVP.Models.Sockets.GroupMessaging {
    public class SendFamilyMessageSignalArgs {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("familyId")]
        public long FamilyId { get; set; }
    }
}
