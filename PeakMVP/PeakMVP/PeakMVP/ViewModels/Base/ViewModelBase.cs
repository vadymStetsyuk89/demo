using PeakMVP.Models.Arguments.InitializeArguments;
using PeakMVP.Services.Cancellation;
using PeakMVP.Services.Dialog;
using PeakMVP.Services.Navigation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Base {
    public abstract class ViewModelBase : ExtendedBindableObject {

        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;
        protected readonly ICancellationService CancellationService;

        public ViewModelBase() {
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
            CancellationService = ViewModelLocator.Resolve<ICancellationService>();
            BackCommand = new Command(async () => await NavigationService.GoBackAsync());
        }

        public ICommand BackCommand { get; protected set; }

        bool _isBusy;
        public bool IsBusy {
            get { return _isBusy; }
            set {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public bool IsSubscribedOnAppEvents { get; private set; }

        public bool IsIntended { get; private set; }

        public virtual Task InitializeAsync(object navigationData) {
            if (!IsSubscribedOnAppEvents) {
                OnSubscribeOnAppEvents();
            }

            if (navigationData is IntentViewModelArgs) {
                TakeIntent();
            }
            else if (navigationData is NoIntentionViewModelArgs) {
                LoseIntent();
            }

            return Task.FromResult(false);
        }

        public virtual void Dispose() {
            OnUnsubscribeFromAppEvents();
            UnsubscribeOnIntentEvent();
        }

        protected void ResetCancellationTokenSource(ref CancellationTokenSource cancellationTokenSource) {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
        }

        protected virtual void OnSubscribeOnAppEvents() {
            IsSubscribedOnAppEvents = true;
        }

        protected virtual void OnUnsubscribeFromAppEvents() {
            IsSubscribedOnAppEvents = false;
        }

        protected virtual void SubscribeOnIntentEvent() { }

        protected virtual void UnsubscribeOnIntentEvent() { }

        protected virtual void TakeIntent() {
            if (!IsIntended) {
                IsIntended = true;
                SubscribeOnIntentEvent();
            }
        }

        protected virtual void LoseIntent() {
            IsIntended = false;
            UnsubscribeOnIntentEvent();
        }
    }
}
