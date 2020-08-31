namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class SportEndPoints {

        internal const string GET_SPORTS_API_KEY = "api/sport";

        public string GetSportsEndPoint { get; private set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        public SportEndPoints(string host) {
            GetSportsEndPoint = $"{host}{GET_SPORTS_API_KEY}";
        }
    }
}
