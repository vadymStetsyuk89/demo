using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests {
    public class AuthorisedRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
