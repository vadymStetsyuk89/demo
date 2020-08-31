using Newtonsoft.Json;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.ViewModels.MainContent.Invites;

namespace PeakMVP.Models.Rests.DTOs {
    public class GroupDTO : IPossibleMessaging, IInviteTo {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("ownerId")]
        public long OwnerId { get; set; }

        [JsonProperty("owner")]
        public ProfileDTO Owner { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("members")]
        public MemberDTO[] Members { get; set; }
    }
}
