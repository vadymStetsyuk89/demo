using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Groups {
    public sealed class GroupRequestDeclineRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
