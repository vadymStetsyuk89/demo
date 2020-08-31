namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class ProfileMediaEndPoints {

        internal const string GET_PROFILE_MEDIA_KEY = "api/profile/media?profileId={0}";
        internal const string ADD_PROFILE_MEDIA_KEY = "api/profile/media";
        internal const string REMOVE_PROFILE_MEDIA_BY_ID_KEY = "api/profile/media/remove/{0}";
        internal const string UPLOAD_MEDIA = "api/media";
        internal const string DELETE_MEDIA = "api/media/{0}";

        public string UploadMedia { get; private set; }

        public string DeleteMedia { get; private set; }

        /// <summary>
        /// Get profile media.
        /// </summary>
        public string GetProfileMediaEndPoints { get; private set; }

        /// <summary>
        /// Add profile media
        /// </summary>
        public string AddProfileMediaEndPoint { get; set; }

        /// <summary>
        /// Remove profile media
        /// </summary>
        public string RemoveProfileMediaByIdEndPoint { get; set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public ProfileMediaEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetProfileMediaEndPoints = $"{host}{GET_PROFILE_MEDIA_KEY}";
            AddProfileMediaEndPoint = string.Format("{0}{1}", host, ADD_PROFILE_MEDIA_KEY);
            RemoveProfileMediaByIdEndPoint = string.Format("{0}{1}", host, REMOVE_PROFILE_MEDIA_BY_ID_KEY);
            UploadMedia = string.Format("{0}{1}", host, UPLOAD_MEDIA);
            DeleteMedia = string.Format("{0}{1}", host, DELETE_MEDIA);
        }
    }
}
