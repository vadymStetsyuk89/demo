using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.ProfileMedia {
    public class UploadMediaResponse : MediaDTO, IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
