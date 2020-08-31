using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Teams {
    public class TeamExternalInvitesRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
