using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class ManageEventDataModel {

        [JsonProperty("shortLabel")]
        public string ShortLabel { get; set; }

        [JsonProperty("repeatsUntil")]
        public DateTime RepeatsUntil { get; set; }

        [JsonProperty("repeatingType")]
        public string RepeatingType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notifyTeam")]
        public bool NotifyTeam { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("isTimeTbd")]
        public bool IsTimeTbd { get; set; }

        [JsonProperty("locationId")]
        public long LocationId { get; set; }

        [JsonProperty("assignments")]
        public AssignmentDataModel[] Assignments { get; set; }

        [JsonProperty("locationDetails")]
        public string LocationDetails { get; set; }

        [JsonProperty("durationInMinutes")]
        public long DurationInMinutes { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
