using System.Collections.Generic;

namespace PeakMVP.Models.DataItems.MainContent.Search {
    public class FoundGroupDataItem  {

        public List<FoundSingleDataItemBase> FoundUsers { get; set; }

        public string GroupType { get; set; }

        public int UsersCount { get; set; }

        public string IconSourcePath { get; set; }

        public bool IsHaveSeparator { get; set; }
    }
}
