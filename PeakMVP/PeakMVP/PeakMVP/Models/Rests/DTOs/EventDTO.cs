using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public class EventDTO: TeamActionBaseDTO {

        public static readonly string NO_REPEAT_REPITITION_TYPE = "DoNotRepeat";
        public static readonly string DAILY_REPITITION_TYPE = "Daily";
        public static readonly string WEEKLY_REPITITION_TYPE = "Weekly";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortLabel")]
        public string ShortLabel { get; set; }

        [JsonProperty("repeatingType")]
        public string RepeatingType { get; set; }

        [JsonProperty("repeatsUntil")]
        public DateTime RepeatsUntil { get; set; }
    }
}
