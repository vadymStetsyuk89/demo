using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class MessagingEvents {

        public event EventHandler<int> RefreshedUnreadMessagesForConverseClusterTab  = delegate { };

        public void RefreshedUnreadMessagesForConverseClusterTabInvoke(object sender, int unreadMessages) => RefreshedUnreadMessagesForConverseClusterTab.Invoke(sender, unreadMessages);
    }
}
