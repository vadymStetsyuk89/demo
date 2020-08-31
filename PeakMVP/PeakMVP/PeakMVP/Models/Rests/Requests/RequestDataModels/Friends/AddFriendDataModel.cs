using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Friends {
    public class AddFriendDataModel {
        [JsonProperty("friendId")]
        public long FriendId { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
