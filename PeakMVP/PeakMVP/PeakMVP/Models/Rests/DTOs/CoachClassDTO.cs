using Newtonsoft.Json;
using System;

namespace PeakMVP.Models.Rests.DTOs {
    public class CoachClassDTO {
        [JsonProperty("about", NullValueHandling = NullValueHandling.Ignore)]
        public string About { get; set; }

        [JsonProperty("mySports", NullValueHandling = NullValueHandling.Ignore)]
        public string MySports { get; set; }

        [JsonProperty("childrens", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Childrens { get; set; }

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

        [JsonProperty("isEmailConfirmed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsEmailConfirmed { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("avatars")]
        public BackgroundImageDTO[] Avatars { get; set; }

        [JsonProperty("availability")]
        public string Availability { get; set; }

        [JsonProperty("lastSeen")]
        public DateTimeOffset LastSeen { get; set; }

        [JsonProperty("parentId")]
        public long ParentId { get; set; }

        [JsonProperty("childUserName")]
        public string ChildUserName { get; set; }

        [JsonProperty("childPassword")]
        public string ChildPassword { get; set; }

        [JsonProperty("contact")]
        public ContactDTO Contact { get; set; }

        [JsonProperty("address")]
        public AddressDTO Address { get; set; }

        [JsonProperty("backgroundImage")]
        public BackgroundImageDTO BackgroundImage { get; set; }

        [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]
        public ProfileMediaDTO[] Media { get; set; }
    }
}
