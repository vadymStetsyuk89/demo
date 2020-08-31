using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Chat {
    public class GetGroupChatRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
