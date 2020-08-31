using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Profile {
    public class SetProfileAvatarDataModel {

        [JsonProperty("mediaId")]
        public long MediaId { get; set; }

        [JsonProperty("imageDataBase64")]
        public string ImageDataBase64 { get; set; }

        [JsonProperty("imageFileName")]
        public string ImageFileName { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
