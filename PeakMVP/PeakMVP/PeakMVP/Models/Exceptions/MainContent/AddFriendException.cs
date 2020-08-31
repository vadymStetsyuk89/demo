using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Exceptions.MainContent {
    public class AddFriendException : Exception {
        [JsonProperty("")]
        public string[] Empty { get; set; }

        [JsonProperty("FriendId")]
        public string[] FriendId { get; set; }
    }
}
