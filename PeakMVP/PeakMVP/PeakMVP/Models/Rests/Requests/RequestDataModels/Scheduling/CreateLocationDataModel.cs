using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class CreateLocationDataModel {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
