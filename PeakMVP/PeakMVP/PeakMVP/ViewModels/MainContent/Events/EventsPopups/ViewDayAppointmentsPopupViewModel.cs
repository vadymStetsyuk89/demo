using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.Events.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.Events.Popups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.ViewModels.MainContent.Events.EventsPopups {
    public class ViewDayAppointmentsPopupViewModel : PopupBaseViewModel , IEventManagementDependent {

        private bool _isEventsManagementAvailable = (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization);
        public bool IsEventsManagementAvailable {
            get => _isEventsManagementAvailable;
            private set => SetProperty<bool>(ref _isEventsManagementAvailable, value);
        }

        private string _title;
        public string Title {
            get => _title;
            private set => SetProperty<string>(ref _title, value);
        }

        private List<CalendarScheduleEventViewModel> _scheduleEvents;
        public List<CalendarScheduleEventViewModel> ScheduleEvents {
            get => _scheduleEvents;
            private set => SetProperty<List<CalendarScheduleEventViewModel>>(ref _scheduleEvents, value);
        }

        public override Type RelativeViewType => typeof(ViewDayAppointmentsPopupView);

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.CalendarScheduleDateSelected += OnScheduleEventsCalendarScheduleDateSelected;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.CalendarScheduleDateSelected -= OnScheduleEventsCalendarScheduleDateSelected;
        }

        private void OnScheduleEventsCalendarScheduleDateSelected(object sender, ScheduledCalendarDateSelectedArgs e) {
            Title = string.Format("{0:MMM dd yyyy}", e.Date);
            ScheduleEvents = e.Context.ToList<CalendarScheduleEventViewModel>();

            ShowPopupCommand.Execute(null);
        }
    }
}
