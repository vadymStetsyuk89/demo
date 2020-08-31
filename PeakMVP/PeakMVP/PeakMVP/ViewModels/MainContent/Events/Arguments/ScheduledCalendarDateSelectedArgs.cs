using System;
using System.Collections.Generic;

namespace PeakMVP.ViewModels.MainContent.Events.Arguments {
    public class ScheduledCalendarDateSelectedArgs {

        public DateTime Date { get; set; }

        public IEnumerable<CalendarScheduleEventViewModel> Context { get; set; }
    }
}
