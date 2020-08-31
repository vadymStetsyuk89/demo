namespace PeakMVP.Models.Rests.Requests.Contracts {
    public interface IRequest {
        string Url { get; set; }

        object Data { get; set; }
    }
}
