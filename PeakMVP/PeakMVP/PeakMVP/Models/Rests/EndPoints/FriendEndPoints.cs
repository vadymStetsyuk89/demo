namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class FriendEndPoints {

        internal const string GET_ALL_FRIENDS_API_KEY = "api/friend?ProfileId={0}&ProfileType={1}";

        internal const string GET_FRIEND_BY_ID_API_KEY = "api/friend/{0}";

        internal const string ADD_FRIEND_REQUEST_API_KEY = "api/friend/request/send";

        internal const string CONFIRM_FRIEND_REQUEST_API_KEY = "api/friend/request/confirm";

        internal const string DELETE_FRIEND_REQUEST_API_KEY = "api/friend/request/delete";

        /// <summary>
        /// Get all friends.
        /// </summary>
        public string GetAllFriendsEndPoint { get; private set; }

        /// <summary>
        /// Get friend by id.
        /// </summary>
        public string GetFriendByIdEndPoint { get; private set; }

        /// <summary>
        /// Add friend request.
        /// </summary>
        public string AddFriendRequestEndPoint { get; private set; }

        /// <summary>
        /// Confirm friend request.
        /// </summary>
        public string ConfirmRequestEndPoint { get; private set; }

        /// <summary>
        /// Delete friend request
        /// </summary>
        public string DeleteRequestEndPoint { get; private set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public FriendEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetAllFriendsEndPoint = $"{host}{GET_ALL_FRIENDS_API_KEY}";
            ConfirmRequestEndPoint = $"{host}{CONFIRM_FRIEND_REQUEST_API_KEY}";
            AddFriendRequestEndPoint = $"{host}{ADD_FRIEND_REQUEST_API_KEY}";
            DeleteRequestEndPoint = $"{host}{DELETE_FRIEND_REQUEST_API_KEY}";
            GetFriendByIdEndPoint = $"{host}{GET_FRIEND_BY_ID_API_KEY}";
        }
    }
}
