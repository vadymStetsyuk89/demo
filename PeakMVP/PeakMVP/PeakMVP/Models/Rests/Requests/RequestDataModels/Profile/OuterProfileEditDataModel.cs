using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Profile {
    public class OuterProfileEditDataModel
    {
        [JsonProperty("teamMemberProfileId")]
        public long TeamMemberProfileId { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("sports")]
        public string Sports { get; set; }
    }
}
