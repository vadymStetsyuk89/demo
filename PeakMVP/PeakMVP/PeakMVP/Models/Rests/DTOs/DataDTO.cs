using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class DataDTO {
        [JsonProperty("items")]
        public ItemDTO[] Items { get; set; }

        [JsonProperty("avatars")]
        public MediaDTO[] Avatars { get; set; }

        [JsonProperty("shortId")]
        public string ShortId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
