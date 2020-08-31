using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.Messages;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface IMessageItemFactory {
        List<MessageItem> CreateMessageItems(IEnumerable<MessageDTO> messages, PossibleConverstionItem conversation);

        MessageItem BuildSingleMessageItem(ReceivedMessageSignalArgs messageArgs, PossibleConverstionItem conversation);

        MessageItem BuildSingleMessageItem(MessageDTO message, PossibleConverstionItem conversation);

        MessageDTO BuildMessage(ReceivedMessageSignalArgs data);
    }
}
