﻿using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs.Base;
using PeakMVP.Models.Rests.Responses.Contracts;

namespace PeakMVP.Models.Rests.Responses.ProfileResponses {
    public class SetProfileSettingsResponse : ProfileBase, IResponse {

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
