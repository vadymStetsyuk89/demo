using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Helpers.EqualityComparers;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Sockets.Messages;
using PeakMVP.Services.Chat;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.ViewModels.MainContent.Messenger.Arguments;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs {
    public abstract class MessagingClusterTabBaseViewModel : MessengerTabViewModelBase, IResolvePossibleConversations, IMessagesCounterTab {

        private readonly IPossibleConversationItemsFactory _possibleConversationItemsFactory;
        protected readonly IMessagesService _messagesService;
        protected readonly IGroupMessagingService _groupMessagingService;
        protected readonly IChatService _chatService;

        protected readonly GenericEqualityComparer<IPossibleMessaging> _possibleConversationGroupByIdComparer = new GenericEqualityComparer<IPossibleMessaging>((group) => group.Id);

        public MessagingClusterTabBaseViewModel(
            IPossibleConversationItemsFactory possibleConversationItemsFactory,
            IChatService chatService,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService) {

            _possibleConversationItemsFactory = possibleConversationItemsFactory;
            _chatService = chatService;
            _messagesService = messagesService;
            _groupMessagingService = groupMessagingService;
        }

        public ICommand StartConversationCommand => new Command(async (object parameter) =>
            await NavigationService.LastPageViewModel.InitializeAsync(new StartConversationArgs() { TargetCompanion = (PossibleConverstionItem)parameter }));

        private ObservableCollection<PossibleConverstionItem> _possibleConversations = new ObservableCollection<PossibleConverstionItem>();
        public ObservableCollection<PossibleConverstionItem> PossibleConversations {
            get => _possibleConversations;
            private set => SetProperty<ObservableCollection<PossibleConverstionItem>>(ref _possibleConversations, value);
        }

        private int _unreadMessages;
        public int UnreadMessages {
            get => _unreadMessages;
            protected set{
                if (value != _unreadMessages) {
                    SetProperty<int>(ref _unreadMessages, value);
                }
            }
        }

        public abstract Task ResolvePossibleConversationsAsync();

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            _messagesService.FriendMessageReceived += OnCommonSignalMessageReceived;
            _groupMessagingService.TeamMessageReceived += OnCommonSignalMessageReceived;
            _groupMessagingService.GroupMessageReceived += OnCommonSignalMessageReceived;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            _messagesService.FriendMessageReceived -= OnCommonSignalMessageReceived;
            _groupMessagingService.TeamMessageReceived -= OnCommonSignalMessageReceived;
            _groupMessagingService.GroupMessageReceived -= OnCommonSignalMessageReceived;
        }

        protected async void ChargePossibleConverstionItems(IEnumerable<IPossibleMessaging> possibleMessagings) {
            ObservableCollection<PossibleConverstionItem> resolvedPossibleConversations = (possibleMessagings != null) ? new ObservableCollection<PossibleConverstionItem>(await _possibleConversationItemsFactory.BuildConversationItemAsync(possibleMessagings)) : new ObservableCollection<PossibleConverstionItem>();

            Device.BeginInvokeOnMainThread(() => {
                PossibleConversations = resolvedPossibleConversations;
                TODO();
            });
        }

        private void OnCommonSignalMessageReceived(object sender, ReceivedMessageSignalArgs e) {
            long companionId = 0;

            if (long.TryParse(e.FromId, out companionId)) {
                _groupMessagingService.RefreshUnreadMessagesFromGroupingChatsAsync(new CancellationTokenSource());
            }
            else {
                Debugger.Break();
            }
        }

        protected abstract void TODO();
    }
}
