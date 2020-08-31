using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class AssignmentDataModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("teamMemberId")]
        public long? TeamMemberId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
