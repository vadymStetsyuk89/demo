using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Groups {
    public class InviteMembersToTheGroupDataModel {
        [JsonProperty("shortIds")]
        public string[] ShortIds { get; set; }

        [JsonProperty("membersIds")]
        public long[] MembersIds { get; set; }

        [JsonProperty("groupId")]
        public long GroupId { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
