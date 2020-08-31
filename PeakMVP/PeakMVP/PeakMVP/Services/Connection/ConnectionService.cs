using Plugin.Connectivity;

namespace PeakMVP.Services.Connection {
    public class ConnectionService : IConnectionService {
        public bool CheckOnline() => CrossConnectivity.Current.IsConnected;
    }
}
