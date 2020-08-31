using Newtonsoft.Json;

namespace PeakMVP.Models.Sockets.Messages {
    public class SendMessageSignalArgs {
        [JsonProperty("Text")]
        public string Text { get; set; }

        [JsonProperty("To")]
        public long To { get; set; }

        [JsonProperty("DeliveryTrackingId")]
        public string DeliveryTrackingId { get; set; }
    }
}
