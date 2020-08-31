using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Profile
{
    public class BackgroundImageDataModel
    {
        [JsonProperty("file")]
        public FileDTO File { get; set; }

        [JsonProperty("thumbnail")]
        public FileDTO Thumbnail { get; set; }
    }
}
