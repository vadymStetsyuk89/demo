using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Contracts;
using System.Collections.Generic;

namespace PeakMVP.Models.Rests.Responses.Teams {
    public class GetTeamByIdResponse : IResponse {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sport")]
        public string Sport { get; set; }

        [JsonProperty("coachId")]
        public long? CoachId { get; set; }

        [JsonProperty("coach")]
        public ProfileDTO Coach { get; set; }

        [JsonProperty("organizationId")]
        public long? OrganizationId { get; set; }

        [JsonProperty("organization")]
        public ProfileDTO Organization { get; set; }

        [JsonProperty("locations")]
        public List<LocationDTO> Locations { get; set; }

        [JsonProperty("opponents")]
        public List<OpponentDTO> Opponents { get; set; }

        [JsonProperty("games")]
        public List<GameDTO> Games { get; set; }

        [JsonProperty("events")]
        public List<EventDTO> Events { get; set; }

        [JsonProperty("owner")]
        public ProfileDTO Owner { get; set; }

        [JsonProperty("createdBy")]
        public ProfileDTO CreatedBy { get; set; }

        [JsonProperty("members")]
        public TeamMember[] Members { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
