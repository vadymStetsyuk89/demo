using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Teams {
    public class CreateTeamDataModel {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sportId")]
        public long SportId { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
