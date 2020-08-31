using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Scheduling {
    public class DeleteEventResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
