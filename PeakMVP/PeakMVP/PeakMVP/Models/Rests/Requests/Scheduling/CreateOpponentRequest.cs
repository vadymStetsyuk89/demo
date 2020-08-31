using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Scheduling {
    public class CreateOpponentRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
