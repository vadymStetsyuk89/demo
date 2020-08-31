using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Sports {
    class GetSportsRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}

