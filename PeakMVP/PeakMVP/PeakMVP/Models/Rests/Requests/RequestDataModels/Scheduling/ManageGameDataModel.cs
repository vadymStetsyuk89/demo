using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Scheduling {
    public class ManageGameDataModel
    {
        [JsonProperty("notifyTeam")]
        public bool NotifyTeam { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("isTimeTbd")]
        public bool IsTimeTbd { get; set; }

        [JsonProperty("opponentId")]
        public long OpponentId { get; set; }

        [JsonProperty("locationId")]
        public long LocationId { get; set; }

        [JsonProperty("assignments")]
        public AssignmentDataModel[] Assignments { get; set; }

        [JsonProperty("locationDetails")]
        public string LocationDetails { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uniform")]
        public string Uniform { get; set; }

        [JsonProperty("durationInMinutes")]
        public long DurationInMinutes { get; set; }

        [JsonProperty("minutesToArriveEarly")]
        public long MinutesToArriveEarly { get; set; }

        [JsonProperty("notForStandings")]
        public bool NotForStandings { get; set; }

        [JsonProperty("isCanceled")]
        public bool IsCanceled { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }
    }
}
