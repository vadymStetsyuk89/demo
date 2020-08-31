using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Invites {
    public class InvitesRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
