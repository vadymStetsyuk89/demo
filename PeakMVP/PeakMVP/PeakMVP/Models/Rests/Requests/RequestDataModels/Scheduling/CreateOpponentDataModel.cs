using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class CreateOpponentDataModel {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
