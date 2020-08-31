using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent.Messenger;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Services.DataItems.MainContent {
    public interface IMessengerDataItems {

        List<IMessengerDataItem> ResolveDefaultMessengerTabItems(UserProfile userProfile);

        ConversationTabViewModel BuildConversationTab(PossibleConverstionItem data);
    }
}
