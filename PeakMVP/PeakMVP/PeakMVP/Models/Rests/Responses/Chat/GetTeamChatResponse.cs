using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Chat {
    public class GetTeamChatResponse : IResponse {
        [JsonProperty("data")]
        public MessageDTO[] TeamMessages { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
