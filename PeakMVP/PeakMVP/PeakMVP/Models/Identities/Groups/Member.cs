using PeakMVP.Helpers;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Identities.Groups {
    public class Member : ObservableObject {

        private string _status;
        public string Status {
            get => _status;
            set => SetProperty<string>(ref _status, value);
        }

        private bool _isGroupOwner;
        public bool IsGroupOwner {
            get => _isGroupOwner;
            set => SetProperty<bool>(ref _isGroupOwner, value);
        }

        private ProfileDTO _profile;
        public ProfileDTO Profile {
            get => _profile;
            set => SetProperty<ProfileDTO>(ref _profile, value);
        }

        private bool _isCanBeRemoved;
        public bool IsCanBeRemoved {
            get => _isCanBeRemoved;
            set => SetProperty<bool>(ref _isCanBeRemoved, value);
        }
    }
}
