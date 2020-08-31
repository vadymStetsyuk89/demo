using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Teams {
    /// <summary>
    /// Only used in appropriate team service, when `SendTeamAppointmentResponse is bad`. SendTeamAppointmentResponse also contains Team property...
    /// </summary>
    public class SendTeamAppointmentBadResponse {

        [JsonProperty("team")]
        public string[] Team { get; set; }
    }
}
