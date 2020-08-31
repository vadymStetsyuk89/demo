using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Search {
    public class SearchRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
