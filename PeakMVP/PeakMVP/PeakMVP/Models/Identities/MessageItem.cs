using PeakMVP.Helpers;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Identities {
    public sealed class MessageItem : ObservableObject {

        public MessageDTO Data { get; set; }

        public string Avatar { get; set; }

        public bool IsIncomming { get; set; }

        public void UpdateDate() {
            OnPropertyChanged("Data");
        }
    }
}
