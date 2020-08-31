using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Responses.Contracts {
    public interface IResponse {
        [JsonProperty("")]
        string[] Errors { get; set; }
    }
}
