using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests.Scheduling {
    class DeleteEventRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
