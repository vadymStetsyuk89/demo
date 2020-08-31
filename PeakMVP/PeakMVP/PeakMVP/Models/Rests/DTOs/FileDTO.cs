using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class FileDTO {
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
