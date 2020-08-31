using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Invites {
    public class InviteMemberToTeamByIdRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
