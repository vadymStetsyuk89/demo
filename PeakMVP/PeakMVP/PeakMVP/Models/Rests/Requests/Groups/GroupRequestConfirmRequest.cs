using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Groups {
    public class GroupRequestConfirmRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}

