using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.ProfileRequests {
    public class UpdateOuterProfileRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
