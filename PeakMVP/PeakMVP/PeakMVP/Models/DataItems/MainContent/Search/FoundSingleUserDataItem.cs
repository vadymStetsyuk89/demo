using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.DataItems.MainContent.Search {
    public class FoundSingleUserDataItem : FoundSingleDataItemBase {

        public ProfileDTO UserProfile { get; set; }
    }
}
