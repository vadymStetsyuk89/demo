using PeakMVP.Models.Identities.Messenger;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeakMVP.Factories.MainContent.Messaging {
    public interface IPossibleConversationItemsFactory {

        PossibleConverstionItem BuildConversationItem(IPossibleMessaging data);

        Task<List<PossibleConverstionItem>> BuildConversationItemAsync(IEnumerable<IPossibleMessaging> data);
    }
}
