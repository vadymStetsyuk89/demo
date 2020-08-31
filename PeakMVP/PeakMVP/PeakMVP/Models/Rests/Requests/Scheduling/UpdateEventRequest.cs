using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Scheduling {
    public class UpdateEventRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
