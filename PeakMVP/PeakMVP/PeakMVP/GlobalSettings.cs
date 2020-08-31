using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Helpers.AppEvents;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.EndPoints;

namespace PeakMVP {
    public class GlobalSettings {

        public static GlobalSettings Instance { get; } = new GlobalSettings();

        public AppEndpoints Endpoints => BaseSingleton<AppEndpoints>.Instance;

        public AzureMobileCenter AzureMobileCenter => BaseSingleton<AzureMobileCenter>.Instance;

        public AppMessagingEvents AppMessagingEvents => BaseSingleton<AppMessagingEvents>.Instance;

        public GlobalSettings() {
            string jsonUserProfile = Settings.UserProfile;

            UserProfile = string.IsNullOrEmpty(jsonUserProfile) ? new UserProfile() : JsonConvert.DeserializeObject<UserProfile>(jsonUserProfile);
        }

        /// <summary>
        /// User profile instance
        /// </summary>
        static UserProfile _userProfile;
        public UserProfile UserProfile {
            get => _userProfile;
            private set => _userProfile = value;
        }
    }
}