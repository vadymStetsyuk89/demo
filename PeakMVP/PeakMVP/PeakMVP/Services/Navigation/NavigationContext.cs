using PeakMVP.Models.AppNavigation;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent;
using PeakMVP.ViewModels.MainContent.Albums;
using PeakMVP.ViewModels.MainContent.Character;
using PeakMVP.ViewModels.MainContent.Events;
using PeakMVP.ViewModels.MainContent.Friends;
using PeakMVP.ViewModels.MainContent.Groups;
using PeakMVP.ViewModels.MainContent.Live;
using PeakMVP.ViewModels.MainContent.Members;
using PeakMVP.ViewModels.MainContent.Messenger;
using PeakMVP.ViewModels.MainContent.ProfileSettings;
using PeakMVP.ViewModels.MainContent.Teams;
using System.Collections.Generic;

namespace PeakMVP.Services.Navigation {
    public class NavigationContext : INavigationContext {

        public static readonly string SOCIAL_MODE_ICON_PATH = "resource://PeakMVP.Images.Svg.ic_earth.svg";
        public static readonly string SPORT_MODE_ICON_PATH = "resource://PeakMVP.Images.Svg.ic_trophy.svg";

        public static readonly string FEED_TITLE = "Feed";
        public static readonly string PROFILE_TITLE = "Profile";
        public static readonly string MESSAGES_TITLE = "Messages";
        public static readonly string FRIENDS_TITLE = "Friends";
        public static readonly string GROUPS_TITLE = "Groups";
        public static readonly string ALBUMS_TITLE = "Albums";
        public static readonly string SETTINGS_TITLE = "Settings";
        public static readonly string TEAMS_TITLE = "Teams";
        public static readonly string MEMBERS_TITLE = "Members";
        public static readonly string EVENTS_TITLE = "Events";
        public static readonly string LIVE_TITLE = "Live";

        public static readonly string FEED_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_earth.svg";
        public static readonly string PROFILE_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_user.svg";
        public static readonly string MESSAGES_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_message.svg";
        public static readonly string FRIENDS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_hearrt.svg";
        public static readonly string GROUPS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_users.svg";
        public static readonly string ALBUMS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_image.svg";
        public static readonly string SETTINGS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_gear.svg";
        public static readonly string TEAMS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_home.svg";
        public static readonly string MEMBERS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_users.svg";
        public static readonly string EVENTS_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_calendar.svg";
        public static readonly string START_IMAGE_PATH = "resource://PeakMVP.Images.Svg.ic_stopwatch.svg";

        public List<NavigationModeBase> BuildModes(ProfileType profileType) {

            IBottomBarTab messengerTab = ViewModelLocator.Resolve<MessengerViewModel>();
            IBottomBarTab settingsTab = ViewModelLocator.Resolve<SettingsViewModel>();

            List<IBottomBarTab> socialModeItems = new List<IBottomBarTab>() {
                ViewModelLocator.Resolve<CharacterViewModel>(),
                ViewModelLocator.Resolve<DashboardViewModel>(),
                messengerTab,
                ViewModelLocator.Resolve<FriendsViewModel>(),
                ViewModelLocator.Resolve<GroupsViewModel>(),
                ViewModelLocator.Resolve<AlbumsViewModel>(),
                settingsTab
            };

            List<IBottomBarTab> sportModeItems = new List<IBottomBarTab>();

            if (profileType == ProfileType.Coach || profileType == ProfileType.Organization) {
                sportModeItems.Add(ViewModelLocator.Resolve<TeamsViewModel>());
                sportModeItems.Add(ViewModelLocator.Resolve<MembersViewModel>());
                sportModeItems.Add(ViewModelLocator.Resolve<EventsViewModel>());
                sportModeItems.Add(ViewModelLocator.Resolve<LiveViewModel>());
                sportModeItems.Add(messengerTab);
                sportModeItems.Add(settingsTab);
            }
            else if (profileType == ProfileType.Parent || profileType == ProfileType.Player) {
                sportModeItems.Add(ViewModelLocator.Resolve<TeamsViewModel>());
                sportModeItems.Add(ViewModelLocator.Resolve<MembersViewModel>());
                sportModeItems.Add(ViewModelLocator.Resolve<EventsViewModel>());
                sportModeItems.Add(messengerTab);
                sportModeItems.Add(settingsTab);
            }

            return new List<NavigationModeBase>() { new SocialMode(socialModeItems), new SportMode(sportModeItems) };
        }
    }
}
