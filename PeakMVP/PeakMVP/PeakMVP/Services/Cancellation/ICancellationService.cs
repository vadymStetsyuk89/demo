using System.Threading;

namespace PeakMVP.Services.Cancellation {
    public interface ICancellationService {
        CancellationTokenSource GetCancellationTokenSource();

        CancellationToken GetToken();

        void Cancel();
    }
}
