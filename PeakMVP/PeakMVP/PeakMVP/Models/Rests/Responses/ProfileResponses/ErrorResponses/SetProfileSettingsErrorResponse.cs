using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.ProfileResponses.ErrorResponses {
    class SetProfileSettingsErrorResponse {
        [JsonProperty("")]
        public string[] Empty { get; set; }

        [JsonProperty("CurrentPassword")]
        public string[] CurrentPassword { get; set; }

        [JsonProperty("Email")]
        public string[] Email { get; set; }

        [JsonProperty("FirstName")]
        public string[] FirstName { get; set; }

        [JsonProperty("LastName")]
        public string[] LastName { get; set; }

        [JsonProperty("providedPassword")]
        public string[] ProvidedPassword { get; set; }
    }
}
