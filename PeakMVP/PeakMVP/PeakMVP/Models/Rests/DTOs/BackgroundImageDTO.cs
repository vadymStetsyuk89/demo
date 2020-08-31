using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class BackgroundImageDTO {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mime")]
        public string Mime { get; set; }
    }
}
