using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Friends.Contracts;
using PeakMVP.ViewModels.MainContent.Invites;

namespace PeakMVP.Models.Rests.DTOs {
    public class FriendshipDTO : IStatusProfile {
        [JsonProperty("profile")]
        public ProfileDTO Profile { get; set; }

        [JsonProperty("isConfirmed")]
        public bool IsConfirmed { get; set; }

        [JsonProperty("isRequest")]
        public bool IsRequest { get; set; }
    }
}
