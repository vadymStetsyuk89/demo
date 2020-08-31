using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs.Base;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class NixTeamAppointmentResponse : ProfileBase, IResponse {

        [JsonProperty("newMessageIds")]
        public string[] NewMessageIds { get; set; }

        [JsonProperty("relation")]
        public string[] Relation { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
