using PeakMVP.ViewModels.MainContent.ActionBars.Top;

namespace PeakMVP.ViewModels.Base {
    public abstract class LoggedContentPageViewModel : ContentPageBaseViewModel {

        public LoggedContentPageViewModel() {
            ActionBarViewModel = ViewModelLocator.Resolve<ModeActionBarViewModel>();

            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = false;
            ActionBarViewModel.InitializeAsync(this);
        }
    }
}
