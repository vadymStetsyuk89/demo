using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Identity {
    public class ResetPasswordDataModel {
        [JsonProperty("identificator")]
        public string Identificator { get; set; }
    }
}
