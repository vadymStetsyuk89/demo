using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Teams {
    public class ResendExternalMemberInviteRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
