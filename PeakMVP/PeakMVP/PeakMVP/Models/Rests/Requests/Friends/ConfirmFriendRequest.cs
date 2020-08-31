using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Friends {
    public class ConfirmFriendRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
