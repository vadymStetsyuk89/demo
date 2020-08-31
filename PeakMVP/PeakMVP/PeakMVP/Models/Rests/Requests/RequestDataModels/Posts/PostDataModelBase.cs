using Newtonsoft.Json;
using PeakMVP.Models.Identities.Feed;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Posts {
    public abstract class PostDataModelBase {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("groupId")]
        public long? GroupId { get; set; }

        [JsonProperty("postPolicyType")]
        public PostPolicyType PostPolicyType { get; set; }

        [JsonProperty("files")]
        public long[] Files { get; set; }

        [JsonProperty("markedPostMedia")]
        public long[] MarkedPostMedia { get; set; }
    }

    //public abstract class PostDataModelBase {
    //    [JsonProperty("text")]
    //    public string Text { get; set; }

    //    [JsonProperty("groupId")]
    //    public long GroupId { get; set; }

    //    [JsonProperty("postPolicyType")]
    //    public PostPolicyType PostPolicyType { get; set; }

    //    [JsonProperty("files")]
    //    public AttachedFileDataModel[] Files { get; set; }

    //    [JsonProperty("profileId")]
    //    public long ProfileId { get; set; }

    //    [JsonProperty("profileType")]
    //    public string ProfileType { get; set; }
    //}
}

