namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class ProfileEndpoints {

        internal const string GET_PROFILE_API_KEY = "api/profile";

        internal const string GET_PROFILE_BY_SHORT_ID_API_KEY = "api/profile/{0}";

        internal const string GET_PROFILE_BY_ID_API_KEY = "api/profile/id/{0}";

        private static readonly string _SET_PROFILE_AVATAR = "api/profile/avatar";

        private static readonly string _SET_PROFILE_SETTINGS = "api/profile";

        private static readonly string _SET_APP_BACKGROUND_IMAGE = "api/profile/background-image";

        private static readonly string _OUTER_PROFILE_EDIT = "api/team-member/profileEdit";

        /// <summary>
        /// Get profile.
        /// </summary>
        public string GetProfileEndPoints { get; private set; }

        /// <summary>
        /// Get profile by short id.
        /// </summary>
        public string GetProfileByShortIdEndPoints { get; private set; }

        /// <summary>
        /// Get profile by id.
        /// </summary>
        public string GetProfileByIdEndPoints { get; private set; }

        /// <summary>
        /// Set profile avatar end point
        /// </summary>
        public string SetProfileAvatarEndPoint { get; private set; }

        /// <summary>
        /// Set profile settings end point
        /// </summary>
        public string SetProfileSettingsEndPoint { get; private set; }

        /// <summary>
        /// Set app background image end point
        /// </summary>
        public string SetAppBackgroundImageEndPoint { get; private set; }

        public string OuterProfileEditEndPoint { get; private set; }

        public ProfileEndpoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetProfileEndPoints = $"{host}{GET_PROFILE_API_KEY}";
            GetProfileByShortIdEndPoints = $"{host}{GET_PROFILE_BY_SHORT_ID_API_KEY}";
            GetProfileByIdEndPoints = $"{host}{GET_PROFILE_BY_ID_API_KEY}";
            SetProfileAvatarEndPoint = string.Format("{0}{1}", host, _SET_PROFILE_AVATAR);
            SetProfileSettingsEndPoint = string.Format("{0}{1}", host, _SET_PROFILE_SETTINGS);
            SetAppBackgroundImageEndPoint = string.Format("{0}{1}", host, _SET_APP_BACKGROUND_IMAGE);
            OuterProfileEditEndPoint = string.Format("{0}{1}", host, _OUTER_PROFILE_EDIT);
        }
    }
}
