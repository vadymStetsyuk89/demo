using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.Models.Rests.DTOs.TeamMembership {
    public class TeamMemberContactInfo {

        private List<ContactPhoneNumber> _phones;
        [JsonProperty("phones")]
        public List<ContactPhoneNumber> Phones {
            get => _phones;
            set {
                _phones = value;

                if (_phones != null && _phones.Any()) {
                    FirstPhoneNumber = _phones.First().Phone;
                }
                else {
                    FirstPhoneNumber = null;
                }
            }
        }

        [JsonProperty("guardianType")]
        public string GuardianType { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zipCode")]
        public int ZipCode { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonIgnore]
        public string FirstPhoneNumber { get; private set; }
    }
}
