using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.Teams {

    /// <summary>
    /// Only used in appropriate team service, when `CreateTeamResponse is bad`. CreateTeamResponse also contains Name property...
    /// </summary>
    public class CreateTeamBadResponse {

        [JsonProperty("Name")]
        public string[] Name { get; set; }
    }
}
