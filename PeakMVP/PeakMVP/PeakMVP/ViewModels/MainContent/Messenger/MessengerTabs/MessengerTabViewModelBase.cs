using System.Threading.Tasks;
using PeakMVP.Models.DataItems.MainContent.Messenger;
using PeakMVP.ViewModels.Base;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public abstract class MessengerTabViewModelBase : NestedViewModel, IMessengerDataItem {

        private string _title;
        public string Title {
            get => _title;
            set => SetProperty<string>(ref _title, value);
        }

        private bool _canBeClosed;
        public bool CanBeClosed {
            get => _canBeClosed;
            protected set => SetProperty<bool>(ref _canBeClosed, value);
        }

        public abstract Task AskToRefreshAsync();

        public virtual bool IsSelected { get; set; }
    }
}