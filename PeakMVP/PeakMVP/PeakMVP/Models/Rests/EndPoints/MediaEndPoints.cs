namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class MediaEndPoints {

        internal const string USER_CONTENT_API_KEY = "/user-content/{0}";

        /// <summary>
        /// Get profile.
        /// </summary>
        public string GetMediaEndPoints { get; private set; }

        /// <summary>
        ///     CTOR().
        /// </summary>
        /// <param name="host"></param>
        public MediaEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetMediaEndPoints = $"{host}{USER_CONTENT_API_KEY}";
        }
    }
}
