using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using PeakMVP.Models.Rests.Responses.Friends.Contracts;

namespace PeakMVP.Models.Rests.Responses.Friends {
    public class GetFriendByIdResponse : IResponse, IStatusProfile {
        [JsonProperty("profile")]
        public ProfileDTO Profile { get; set; }

        [JsonProperty("isConfirmed")]
        public bool IsConfirmed { get; set; }

        [JsonProperty("isRequest")]
        public bool IsRequest { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
