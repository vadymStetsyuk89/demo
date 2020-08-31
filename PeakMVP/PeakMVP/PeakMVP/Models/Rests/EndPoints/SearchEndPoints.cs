namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class SearchEndPoints {

        internal const string SEARCH_API_KEY = "api/search?Phrase={0}&Type={1}";

        internal const string SEARCH_FRIENDS_API_KEY = "api/search/friends?Phrase={0}&Type={1}&ProfileId={2}&ProfileType={3}";

        public string SimpleSearchEndPoints { get; private set; }

        public string SearchFriendsEndpoints { get; private set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        public SearchEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            SimpleSearchEndPoints = $"{host}{SEARCH_API_KEY}";
            SearchFriendsEndpoints = $"{host}{SEARCH_FRIENDS_API_KEY}";
        }
    }
}
