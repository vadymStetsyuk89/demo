using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Teams {
    public class RejectTeamRequestsRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
