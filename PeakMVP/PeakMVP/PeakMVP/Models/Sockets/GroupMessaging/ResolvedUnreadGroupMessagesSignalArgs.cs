using Newtonsoft.Json;
using PeakMVP.Models.Identities.Messenger;

namespace PeakMVP.Models.Sockets.GroupMessaging {
    public class ResolvedUnreadGroupMessagesSignalArgs {

        [JsonProperty("groupChatCounters")]
        public GroupUnreadMessages[] GroupChatCounters { get; set; }

        [JsonProperty("teamChatCounters")]
        public TeamUnreadMessages[] TeamChatCounters { get; set; }

        [JsonProperty("familyChatCounter")]
        public FamilyUnreadMessages FamilyChatCounter { get; set; }
    }

    public class GroupUnreadMessages : UnreadMessagesBase, IPossibleMessaging {
        [JsonProperty("groupId")]
        public long Id { get; set; }
    }

    public class TeamUnreadMessages : UnreadMessagesBase, IPossibleMessaging {
        [JsonProperty("teamId")]
        public long Id { get; set; }
    }

    public class FamilyUnreadMessages : UnreadMessagesBase, IPossibleMessaging {
        [JsonProperty("familyId")]
        public long Id { get; set; }
    }

    public class FriendUnreadMessages : UnreadMessagesBase, IPossibleMessaging {
        [JsonProperty("profileId")]
        public long Id { get; set; }
    }

    public abstract class UnreadMessagesBase {
        [JsonProperty("count")]
        public long Count { get; set; }
    }
}
