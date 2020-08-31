using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent;
using PeakMVP.Models.DataItems.MainContent.MenuOptions;
using PeakMVP.ViewModels.MainContent;
using PeakMVP.ViewModels.MainContent.Friends;
using PeakMVP.ViewModels.MainContent.Groups;
using PeakMVP.ViewModels.MainContent.Messenger;
using PeakMVP.ViewModels.MainContent.ProfileSettings;
using System.Collections.Generic;
using System.Diagnostics;

namespace PeakMVP.Services.DataItems.MainContent {
    public class MenuOptionsDataItems : IMenuOptionsDataItems {

        private static readonly string FEED_OPTION_TITLE = "Feed";
        private static readonly string FRIENDS_OPTION_TITLE = "Friends";
        private static readonly string MESSENGER_OPTION_TITLE = "Messenger";
        private static readonly string GROUPS_OPTION_TITLE = "Groups";
        private static readonly string CLIPBOARDS_OPTION_TITLE = "Clipboards";
        private static readonly string SCHEDULE_OPTION_TITLE = "Schedule";
        private static readonly string SETTINGS_OPTION_TITLE = "Settings";
        private static readonly string LOG_OUT_OPTION_TITLE = "Log out";
        private static readonly string CHILD_IMPERSONATE_LOG_BACK_OPTION_TITLE = "Log back to parent";

        public IEnumerable<MenuOptionDataItem> ResolveMenuOptions(ProfileType profileType) {
            IEnumerable<MenuOptionDataItem> options = null;

            switch (profileType) {
                case ProfileType.Fan:
                    options = new MenuOptionDataItem[] {
                        new MenuOptionDataItem() {
                            Title = FEED_OPTION_TITLE,
                            TargetViewModelType = typeof(DashboardViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = MESSENGER_OPTION_TITLE,
                            TargetViewModelType = typeof(MessengerViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = FRIENDS_OPTION_TITLE,
                            TargetViewModelType = typeof(FriendsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = GROUPS_OPTION_TITLE,
                            TargetViewModelType = typeof(GroupsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = SETTINGS_OPTION_TITLE,
                            TargetViewModelType = typeof(SettingsViewModel)
                        },
                        new LogOutMenuOptionDataItem() {
                            Title = LOG_OUT_OPTION_TITLE
                        }
                    };
                    break;
                case ProfileType.Player:
                    List<MenuOptionDataItem> menuOptions = new List<MenuOptionDataItem>() {
                        new MenuOptionDataItem() {
                            Title = FEED_OPTION_TITLE,
                            TargetViewModelType = typeof(DashboardViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title  = MESSENGER_OPTION_TITLE,
                            TargetViewModelType = typeof(MessengerViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = FRIENDS_OPTION_TITLE,
                            TargetViewModelType = typeof(FriendsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = GROUPS_OPTION_TITLE,
                            TargetViewModelType = typeof(GroupsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = SETTINGS_OPTION_TITLE,
                            TargetViewModelType = typeof(SettingsViewModel)
                        },
                        new LogOutMenuOptionDataItem() {
                            Title = LOG_OUT_OPTION_TITLE
                        }
                    };

                    if (GlobalSettings.Instance.UserProfile.ImpersonateProfile != null) {
                        menuOptions.Add(new ImpersonateLogBackMenuOptionDataItem() {
                            Title = CHILD_IMPERSONATE_LOG_BACK_OPTION_TITLE
                        });
                    }

                    options = menuOptions.ToArray();
                    break;
                case ProfileType.Parent:
                    options = new MenuOptionDataItem[] {
                        new MenuOptionDataItem() {
                            Title = FEED_OPTION_TITLE,
                            TargetViewModelType = typeof(DashboardViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = MESSENGER_OPTION_TITLE,
                            TargetViewModelType = typeof(MessengerViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = FRIENDS_OPTION_TITLE,
                            TargetViewModelType = typeof(FriendsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = GROUPS_OPTION_TITLE,
                            TargetViewModelType = typeof(GroupsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = SETTINGS_OPTION_TITLE,
                            TargetViewModelType = typeof(SettingsViewModel)
                        },
                        new LogOutMenuOptionDataItem() {
                            Title = LOG_OUT_OPTION_TITLE
                        }
                    };
                    break;
                case ProfileType.Organization:
                    options = new MenuOptionDataItem[] {
                        new MenuOptionDataItem() {
                            Title = FEED_OPTION_TITLE,
                            TargetViewModelType = typeof(DashboardViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = MESSENGER_OPTION_TITLE,
                            TargetViewModelType = typeof(MessengerViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = FRIENDS_OPTION_TITLE,
                            TargetViewModelType = typeof(FriendsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = GROUPS_OPTION_TITLE,
                            TargetViewModelType = typeof(GroupsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = CLIPBOARDS_OPTION_TITLE,
                            TargetViewModelType = typeof(object)
                        },                     
                        new MenuOptionDataItem() {
                            Title = SETTINGS_OPTION_TITLE,
                            TargetViewModelType = typeof(SettingsViewModel)
                        },
                        new LogOutMenuOptionDataItem() {
                            Title = LOG_OUT_OPTION_TITLE
                        }
                    };
                    break;
                case ProfileType.Coach:
                    options = new MenuOptionDataItem[] {
                        new MenuOptionDataItem() {
                            Title = FEED_OPTION_TITLE,
                            TargetViewModelType = typeof(DashboardViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = MESSENGER_OPTION_TITLE,
                            TargetViewModelType = typeof(MessengerViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = FRIENDS_OPTION_TITLE,
                            TargetViewModelType = typeof(FriendsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = GROUPS_OPTION_TITLE,
                            TargetViewModelType = typeof(GroupsViewModel)
                        },
                        new MenuOptionDataItem() {
                            Title = CLIPBOARDS_OPTION_TITLE,
                            TargetViewModelType = typeof(object)
                        },
                        new MenuOptionDataItem() {
                            Title = SCHEDULE_OPTION_TITLE,
                            TargetViewModelType = typeof(object)
                        },
                        new MenuOptionDataItem() {
                            Title = SETTINGS_OPTION_TITLE,
                            TargetViewModelType = typeof(SettingsViewModel)
                        },
                        new LogOutMenuOptionDataItem() {
                            Title = LOG_OUT_OPTION_TITLE
                        }
                    };
                    break;
                default:
                    Debugger.Break();
                    options = new MenuOptionDataItem[] { };
                    break;
            }

            return options;
        }
    }
}
