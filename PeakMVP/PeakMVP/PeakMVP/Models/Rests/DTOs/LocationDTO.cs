using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class LocationDTO {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("notes")]
        public object Notes { get; set; }
    }
}
