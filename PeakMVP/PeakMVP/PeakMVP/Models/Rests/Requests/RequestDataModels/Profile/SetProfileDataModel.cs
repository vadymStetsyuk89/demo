using Newtonsoft.Json;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Profile {
    public class SetProfileDataModel {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }

        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }

        [JsonProperty("confirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("mySports")]
        public string MySports { get; set; }

        [JsonProperty("profileId")]
        public long ProfileId { get; set; }

        [JsonProperty("profileType")]
        public string ProfileType { get; set; }
    }
}
