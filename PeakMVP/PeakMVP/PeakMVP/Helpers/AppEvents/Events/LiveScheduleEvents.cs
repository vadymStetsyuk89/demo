using PeakMVP.ViewModels.MainContent.Live;
using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class LiveScheduleEvents {
        public event EventHandler<ScheduledEventBase> ScheduledEventUpdated = delegate { };

        public void ScheduledEventUpdatedInvoke(object sender, ScheduledEventBase e) => ScheduledEventUpdated.Invoke(sender, e);
    }
}
