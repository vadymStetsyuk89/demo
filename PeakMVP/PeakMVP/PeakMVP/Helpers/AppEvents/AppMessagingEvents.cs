using PeakMVP.Helpers.AppEvents.Events;

namespace PeakMVP.Helpers.AppEvents {
    public class AppMessagingEvents {

        public GroupsEvents GroupsEvents { get; private set; } = BaseSingleton<GroupsEvents>.Instance;

        public TeamEvents TeamEvents { get; private set; } = BaseSingleton<TeamEvents>.Instance;

        public FriendEvents FriendEvents { get; private set; } = BaseSingleton<FriendEvents>.Instance;

        public PostEvents PostEvents { get; private set; } = BaseSingleton<PostEvents>.Instance;

        public ProfileSettingsEvents ProfileSettingsEvents { get; private set; } = BaseSingleton<ProfileSettingsEvents>.Instance;

        public ScheduleEvents ScheduleEvents { get; private set; } = BaseSingleton<ScheduleEvents>.Instance;

        public MediaEvents MediaEvents { get; private set; } = BaseSingleton<MediaEvents>.Instance;

        public MessagingEvents MessagingEvents { get; private set; } = BaseSingleton<MessagingEvents>.Instance;

        public TeamMemberEvents TeamMemberEvents { get; private set; } = BaseSingleton<TeamMemberEvents>.Instance;

        public LiveScheduleEvents LiveScheduleEvents { get; private set; } = BaseSingleton<LiveScheduleEvents>.Instance;
    }
}
