using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Live {
    public class ScheduledEventInfoViewModel : LoggedContentPageViewModel {

        private CancellationTokenSource _resolveScheduledEventCancellationTokenSource = new CancellationTokenSource();

        public ScheduledEventInfoViewModel() {

            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

            IsPullToRefreshEnabled = false;

            NestedTabs = new IVisualFiguring[] {
                ViewModelLocator.Resolve<ScheduleEventDetailsViewModel>(),
                ViewModelLocator.Resolve<InterestAndAvailabilityViewModel>(),
                ViewModelLocator.Resolve<EventStatisticsViewModel>()
            };

            NestedTabs.ForEach(visualFiguring => visualFiguring.InitializeAsync(this));
        }

        public ICommand EditCommand => new Command(async () => {
            ///
            /// TODO resolve type of event and open appropriate view
            /// 
            await DialogService.ToastAsync("EditCommand in developing");
        });

        private IVisualFiguring[] _nestedTabs;
        public IVisualFiguring[] NestedTabs {
            get => _nestedTabs;
            private set {
                _nestedTabs?.ForEach<IVisualFiguring>(nestedTab => nestedTab.Dispose());
                SetProperty<IVisualFiguring[]>(ref _nestedTabs, value);
            }
        }

        private ScheduledEventBase _targetScheduledEvent;
        public ScheduledEventBase TargetScheduledEvent {
            get => _targetScheduledEvent;
            private set => SetProperty<ScheduledEventBase>(ref _targetScheduledEvent, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _resolveScheduledEventCancellationTokenSource);

            NestedTabs?.ForEach<IVisualFiguring>(nestedTab => nestedTab.Dispose());
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is ScheduledEventBase scheduledEvent) {
                ResolveScheduledEventAsync(scheduledEvent);
            }

            NestedTabs?.ForEach<IVisualFiguring>(nestedTab => nestedTab.InitializeAsync(navigationData));

            return base.InitializeAsync(navigationData);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged += OnProfileSettingsEventsAppBackgroundImageChanged;
            GlobalSettings.Instance.AppMessagingEvents.LiveScheduleEvents.ScheduledEventUpdated += OnLiveScheduleEventsScheduledEventUpdated;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged -= OnProfileSettingsEventsAppBackgroundImageChanged;
            GlobalSettings.Instance.AppMessagingEvents.LiveScheduleEvents.ScheduledEventUpdated -= OnLiveScheduleEventsScheduledEventUpdated;
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _resolveScheduledEventCancellationTokenSource);
        }

        private Task ResolveScheduledEventAsync(ScheduledEventBase scheduledEvent) {
            return Task.Run(async () => {
                try {
                    ResetCancellationTokenSource(ref _resolveScheduledEventCancellationTokenSource);
                    CancellationTokenSource cancellationTokenSource = _resolveScheduledEventCancellationTokenSource;

                    ///
                    /// TODO: use appropriate api to resolve `fresh` values
                    ///
                    Device.BeginInvokeOnMainThread(() => {
                        TargetScheduledEvent = scheduledEvent;
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });
        }

        private void OnProfileSettingsEventsAppBackgroundImageChanged(object sender, EventArgs e) => AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

        private void OnLiveScheduleEventsScheduledEventUpdated(object sender, ScheduledEventBase e) {
            ///
            /// TODO: use appropriate api to resolve `fresh` values, 
            /// probably call to ResolveScheduledEventAsync(ScheduledEventBase scheduledEvent)
            ///
            ResolveScheduledEventAsync(e);
        }
    }
}
