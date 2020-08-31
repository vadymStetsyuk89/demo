using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.ProfileMedia {
    public class DeleteMediaResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
