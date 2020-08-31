using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Identities {
    public class Media {

        public ProfileMediaDTO Data { get; set; }

        public long Id { get; set; }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

        public MediaType MediaType { get; set; }
    }
}
