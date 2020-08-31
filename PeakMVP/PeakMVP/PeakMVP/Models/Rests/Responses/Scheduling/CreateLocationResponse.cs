using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Scheduling {
    public class CreateLocationResponse : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("notes")]
        public object Notes { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
