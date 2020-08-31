using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using System;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels {
    public class RegistrationRequestDataModel {

        [JsonProperty("user")]
        public UserDTO User { get; set; } = new UserDTO();

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("sportId")]
        public string SportId { get; set; }

        [JsonProperty("sport")]
        public string Sport { get; set; }

        [JsonProperty("age")]
        public long Age { get; set; }

        [JsonProperty("contact")]
        public ContactDTO Contact { get; set; } = new ContactDTO();

        [JsonProperty("address")]
        public AddressDTO Address { get; set; } = new AddressDTO();

        [JsonProperty("parentId")]
        public long? ParentId { get; set; }

        [JsonProperty("childrens")]
        public RegistrationRequestDataModel[] Children { get; set; } = new RegistrationRequestDataModel[] { };
    }
}
