using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Sockets.StateArgs {
    public class ChangedGroupsSignalArgs {
        [JsonProperty("data")]
        public GroupDTO[] Data { get; set; }

        [JsonProperty("nextPageId")]
        public object NextPageId { get; set; }
    }
}
