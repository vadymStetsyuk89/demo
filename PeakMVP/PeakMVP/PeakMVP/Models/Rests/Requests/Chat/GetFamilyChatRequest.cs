using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Chat {
    public class GetFamilyChatRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
