using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Identities.Scheduling;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Events.Arguments;
using PeakMVP.ViewModels.MainContent.Events.EventsPopups;
using PeakMVP.Views.CompoundedViews.MainContent.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class CalendarViewingOfEventsAndGamesContentViewModel : NestedViewModel, IVisualFiguring {

        private static readonly string _CALENDAR_DAY_DETAILS_INFO_ERROR = "Can't view day details now";

        private TeamMember _targetTeamMember;


        public CalendarViewingOfEventsAndGamesContentViewModel() {
            ViewDayAppointmentsPopupViewModel = ViewModelLocator.Resolve<ViewDayAppointmentsPopupViewModel>();
        }

        public ICommand DayTappedCommand => new Command(async (param) => {
            try {
                if (param is DateTime value) {
                    IEnumerable<CalendarScheduleEventViewModel> scheduleContexts = CalendarScheduleEvents.Where<CalendarScheduleEventViewModel>(calendarScheduleEventViewModel => (calendarScheduleEventViewModel.StartDate.Year == value.Year && calendarScheduleEventViewModel.StartDate.Month == value.Month && calendarScheduleEventViewModel.StartDate.Day == value.Day));

                    if (scheduleContexts.Any()) {
                        GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeCalendarScheduleDateSelected(this, new ScheduledCalendarDateSelectedArgs() {
                            Date = value,
                            Context = scheduleContexts.ToArray()
                        });
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(_CALENDAR_DAY_DETAILS_INFO_ERROR);
            }
        });

        public Type RelativeViewType => typeof(CalendarViewingOfEventsAndGamesContentView);

        public string TabHeader { get; private set; }

        private ObservableCollection<CalendarScheduleEventViewModel> _calendarScheduleEvents;
        public ObservableCollection<CalendarScheduleEventViewModel> CalendarScheduleEvents {
            get => _calendarScheduleEvents;
            private set {
                _calendarScheduleEvents?.ForEach<CalendarScheduleEventViewModel>(scheduleEvent => scheduleEvent.Dispose());
                SetProperty<ObservableCollection<CalendarScheduleEventViewModel>>(ref _calendarScheduleEvents, value);
            }
        }

        private ViewDayAppointmentsPopupViewModel _viewDayAppointmentsPopupViewModel;
        public ViewDayAppointmentsPopupViewModel ViewDayAppointmentsPopupViewModel {
            get => _viewDayAppointmentsPopupViewModel;
            private set {
                _viewDayAppointmentsPopupViewModel?.Dispose();
                _viewDayAppointmentsPopupViewModel = value;
            }
        }

        public override void Dispose() {
            base.Dispose();

            ViewDayAppointmentsPopupViewModel?.Dispose();
            CalendarScheduleEvents?.ForEach<CalendarScheduleEventViewModel>(scheduleEvent => scheduleEvent.Dispose());
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ExtractEventsAndGamesForSelectedTeamArgs gamesForSelectedTeamArgs) {
                ViewDayAppointmentsPopupViewModel?.ClosePopupCommand.Execute(null);

                _targetTeamMember = gamesForSelectedTeamArgs.TargetTeamMember;

                CalendarScheduleEvents = gamesForSelectedTeamArgs.TargetTeamMember?.Team?.Events?
                    .Cast<TeamActionBaseDTO>()
                    .Concat<TeamActionBaseDTO>(gamesForSelectedTeamArgs.TargetTeamMember?.Team?.Games ?? new List<GameDTO>())
                    .SelectMany<TeamActionBaseDTO, CalendarScheduleEventViewModel>(teamAction => {
                        List<CalendarScheduleEventViewModel> result = new List<CalendarScheduleEventViewModel>();

                        if (teamAction is GameDTO) {
                            CalendarScheduleEventViewModel eventViewModel = ViewModelLocator.Resolve<CalendarScheduleEventViewModel>();
                            eventViewModel.InitializeAsync(new ChargeScheduleActionArgs() { ScheduleAction = new ScheduleAction(teamAction, _targetTeamMember), StartDate = teamAction.StartDate });

                            result.Add(eventViewModel);
                        }
                        else if (teamAction is EventDTO eventAction) {
                            if (eventAction.RepeatingType == ActionRepeatsDataItem.DAILY_REPEAT_VALUE) {
                                int repeats = (eventAction.RepeatsUntil - eventAction.StartDate).Days;
                                DateTime nextDate = eventAction.StartDate;

                                for (int i = 0; i <= repeats; i++) {
                                    CalendarScheduleEventViewModel eventViewModel = ViewModelLocator.Resolve<CalendarScheduleEventViewModel>();
                                    eventViewModel.InitializeAsync(new ChargeScheduleActionArgs() { ScheduleAction = new ScheduleAction(teamAction, _targetTeamMember), StartDate = nextDate });

                                    result.Add(eventViewModel);

                                    nextDate = nextDate + TimeSpan.FromDays(1);
                                }
                            }
                            else if (eventAction.RepeatingType == ActionRepeatsDataItem.WEEKLY_REPEAT_VALUE) {
                                int repeats = ((eventAction.RepeatsUntil - eventAction.StartDate).Days) / 7;
                                DateTime nextDate = eventAction.StartDate;

                                for (int i = 0; i <= repeats; i++) {
                                    CalendarScheduleEventViewModel eventViewModel = ViewModelLocator.Resolve<CalendarScheduleEventViewModel>();
                                    eventViewModel.InitializeAsync(new ChargeScheduleActionArgs() { ScheduleAction = new ScheduleAction(teamAction, _targetTeamMember), StartDate = nextDate });

                                    result.Add(eventViewModel);

                                    nextDate = nextDate + TimeSpan.FromDays(7);
                                }
                            }
                            else if (eventAction.RepeatingType == ActionRepeatsDataItem.NOT_REPEAT_VALUE) {
                                CalendarScheduleEventViewModel eventViewModel = ViewModelLocator.Resolve<CalendarScheduleEventViewModel>();
                                eventViewModel.InitializeAsync(new ChargeScheduleActionArgs() { ScheduleAction = new ScheduleAction(teamAction, _targetTeamMember), StartDate = eventAction.StartDate });

                                result.Add(eventViewModel);
                            }
                            else {
                                Debugger.Break();
                            }
                        }
                        else {
                            Debugger.Break();
                        }

                        return result;
                    })
                    .ToObservableCollection<CalendarScheduleEventViewModel>();
            }

            ViewDayAppointmentsPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }
    }
}
