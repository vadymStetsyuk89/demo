using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.TeamMembers {
    public class AddTeamMemberContactInfoRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
