using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class ItemDTO {
        [JsonProperty("shortId")]
        public string ShortId { get; set; }

        [JsonProperty("avatars")]
        public MediaDTO[] Avatars { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
