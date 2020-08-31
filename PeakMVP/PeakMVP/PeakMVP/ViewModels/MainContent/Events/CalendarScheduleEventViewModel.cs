using PeakMVP.Controls.ScheduleCalendar;
using PeakMVP.Helpers;
using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Identities.Scheduling;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Events.Arguments;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class CalendarScheduleEventViewModel : NestedViewModel, IScheduleEvent {

        public ICommand EditCommand => new Command(async () => {
            ContentPageBaseViewModel relativeBasePage = NavigationService.LastPageViewModel as ContentPageBaseViewModel;
            Guid busyKey;

            if (relativeBasePage != null) {
                busyKey = Guid.NewGuid();
                relativeBasePage.SetBusy(busyKey, true);
                await Task.Delay(AppConsts.DELAY_STUB);
            }

            if (ScheduleAction.ScheduledAction is GameDTO gameAction) {
                await NavigationService.NavigateToAsync<EditGameViewModel>();
                await NavigationService.LastPageViewModel.InitializeAsync(ScheduleAction.RelativeTeamMember);
                await NavigationService.LastPageViewModel.InitializeAsync(gameAction);
            }
            else if (ScheduleAction.ScheduledAction is EventDTO eventAction) {
                await NavigationService.NavigateToAsync<EditEventViewModel>();
                await NavigationService.LastPageViewModel.InitializeAsync(ScheduleAction.RelativeTeamMember);
                await NavigationService.LastPageViewModel.InitializeAsync(eventAction);
            }
            else {
                Debugger.Break();
            }

            if (relativeBasePage != null) {
                relativeBasePage.SetBusy(busyKey, false);
            }
        });

        public Color Color { get; private set; }

        public ScheduleAction ScheduleAction { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public string Subject { get; private set; }

        public EventRepeating Repeating { get; private set; }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ChargeScheduleActionArgs scheduleActionArgs) {
                try {
                    ScheduleAction = scheduleActionArgs.ScheduleAction;

                    StartDate = scheduleActionArgs.StartDate;
                    EndDate = scheduleActionArgs.StartDate;
                    Subject = ScheduleAction.Header;

                    if (ScheduleAction.ScheduledAction is EventDTO eventAction) {
                        Color = (Color)Application.Current.Resources["GreenSuccessColor"];

                        if (eventAction.RepeatingType == ActionRepeatsDataItem.DAILY_REPEAT_VALUE) {
                            Repeating = EventRepeating.Daily;
                        }
                        else if (eventAction.RepeatingType == ActionRepeatsDataItem.WEEKLY_REPEAT_VALUE) {
                            Repeating = EventRepeating.Weekly;
                        }
                        else {
                            Repeating = EventRepeating.Once;
                        }
                    }
                    else if (ScheduleAction.ScheduledAction is GameDTO gameAction) {
                        Color = (Color)Application.Current.Resources["BlueColor"];
                    }
                    else {
                        Debugger.Break();
                    }
                }
                catch (Exception exc) {
                    Debugger.Break();
                    DialogService.ToastAsync(exc.Message);
                }
            }

            return base.InitializeAsync(navigationData);
        }
    }
}

