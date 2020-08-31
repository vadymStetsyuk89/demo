using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Sockets.Messages {
    public class ReceivedMessageSignalArgs {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fromId")]
        public string FromId { get; set; }

        [JsonProperty("toId")]
        public string ToId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("seen")]
        public bool? Seen { get; set; }

        [JsonProperty("deliveryTrackingId")]
        public string DeliveryTrackingId { get; set; }
    }
}
