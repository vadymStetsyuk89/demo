using PeakMVP.ViewModels.Base;
using PeakMVP.Views.CompoundedViews.MainContent.Live.ScheduleEventInfo;
using System;

namespace PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo {
    public class InterestAndAvailabilityViewModel : NestedViewModel, IVisualFiguring {

        public static readonly string INTEREST_AND_AVAILABILITY_TITLE = "Availability";

        public Type RelativeViewType => typeof(InterestAndAvailabilityView);

        public string TabHeader => INTEREST_AND_AVAILABILITY_TITLE.ToUpper();
    }
}
