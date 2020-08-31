using Newtonsoft.Json;
using PeakMVP.Models.Sockets.GroupMessaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Sockets.Messages {
    public class ResolvedUnreadFriendMessagesSignalArgs {

        [JsonProperty("counts")]
        public FriendUnreadMessages[] Counts { get; set; }
    }

    public class TODO {
        [JsonProperty("profileId")]
        public long ProfileId { get; set; }
    }
}
