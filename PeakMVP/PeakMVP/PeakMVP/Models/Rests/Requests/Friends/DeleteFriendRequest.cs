using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Friends {
    public class DeleteFriendRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}

