using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class FanProfileContentViewModel : UserTypeDependentProfileContentBaseViewModel {
        public override Task AskToRefreshAsync() => Task.Run(() => { });
    }
}
