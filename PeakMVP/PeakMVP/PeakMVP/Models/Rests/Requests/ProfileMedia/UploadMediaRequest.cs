using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.ProfileMedia {
    public class UploadMediaRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
