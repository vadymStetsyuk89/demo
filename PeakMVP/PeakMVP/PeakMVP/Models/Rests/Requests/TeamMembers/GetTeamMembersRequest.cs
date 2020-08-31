using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.TeamMembers {
    public class GetTeamMembersRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
