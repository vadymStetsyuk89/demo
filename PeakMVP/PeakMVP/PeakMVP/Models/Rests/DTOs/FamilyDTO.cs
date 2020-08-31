using Newtonsoft.Json;
using PeakMVP.Models.Identities.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.DTOs {
    public class FamilyDTO : IPossibleMessaging {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("parentId")]
        public long ParentId { get; set; }

        [JsonProperty("members")]
        public ProfileDTO[] Members { get; set; }
    }
}
