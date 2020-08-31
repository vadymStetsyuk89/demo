using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Services.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace PeakMVP.Factories.MainContent.Messaging {
    public class PossibleConversationItemsFactory : IPossibleConversationItemsFactory {

        private static readonly string _ICON_STUB_PATH = "PeakMVP.Images.ic_profile-avatar_white.png";
        private static readonly string _FAMILY_CONVERSATION_TITLE = "Family chat";
        private static readonly string _ONLINE_AVAILABILITY_VALUE = "online";
        private static readonly string _OFFLINE_AVAILABILITY_VALUE = "offline";
        private static readonly string _COMPANION_NAME_STUB = "Can't resolve name";
        private static readonly string _COMPANION_TYPE_ERROR = "Can't resolve companion type";

        private readonly IChatService _chatService;

        public PossibleConversationItemsFactory(
            IChatService chatService) {

            _chatService = chatService;
        }

        public Task<List<PossibleConverstionItem>> BuildConversationItemAsync(IEnumerable<IPossibleMessaging> data) =>
            Task<List<PossibleConverstionItem>>.Run(() => {
                List<PossibleConverstionItem> possibleConverstionItems = null;

                if (data != null) {
                    possibleConverstionItems = new List<PossibleConverstionItem>();

                    data.ForEach(iPM => possibleConverstionItems.Add(BuildConversationItem(iPM)));

                    //await possibleConverstionItems.ForEachAsync((conversationItem) => Task.Run(async () => conversationItem.UnreadMessagesCount = await _chatService.GetUnreadMessagesCountAsync(conversationItem.Companion.Id, conversationItem.MessagingCompanionType)));
                }

                return possibleConverstionItems;
            });

        public PossibleConverstionItem BuildConversationItem(IPossibleMessaging data) {
            PossibleConverstionItem converstionItem = new PossibleConverstionItem();
            converstionItem.IconPath = null;
            converstionItem.Companion = data;

            string resolvedCompanionName = _COMPANION_NAME_STUB;

            if (data is ProfileDTO profileDTO) {
                converstionItem.MessagingCompanionType = MessagingCompanionType.Friend;
                converstionItem.ProfileType = profileDTO.Type;
                converstionItem.IsAvailable = profileDTO.Availability.ToLower() == _ONLINE_AVAILABILITY_VALUE;
                converstionItem.IconPath = (profileDTO.Avatar != null ) ? profileDTO.Avatar.Url : null;

                resolvedCompanionName = string.Format("{0} {1}", profileDTO.FirstName, profileDTO.LastName);

            }
            else if (data is GroupDTO groupDTO) {
                converstionItem.MessagingCompanionType = MessagingCompanionType.Group;

                resolvedCompanionName = groupDTO.Name;
            }
            else if (data is TeamDTO teamDTO) {
                converstionItem.MessagingCompanionType = MessagingCompanionType.Team;

                resolvedCompanionName = teamDTO.Name;
            }
            else if (data is FamilyDTO) {
                converstionItem.MessagingCompanionType = MessagingCompanionType.Family;

                resolvedCompanionName = _FAMILY_CONVERSATION_TITLE;
            }
            else {
                throw new InvalidOperationException(_COMPANION_TYPE_ERROR);
            }

            converstionItem.CompanionName = resolvedCompanionName;

            return converstionItem;
        }

        //public Task<PossibleConverstionItem> BuildConversationItemAsync(IPossibleMessaging data) =>
        //    Task.Run(async () => {
        //        PossibleConverstionItem converstionItem = new PossibleConverstionItem();
        //        converstionItem.IconPath = null;
        //        converstionItem.Companion = data;

        //        string resolvedCompanionName = _COMPANION_NAME_STUB;

        //        if (data is ProfileDTO profileDTO) {
        //            converstionItem.MessagingCompanionType = MessagingCompanionType.Friend;
        //            converstionItem.ProfileType = profileDTO.Type;
        //            converstionItem.IsAvailable = profileDTO.Availability.ToLower() == _ONLINE_AVAILABILITY_VALUE;
        //            converstionItem.IconPath = (profileDTO.Avatars != null && profileDTO.Avatars.Any()) ? profileDTO.Avatars.Last().Url : null;

        //            resolvedCompanionName = string.Format("{0} {1}", profileDTO.FirstName, profileDTO.LastName);

        //        }
        //        else if (data is GroupDTO groupDTO) {
        //            converstionItem.MessagingCompanionType = MessagingCompanionType.Group;

        //            resolvedCompanionName = groupDTO.Name;
        //        }
        //        else if (data is TeamDTO teamDTO) {
        //            converstionItem.MessagingCompanionType = MessagingCompanionType.Team;

        //            resolvedCompanionName = teamDTO.Name;
        //        }
        //        else if (data is FamilyDTO) {
        //            converstionItem.MessagingCompanionType = MessagingCompanionType.Family;

        //            resolvedCompanionName = _FAMILY_CONVERSATION_TITLE;
        //        }
        //        else {
        //            throw new InvalidOperationException(_COMPANION_TYPE_ERROR);
        //        }

        //        converstionItem.CompanionName = resolvedCompanionName;
        //        converstionItem.UnreadMessagesCount = await _chatService.GetUnreadMessagesCountAsync(converstionItem.Companion.Id, converstionItem.MessagingCompanionType);

        //        return converstionItem;
        //    });
    }
}
