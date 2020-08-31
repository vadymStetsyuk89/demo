using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Teams {
    public class InviteExternalMemberToTeamRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
