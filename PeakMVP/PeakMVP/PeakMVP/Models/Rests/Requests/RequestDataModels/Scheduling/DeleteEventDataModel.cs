using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class DeleteEventDataModel {

        [JsonProperty("eventId")]
        public long EventId { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
