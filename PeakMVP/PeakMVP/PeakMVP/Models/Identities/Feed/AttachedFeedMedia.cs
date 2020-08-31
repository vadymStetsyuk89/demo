using PeakMVP.Helpers;

namespace PeakMVP.Models.Identities.Feed {
    public class AttachedFeedMedia : ObservableObject {

        public long Id { get; set; }

        public string UISource64 { get; set; }

        public string SourceBase64 { get; set; }

        public string ThumbnailBase64 { get; set; }

        public MediaType MediaType { get; set; }

        public string SourceUrl { get; set; }

        public string SourceThumbnailUrl { get; set; }

        private bool _isCanBeDetached = true;
        public bool IsCanBeDetached {
            get => _isCanBeDetached;
            set => SetProperty<bool>(ref _isCanBeDetached, value);
        }
    }
}
