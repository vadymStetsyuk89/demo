using PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.ViewModels.MainContent.Messenger.Arguments {
    public class CloseConversationTabArgs {
        public ConversationTabViewModel TabToClose { get; set; }
    }
}
