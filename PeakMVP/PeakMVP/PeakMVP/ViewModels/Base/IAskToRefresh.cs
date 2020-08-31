using System.Threading.Tasks;

namespace PeakMVP.ViewModels.Base {
    public interface IAskToRefresh {

        Task AskToRefreshAsync();
    }
}
