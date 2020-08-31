using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Posts {
    public class GetPostsRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
