using Microsoft.AppCenter.Crashes;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.Base;
using PeakMVP.Views.CompoundedViews.MainContent.Live;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Live {
    public class LiveViewModel : TabbedViewModel {

        private CancellationTokenSource _sheduledEventsCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _determineInterestCancellationTokenSource = new CancellationTokenSource();

        public LiveViewModel() {
            IsNestedPullToRefreshEnabled = true;
        }

        public ICommand SelectedScheduledEventCommand => new Command(async (object param) => {
            if (param is ScheduledEventBase) {
                await NavigationService.NavigateToAsync<ScheduledEventInfoViewModel>(param);
            }
            else {
                Debugger.Break();
            }
        });

        public ICommand EditScoreCommand => new Command(async () => {
            await DialogService.ToastAsync("EditScoreCommand in developing");
        });

        public ICommand ViewScoresAndResultsCommand => new Command(async () => {
            await DialogService.ToastAsync("ViewScoresAndResultsCommand in developing");
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

        private List<ScheduledEventBase> _scheduledEvents;
        public List<ScheduledEventBase> ScheduledEvents {
            get => _scheduledEvents;
            private set => SetProperty<List<ScheduledEventBase>>(ref _scheduledEvents, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _sheduledEventsCancellationTokenSource);
            ResetCancellationTokenSource(ref _determineInterestCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(ResolveScheduledEventsAsync);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _sheduledEventsCancellationTokenSource);
            ResetCancellationTokenSource(ref _determineInterestCancellationTokenSource);
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.LIVE_TITLE;
            TabIcon = NavigationContext.START_IMAGE_PATH;
            RelativeViewType = typeof(LiveView);
        }

        protected override Task NestedRefreshAction() =>
            Task.Run(() => {
                ExecuteActionWithBusy(ResolveScheduledEventsAsync);
            });

        private Task ResolveScheduledEventsAsync() =>
            Task.Run(async () => {
                try {
                    ResetCancellationTokenSource(ref _sheduledEventsCancellationTokenSource);
                    CancellationTokenSource cancellationTokenSource = _sheduledEventsCancellationTokenSource;

                    ///
                    /// TODO: use appropriate service;
                    /// 
                    List<ScheduledEventBase> resolvedScheduledEvents = HARDCODED_DATA();

                    Device.BeginInvokeOnMainThread(() => {
                        ScheduledEvents = resolvedScheduledEvents;
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

        private List<ScheduledEventBase> HARDCODED_DATA() {
            List<ScheduledEventBase> scheduledEvents = new List<ScheduledEventBase>();

            for (int i = 0; i < 12; i++) {
                ScheduledEventBase scheduledEventBase = null;

                if (i % 3 == 0) {
                    scheduledEventBase = new ScheduledGame() {
                        EventName = string.Format("Game vs. {0}", i)
                    };
                }
                else {
                    scheduledEventBase = new ScheduledEvent() {
                        EventName = string.Format("Cat dog bird event name {0}", i)
                    };
                }

                if (i == 0 || i == 1) {
                    scheduledEventBase.IsLive = true;
                }

                scheduledEventBase.Date = new DateTime((DateTime.Now + TimeSpan.FromDays(i)).Ticks);
                scheduledEventBase.TeamName = string.Format("Rangers {0}", i);
                scheduledEventBase.OponentName = string.Format("Sabers {0}", i);
                scheduledEventBase.Location = string.Format("Jomolungma {0}", i % 2 == 0 ? "cat" : "dog");

                scheduledEvents.Add(scheduledEventBase);
            }

            ScheduledEventBase lastScheduledEvent = scheduledEvents.OfType<ScheduledGame>().Last();
            lastScheduledEvent.IsCompleted = true;
            lastScheduledEvent.TeamScore = 75;
            lastScheduledEvent.OponentScore = 3;

            return scheduledEvents;
        }
    }

    public enum Interest {
        Interested,
        Perhaps,
        NotInterested
    }

    public class ScheduledGame : ScheduledEventBase { }

    public class ScheduledEvent : ScheduledEventBase { }

    public abstract class ScheduledEventBase : ObservableObject {

        private string _eventName;
        public string EventName {
            get => _eventName;
            set => SetProperty<string>(ref _eventName, value);
        }

        private DateTime _date;
        public DateTime Date {
            get => _date;
            set => SetProperty<DateTime>(ref _date, value);
        }

        private bool _isLive;
        public bool IsLive {
            get => _isLive;
            set => SetProperty<bool>(ref _isLive, value);
        }

        private bool _isCompleted;
        public bool IsCompleted {
            get => _isCompleted;
            set => SetProperty<bool>(ref _isCompleted, value);
        }

        private Interest? _interest;
        public Interest? Interest {
            get => _interest;
            set => SetProperty<Interest?>(ref _interest, value);
        }

        private string _teamName;
        public string TeamName {
            get => _teamName;
            set => SetProperty<string>(ref _teamName, value);
        }

        private string _oponentName;
        public string OponentName {
            get => _oponentName;
            set => SetProperty<string>(ref _oponentName, value);
        }

        private string _location;
        public string Location {
            get => _location;
            set => SetProperty<string>(ref _location, value);
        }

        private int _teamScore;
        public int TeamScore {
            get => _teamScore;
            set => SetProperty<int>(ref _teamScore, value);
        }

        private int _oponentScore;
        public int OponentScore {
            get => _oponentScore;
            set => SetProperty<int>(ref _oponentScore, value);
        }

        private string _availabilityNote;
        public string AvailabilityNote {
            get => _availabilityNote;
            set => SetProperty<string>(ref _availabilityNote, value);
        }
    }
}
