using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Responses.IdentityResponses {
    public class ImpersonateLogInResponse : IResponse {

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("expiredIn")]
        public long ExpiredIn { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
