using System.Threading.Tasks;

namespace PeakMVP.Services.SignalR {
    public interface ISignalService {

        Task StartAsync(string accessToken);

        Task StopAsync();
    }
}
