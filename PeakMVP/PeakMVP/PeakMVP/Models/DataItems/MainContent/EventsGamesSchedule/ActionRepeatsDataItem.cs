using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule {
    public class ActionRepeatsDataItem {
        public static readonly string NOT_REPEAT_HEADER = "Does not repeat";
        public static readonly string DAILY_HEADER = "Daily";
        public static readonly string WEEKLY_HEADER = "Weekly";

        public static readonly string NOT_REPEAT_VALUE = "DoNotRepeat";
        public static readonly string DAILY_REPEAT_VALUE = "Daily";
        public static readonly string WEEKLY_REPEAT_VALUE = "Weekly";

        public string Header { get; set; }

        public string Value { get; set; }
    }
}
