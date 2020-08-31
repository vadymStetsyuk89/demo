using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.Messages;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.Factories.MainContent {
    public sealed class MessageItemFactory : IMessageItemFactory {

        public List<MessageItem> CreateMessageItems(IEnumerable<MessageDTO> messages, PossibleConverstionItem conversation) {
            List<MessageItem> messageItems = new List<MessageItem>();

            foreach (MessageDTO message in messages) {
                messageItems.Add(BuildSingleMessageItem(message, conversation));
            }

            return messageItems;
        }

        public MessageItem BuildSingleMessageItem(ReceivedMessageSignalArgs messageArgs, PossibleConverstionItem conversation) {
            MessageDTO message = BuildMessage(messageArgs);

            return BuildSingleMessageItem(message, conversation);
        }

        public MessageItem BuildSingleMessageItem(MessageDTO message, PossibleConverstionItem conversation) {
            MessageItem messageItem = new MessageItem() {
                IsIncomming = message.FromId == GlobalSettings.Instance.UserProfile.Id,
                Data = message
            };

            if (messageItem.IsIncomming) {
                messageItem.Avatar = (GlobalSettings.Instance.UserProfile.Avatar?.Url != null)
                    ? GlobalSettings.Instance.UserProfile.Avatar?.Url
                    : null;
            }
            else {
                messageItem.Avatar = ResolveCompanionAvatar(message, conversation);
            }

            return messageItem;
        }

        public MessageDTO BuildMessage(ReceivedMessageSignalArgs data) =>
            new MessageDTO() {
                DeliveryTrackingId = data.DeliveryTrackingId,
                FromId = long.Parse(data.FromId),
                Id = long.Parse(data.Id),
                Seen = data.Seen.HasValue ? data.Seen.Value : false,
                Text = data.Text,
                Time = data.Time,
                ToId = long.Parse(data.ToId)
            };

        private string ResolveCompanionAvatar(MessageDTO message, PossibleConverstionItem conversation) {

            string avatarPath = null;

            ProfileDTO profileDTO = null;

            switch (conversation.MessagingCompanionType) {
                case MessagingCompanionType.Family:
                    profileDTO = ((FamilyDTO)conversation.Companion).Members?.FirstOrDefault(m => m.Id == message.FromId);
                    break;
                case MessagingCompanionType.Friend:
                    profileDTO = ((ProfileDTO)conversation.Companion);
                    break;
                case MessagingCompanionType.Team:
                    profileDTO = ((TeamDTO)conversation.Companion).Members?.FirstOrDefault(m => m.Member.Id == message.FromId)?.Member;

                    if (profileDTO == null) {
                        profileDTO = ((TeamDTO)conversation.Companion).CreatedBy;
                    }
                    break;
                case MessagingCompanionType.Group:
                    profileDTO = ((GroupDTO)conversation.Companion).Members?.FirstOrDefault(m => m.Profile.Id == message.FromId).Profile;
                    break;
            }

            if (profileDTO != null && profileDTO.Avatar != null) {
                avatarPath = profileDTO.Avatar.Url;
            }

            return avatarPath;
        }
    }
}
