using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Live.ScheduleEventInfo;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo {
    public class ScheduleEventDetailsViewModel : NestedViewModel, IVisualFiguring {

        public static readonly string DETAIL_TITLE = "Detail";
        public static readonly string EDIT_AVAILABILITY_NOTE_WARNING = "Can't edit availability note now";

        private CancellationTokenSource _resolveScheduledEventCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _determineInterestCancellationTokenSource = new CancellationTokenSource();

        public ScheduleEventDetailsViewModel() {
            AddAvailabilityNotePopup = ViewModelLocator.Resolve<AddAvailabilityNotePopupViewModel>();
            AddAvailabilityNotePopup.InitializeAsync(this);
        }

        public ICommand EditAvailabilityNoteCommand => new Command(async () => {
            try {
                AddAvailabilityNotePopup.ShowPopupCommand.Execute(TargetScheduledEvent);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(EDIT_AVAILABILITY_NOTE_WARNING);
            }
        });

        public ICommand DetermineAsInterestedCommand => new Command((object param) => {
            if (param is ScheduledEventBase scheduledEvent) {
                DetermineInterestAsync(Interest.Interested, scheduledEvent);
            }
        });

        public ICommand DetermineAsPerhapsCommand => new Command((object param) => {
            if (param is ScheduledEventBase scheduledEvent) {
                DetermineInterestAsync(Interest.Perhaps, scheduledEvent);
            }
        });

        public ICommand DetermineAsNotInterestedCommand => new Command((object param) => {
            if (param is ScheduledEventBase scheduledEvent) {
                DetermineInterestAsync(Interest.NotInterested, scheduledEvent);
            }
        });

        public ICommand ViewLocationInfoCommand => new Command(async () => await DialogService.ToastAsync("ViewLocationInfoCommand in developing"));

        public ICommand ViewOponentInfoCommand => new Command(async () => await DialogService.ToastAsync("ViewOponentInfoCommand in developing"));

        public ICommand LiveCommand => new Command(async () => await DialogService.ToastAsync("LiveCommand in developing"));

        public ICommand CreateLineUpCommand => new Command(async () => await DialogService.ToastAsync("CreateLineUpCommand in developing"));

        private ScheduledEventBase _targetScheduledEvent;
        public ScheduledEventBase TargetScheduledEvent {
            get => _targetScheduledEvent;
            private set => SetProperty<ScheduledEventBase>(ref _targetScheduledEvent, value);
        }

        private AddAvailabilityNotePopupViewModel _addAvailabilityNotePopup;
        public AddAvailabilityNotePopupViewModel AddAvailabilityNotePopup {
            get => _addAvailabilityNotePopup;
            private set {
                _addAvailabilityNotePopup?.Dispose();
                SetProperty<AddAvailabilityNotePopupViewModel>(ref _addAvailabilityNotePopup, value);
            }
        }

        public Type RelativeViewType => typeof(ScheduleEventDetailsView);

        public string TabHeader => DETAIL_TITLE.ToUpper();

        public override void Dispose() {
            base.Dispose();

            AddAvailabilityNotePopup?.Dispose();
            ResetCancellationTokenSource(ref _resolveScheduledEventCancellationTokenSource);
            ResetCancellationTokenSource(ref _determineInterestCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is ScheduledEventBase scheduledEvent) {
                ResolveScheduledEventAsync(scheduledEvent);
            }

            AddAvailabilityNotePopup?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _resolveScheduledEventCancellationTokenSource);
            ResetCancellationTokenSource(ref _determineInterestCancellationTokenSource);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.LiveScheduleEvents.ScheduledEventUpdated += OnLiveScheduleEventsScheduledEventUpdated;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.LiveScheduleEvents.ScheduledEventUpdated -= OnLiveScheduleEventsScheduledEventUpdated;
        }

        private Task DetermineInterestAsync(Interest interest, ScheduledEventBase scheduledEvent) =>
            Task.Run(async () => {
                Guid busyKey = Guid.NewGuid();
                UpdateBusyVisualState(busyKey, true);

                ResetCancellationTokenSource(ref _determineInterestCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _determineInterestCancellationTokenSource;

                try {
                    ///
                    /// TODO: use appropriate service;
                    /// 
                    Device.BeginInvokeOnMainThread(() => {
                        scheduledEvent.Interest = interest;
                        GlobalSettings.Instance.AppMessagingEvents.LiveScheduleEvents.ScheduledEventUpdatedInvoke(this, scheduledEvent);
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }

                UpdateBusyVisualState(busyKey, false);
            });

        private Task ResolveScheduledEventAsync(ScheduledEventBase scheduledEvent) =>
            Task.Run(async () => {
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

        private void OnLiveScheduleEventsScheduledEventUpdated(object sender, ScheduledEventBase e) {
            ///
            /// TODO: use appropriate api to resolve `fresh` values, 
            /// probably call to ResolveScheduledEventAsync(ScheduledEventBase scheduledEvent)
            ///
            ResolveScheduledEventAsync(e);
        }
    }
}
