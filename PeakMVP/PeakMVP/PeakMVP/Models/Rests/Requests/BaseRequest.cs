using PeakMVP.Models.Rests.Requests.Contracts;

namespace PeakMVP.Models.Rests.Requests {
    public abstract class BaseRequest : IRequest {
        /// <summary>
        /// Target url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Data to send. Will be serialized and attached to request.
        /// </summary>
        public object Data { get; set; }
    }
}
