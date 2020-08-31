using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public sealed class MessageDTO {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("fromId")]
        public long FromId { get; set; }

        [JsonProperty("toId")]
        public long ToId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("seen")]
        public bool Seen { get; set; }

        [JsonProperty("deliveryTrackingId")]
        public object DeliveryTrackingId { get; set; }
    }
}
