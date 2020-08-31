using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.DTOs {
    public class ProfileMediaDTO : MediaDTO {
        [JsonProperty("order")]
        public long Order { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        /// <summary>
        /// Deprecated. Backend returns bad values for this property, resolve image/video by mimi type.
        /// </summary>
        //[JsonProperty("category")]
        //public string Category { get; set; }
    }
}
