using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Contracts;
using System;

namespace PeakMVP.Models.Rests.Responses.Friends {
    public class ConfirmFriendResponse : IResponse {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("shortId")]
        public string ShortId { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("avatars")]
        public MediaDTO[] Avatars { get; set; }

        [JsonProperty("availability")]
        public string Availability { get; set; }

        [JsonProperty("lastSeen")]
        public DateTime LastSeen { get; set; }

        [JsonProperty("parentId")]
        public long ParentId { get; set; }

        [JsonProperty("childUserName")]
        public string ChildUserName { get; set; }

        [JsonProperty("childPassword")]
        public object ChildPassword { get; set; }

        [JsonProperty("contact")]
        public ContactDTO Contact { get; set; }

        [JsonProperty("address")]
        public AddressDTO Address { get; set; }

        [JsonProperty("backgroundImage")]
        public MediaDTO BackgroundImage { get; set; }

        [JsonProperty("newMessageIds")]
        public string[] NewMessageIds { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
