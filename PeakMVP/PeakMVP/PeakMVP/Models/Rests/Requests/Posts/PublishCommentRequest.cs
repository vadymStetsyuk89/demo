using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Posts {
    public class PublishCommentRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
