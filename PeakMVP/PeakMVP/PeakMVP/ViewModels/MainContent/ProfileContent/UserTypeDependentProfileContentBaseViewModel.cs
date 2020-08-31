using PeakMVP.ViewModels.Base;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public abstract class UserTypeDependentProfileContentBaseViewModel : NestedViewModel, IAskToRefresh {

        public abstract Task AskToRefreshAsync();
    }
}
