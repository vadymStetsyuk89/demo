using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;

namespace PeakMVP.Models.Rests.Responses.Scheduling {
    public class UpdateEventResponse : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("teamId")]
        public long TeamId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortLabel")]
        public string ShortLabel { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("isTimeTbd")]
        public bool IsTimeTbd { get; set; }

        [JsonProperty("repeatingType")]
        public string RepeatingType { get; set; }

        [JsonProperty("repeatsUntil")]
        public DateTime RepeatsUntil { get; set; }

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

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
