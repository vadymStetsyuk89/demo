using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public abstract class TeamActionBaseDTO {

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("isTimeTbd")]
        public bool IsTimeTbd { get; set; }
        
        [JsonProperty("location")]
        public LocationDTO Location { get; set; }

        [JsonProperty("locationDetails")]
        public string LocationDetails { get; set; }

        [JsonProperty("durationInMinutes")]
        public long DurationInMinutes { get; set; }

        [JsonProperty("isCanceled")]
        public bool IsCanceled { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("assignments")]
        public AssignmentDTO[] Assignments { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
