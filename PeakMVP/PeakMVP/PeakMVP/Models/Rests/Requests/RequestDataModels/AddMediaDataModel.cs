using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels {
    public class AddMediaDataModel {

        [JsonProperty("file")]
        public FileDTO File { get; set; }

        [JsonProperty("thumbnail")]
        public FileDTO Thumbnail { get; set; }

        [JsonProperty("mediaType")]
        public string MediaType { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("mediaId")]
        public long MediaId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
