using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent.Messenger;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs;
using System.Collections.Generic;

namespace PeakMVP.Services.DataItems.MainContent {
    public class MessengerDataItems : IMessengerDataItems {

        private static readonly string _FRENDS_TAB_TITLE = "Friends";
        private static readonly string _GROUPS_TAB_TITLE = "Groups";
        private static readonly string _TEAMS_TAB_TITLE = "Teams";
        private static readonly string _FAMILY_TAB_TITLE = "Family chat";

        public List<IMessengerDataItem> ResolveDefaultMessengerTabItems(UserProfile userProfile) {
            List<IMessengerDataItem> messengerDataItems = null;

            if (userProfile != null) {
                messengerDataItems = new List<IMessengerDataItem>();

                if (userProfile.ProfileType == ProfileType.Coach) {
                    messengerDataItems.Add(BuildSingleMessengerTabItem<FriendsMessengerTabViewModel>(_FRENDS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<GroupsMessengerTabViewModel>(_GROUPS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<TeamsMessengerTabViewModel>(_TEAMS_TAB_TITLE));
                }
                else if (userProfile.ProfileType == ProfileType.Fan) {
                    messengerDataItems.Add(BuildSingleMessengerTabItem<FriendsMessengerTabViewModel>(_FRENDS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<GroupsMessengerTabViewModel>(_GROUPS_TAB_TITLE));
                }
                else if (userProfile.ProfileType == ProfileType.Organization) {
                    messengerDataItems.Add(BuildSingleMessengerTabItem<FriendsMessengerTabViewModel>(_FRENDS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<GroupsMessengerTabViewModel>(_GROUPS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<TeamsMessengerTabViewModel>(_TEAMS_TAB_TITLE));
                }
                else if (userProfile.ProfileType == ProfileType.Parent) {
                    messengerDataItems.Add(BuildSingleMessengerTabItem<FriendsMessengerTabViewModel>(_FRENDS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<GroupsMessengerTabViewModel>(_GROUPS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<FamilyMessengerTabViewModel>(_FAMILY_TAB_TITLE));
                }
                else if (userProfile.ProfileType == ProfileType.Player) {
                    messengerDataItems.Add(BuildSingleMessengerTabItem<FriendsMessengerTabViewModel>(_FRENDS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<GroupsMessengerTabViewModel>(_GROUPS_TAB_TITLE));
                    messengerDataItems.Add(BuildSingleMessengerTabItem<TeamsMessengerTabViewModel>(_TEAMS_TAB_TITLE));

                    if (userProfile.ParentId != null) {
                        messengerDataItems.Add(BuildSingleMessengerTabItem<FamilyMessengerTabViewModel>(_FAMILY_TAB_TITLE));
                    }
                }
            }

            return messengerDataItems;
        }

        public ConversationTabViewModel BuildConversationTab(PossibleConverstionItem data) {
            ConversationTabViewModel conversationTabViewModel = BuildSingleMessengerTabItem<ConversationTabViewModel>("");
            conversationTabViewModel.InitializeAsync(data);

            return conversationTabViewModel;
        }

        private TTargetType BuildSingleMessengerTabItem<TTargetType>(string title) where TTargetType : IMessengerDataItem {
            TTargetType messengerDataItem = ViewModelLocator.Resolve<TTargetType>();
            messengerDataItem.Title = title;

            return messengerDataItem;
        }
    }
}
