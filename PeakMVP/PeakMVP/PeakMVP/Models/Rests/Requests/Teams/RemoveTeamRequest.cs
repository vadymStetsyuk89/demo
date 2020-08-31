using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Teams {
    public class RemoveTeamRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
