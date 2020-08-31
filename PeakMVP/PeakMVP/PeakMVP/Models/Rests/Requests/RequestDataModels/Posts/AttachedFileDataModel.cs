using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Posts {
    public class AttachedFileDataModel {
        [JsonProperty("file")]
        public FileDTO File { get; set; }

        [JsonProperty("thumbnail")]
        public FileDTO Thumbnail { get; set; }

        public string MimeType { get; set; }
    }
}
