using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.DTOs.Search {
    public class ProfilesNodeDTO {

        [JsonProperty("Organization")]
        public ProfileDTO[] Organization { get; set; }

        [JsonProperty("Coach")]
        public ProfileDTO[] Coach { get; set; }

        [JsonProperty("Player")]
        public ProfileDTO[] Player { get; set; }

        [JsonProperty("Parent")]
        public ProfileDTO[] Parent { get; set; }

        [JsonProperty("Fan")]
        public ProfileDTO[] Fan { get; set; }
    }
}
