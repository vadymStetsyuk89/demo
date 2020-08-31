using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Posts {
    public class DeletePostResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
