using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class SendTeamAppointmentResponse : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("profile")]
        public ProfileDTO Profile { get; set; }

        [JsonProperty("team")]
        public TeamDTO Team { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}