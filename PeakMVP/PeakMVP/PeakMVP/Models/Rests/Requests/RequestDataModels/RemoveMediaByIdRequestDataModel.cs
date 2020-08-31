using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels {
    public class RemoveMediaByIdRequestDataModel {
        [JsonProperty("profileMediaId")]
        public long ProfileMediaId { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
