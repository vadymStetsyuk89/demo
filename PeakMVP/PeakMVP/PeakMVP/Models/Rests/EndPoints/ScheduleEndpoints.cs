namespace PeakMVP.Models.Rests.EndPoints {
    public class ScheduleEndpoints {

        internal const string _CREATE_NEW_OPPONENT = "api/opponent/create";
        internal const string _CREATE_NEW_LOCATION = "api/location/create";
        internal const string _CREATE_NEW_GAME = "api/game/create";
        internal const string _DELETE_GAME = "api/game/{0}/remove";
        internal const string _UPDATE_GAME = "api/game/{0}/update";
        internal const string _DELETE_EVENT = "api/event/{0}/remove";
        internal const string _CREATE_NEW_EVENT = "api/event/create";
        internal const string _UPDATE_EVENT = "api/event/{0}/update";

        public ScheduleEndpoints(string host) {
            UpdateEndpoint(host);
        }

        public string CreateNewOpponent { get; private set; }

        public string CreateNewLocation { get; private set; }

        public string CreateNewGame { get; private set; }

        public string DeleteGame { get; private set; }

        public string UpdateGame { get; private set; }

        public string DeleteEvent { get; private set; }

        public string CreateNewEvent { get; private set; }

        public string UpdateEvent { get; private set; }

        private void UpdateEndpoint(string host) {
            CreateNewOpponent = string.Format("{0}{1}", host, _CREATE_NEW_OPPONENT);
            CreateNewLocation = string.Format("{0}{1}", host, _CREATE_NEW_LOCATION);
            CreateNewGame = string.Format("{0}{1}", host, _CREATE_NEW_GAME);
            DeleteGame = string.Format("{0}{1}", host, _DELETE_GAME);
            UpdateGame = string.Format("{0}{1}", host, _UPDATE_GAME);
            DeleteEvent = string.Format("{0}{1}", host, _DELETE_EVENT);
            CreateNewEvent = string.Format("{0}{1}", host, _CREATE_NEW_EVENT);
            UpdateEvent = string.Format("{0}{1}", host, _UPDATE_EVENT);
        }
    }
}
