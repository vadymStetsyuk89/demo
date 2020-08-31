using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.ProfileRequests {
    public class GetProfileRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
