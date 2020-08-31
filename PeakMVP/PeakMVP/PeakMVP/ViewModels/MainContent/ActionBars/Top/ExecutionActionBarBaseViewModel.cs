using PeakMVP.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ActionBars.Top {
    public abstract class ExecutionActionBarBaseViewModel : NestedViewModel, IActionbarViewModel {

        public ICommand ExecuteCommand => new Command(OnExecuteCommand, () => IsEcutionAvailable);

        public ContentPageBaseViewModel RelativeContentPageBaseViewModel { get; private set; }

        private string _title;
        public string Title {
            get => _title;
            protected set => SetProperty<string>(ref _title, value);
        }

        private bool _isEcutionAvailable;
        public bool IsEcutionAvailable {
            get => _isEcutionAvailable;
            protected set => SetProperty<bool>(ref _isEcutionAvailable, value);
        }

        public virtual void ResolveExecutionAvailability(object condition) { }

        protected virtual void OnExecuteCommand() { }
    }
}
