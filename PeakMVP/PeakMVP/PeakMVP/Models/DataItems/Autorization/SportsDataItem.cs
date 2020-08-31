using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.DataItems.Autorization {
    public class SportsDataItem {

        public SportDTO Data { get; set; }

        public string Description { get; set; }

        public SportsType SportsType { get; set; }

        public string Name { get; set; }

        public long Id { get; internal set; }
    }
}
