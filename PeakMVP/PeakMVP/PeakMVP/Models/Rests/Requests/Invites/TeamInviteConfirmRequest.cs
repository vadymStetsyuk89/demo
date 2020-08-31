using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Invites {
    public class TeamInviteConfirmRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
