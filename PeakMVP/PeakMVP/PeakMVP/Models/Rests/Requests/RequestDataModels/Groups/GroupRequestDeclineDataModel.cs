using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Groups {
    public sealed class GroupRequestDeclineDataModel {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("groupId")]
        public long GroupId { get; set; }
    }
}
