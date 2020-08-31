using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Controls.ScheduleCalendar {
    public interface IScheduleEvent {

        Color Color { get; }

        DateTime StartDate { get; }

        DateTime EndDate { get; }

        string Subject { get; }

        EventRepeating Repeating { get; }
    }
}
