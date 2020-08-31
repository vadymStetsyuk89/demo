using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class ResendExternalMemberInviteResponse : IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
