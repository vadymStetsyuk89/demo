using PeakMVP.Helpers;

namespace PeakMVP.Models.Identities.Messenger {
    public class PossibleConverstionItem : ObservableObject {

        public string IconPath { get; set; }

        public string ProfileType { get; set; }

        public bool IsAvailable { get; set; }

        public MessagingCompanionType MessagingCompanionType { get; set; }

        public string CompanionName { get; set; }

        private int _unreadMessagesCount;
        public int UnreadMessagesCount {
            get => _unreadMessagesCount;
            set => SetProperty<int>(ref _unreadMessagesCount, value);
        }

        public IPossibleMessaging Companion { get; set; }
    }
}
