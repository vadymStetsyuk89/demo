using Microsoft.AppCenter.Crashes;
using PeakMVP.Helpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Base {
    public abstract class NestedViewModel : ViewModelBase {

        private static readonly string _OUTPUT_WARNING_RELATIVE_CONTENT_PAGE_VIEW_MODEL_IS_NOT_RESOLVED = "Output warning relative content page view model is notresolved";

        /// <summary>
        /// Crutch note. This is the reference to the `relative ContentPageBaseViewModel`. With current navigation system there still may be cases when
        /// NestedViewModel can't resolve `parent view model` with NavigationService.LastPageViewModel approach, and in that case it cant use busy indicators (they simply will not be activated).
        /// This field will be affiliate once when nested view model on it's InitializeAsync(object navigationData) will receive ContentPageBaseViewModel.
        /// </summary>
        private ContentPageBaseViewModel _relativeContentPageBase;

        public NestedViewModel() {
            NestedRefreshCommand = new Command(async () => {
                try {
                    IsNestedRefreshing = true;
                    await NestedRefreshAction();
                    await Task.Delay(AppConsts.DELAY_STUB);
                }
                catch (Exception exc) {
                    Debugger.Break();
                    Crashes.TrackError(exc);
                }

                IsNestedRefreshing = false;
            });
        }

        private ICommand _nestedRefreshCommand;
        public ICommand NestedRefreshCommand {
            get => _nestedRefreshCommand;
            private set => SetProperty<ICommand>(ref _nestedRefreshCommand, value);
        }

        private bool _isNestedPullToRefreshEnabled;
        public bool IsNestedPullToRefreshEnabled {
            get => _isNestedPullToRefreshEnabled;
            protected set => SetProperty<bool>(ref _isNestedPullToRefreshEnabled, value);
        }

        private bool _isNestedRefreshing;
        public bool IsNestedRefreshing {
            get => _isNestedRefreshing;
            set => SetProperty<bool>(ref _isNestedRefreshing, value);
        }

        protected virtual Task NestedRefreshAction() => Task.Run(() => { });

        protected void UpdatePopupScopeVisibility(bool isPopupVisible) {
            ContentPageBaseViewModel targetContentPage = IsThisInVisualScope();

            if (targetContentPage != null) {
                targetContentPage.IsPopupsVisible = isPopupVisible;
            }
            else {
                Console.WriteLine(_OUTPUT_WARNING_RELATIVE_CONTENT_PAGE_VIEW_MODEL_IS_NOT_RESOLVED);
            }
        }

        protected void UpdateBusyVisualState(bool isBusy) {
            ContentPageBaseViewModel targetContentPage = IsThisInVisualScope();

            if (targetContentPage != null) {
                targetContentPage.IsBusy = isBusy;
            }
            else {
                Console.WriteLine(_OUTPUT_WARNING_RELATIVE_CONTENT_PAGE_VIEW_MODEL_IS_NOT_RESOLVED);
            }
        }

        protected void UpdateBusyVisualState(Guid busyKey, bool isBusy) {
            ContentPageBaseViewModel targetContentPage = IsThisInVisualScope();

            if (targetContentPage != null) {
                Device.BeginInvokeOnMainThread(() => {
                    try {
                        ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).SetBusy(busyKey, isBusy);
                    }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);
                        Debugger.Break();
                    }
                });
            }
            else {
                Console.WriteLine(_OUTPUT_WARNING_RELATIVE_CONTENT_PAGE_VIEW_MODEL_IS_NOT_RESOLVED);
            }
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ContentPageBaseViewModel contentPageBaseViewModel) {
                if (_relativeContentPageBase == null) {
                    _relativeContentPageBase = contentPageBaseViewModel;
                }
            }

            return base.InitializeAsync(navigationData);
        }

        protected async void ExecuteActionWithBusy(Func<Task> asyncFunc) {
            if (asyncFunc == null) {
                Debugger.Break();
                return;
            }

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                await asyncFunc();
            }
            catch (Exception exc) {
                Debugger.Break();
                Crashes.TrackError(exc);
            }

            UpdateBusyVisualState(busyKey, false);
        }

        private ContentPageBaseViewModel IsThisInVisualScope() {
            ContentPageBaseViewModel relativeContentPageBase = null;

            if (NavigationService.LastPageViewModel != null && NavigationService.LastPageViewModel is ContentPageBaseViewModel contentPageBaseViewModel) {
                relativeContentPageBase = contentPageBaseViewModel;
            }
            else {
                relativeContentPageBase = _relativeContentPageBase;
            }

            return relativeContentPageBase;
        }
    }
}
