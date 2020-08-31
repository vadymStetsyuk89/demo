using PeakMVP.ViewModels.Base;
using PeakMVP.Views.CompoundedViews.MainContent.Live.ScheduleEventInfo;
using System;

namespace PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo {
    public class EventStatisticsViewModel : NestedViewModel, IVisualFiguring {

        public static readonly string STATISTICS_TITLE = "Statistics";

        public Type RelativeViewType => typeof(EventStatisticsView);

        public string TabHeader => STATISTICS_TITLE.ToUpper();
    }
}
