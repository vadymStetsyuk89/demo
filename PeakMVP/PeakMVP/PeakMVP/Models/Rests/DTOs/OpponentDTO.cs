using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class OpponentDTO {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("notes")]
        public object Notes { get; set; }
    }
}
