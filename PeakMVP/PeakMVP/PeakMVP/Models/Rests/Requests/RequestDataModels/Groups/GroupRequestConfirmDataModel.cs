using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Groups {
    public sealed class GroupRequestConfirmDataModel {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("groupId")]
        public long GroupId { get; set; }

        //[JsonProperty("isChildUnder13")]
        //public bool IsChildUnder13 { get; set; }
    }
}
