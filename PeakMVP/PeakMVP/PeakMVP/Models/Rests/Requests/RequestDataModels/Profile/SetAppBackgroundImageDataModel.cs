using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Profile {
    public class SetAppBackgroundImageDataModel
    {
        //[JsonProperty("backgroundImage")]
        //public BackgroundImageDataModel BackgroundImage { get; set; }

        //[JsonProperty("profileId")]
        //public long ProfileId { get; set; }

        //[JsonProperty("profileType")]
        //public string ProfileType { get; set; }

        [JsonProperty("mediaId")]
        public long MediaId { get; set; }
    }
}
