using Newtonsoft.Json;

namespace PeakMVP.Models.Sockets.GroupMessaging {

    public class MarkFamilyMessageAsReadedSignalArgs : MarkGroupingMessageAsReadedSignalArgsBase {
        [JsonProperty("familyId")]
        public long FamilyId { get; set; }
    }

    public class MarkGroupMessageAsReadedSignalArgs : MarkGroupingMessageAsReadedSignalArgsBase {
        [JsonProperty("groupId")]
        public long GroupId { get; set; }
    }

    public class MarkTeamMessageAsReadedSignalArgs : MarkGroupingMessageAsReadedSignalArgsBase {
        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }

    public abstract class MarkGroupingMessageAsReadedSignalArgsBase {
        [JsonProperty("messageId")]
        public long MessageId { get; set; }
    }
}
