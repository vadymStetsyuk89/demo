using System;

namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class ChatEndPoints {

        internal const string GET_MESSAGES_API_KEY = "api/chat?PageId={0}&FriendId={1}&ProfileId={2}&ProfileType={3}";

        internal const string GET_GROUP_CHAT_API_KEY = "api/chat/group?GroupId={0}";

        internal const string GET_TEAM_CHAT_API_KEY = "api/chat/team?TeamId={0}";

        internal const string GET_FAMILY_CHAT_API_KEY = "api/chat/family?ProfileId={0}";

        /// <summary>
        /// Get messages.
        /// </summary>
        public string GetMessagesEndPoint { get; private set; }

        /// <summary>
        /// Get croup chat.
        /// </summary>
        public string GetGroupChatEndPoint { get; private set; }

        /// <summary>
        /// Get team chat.
        /// </summary>
        public string GetTeamChatEndPoint { get; private set; }

        /// <summary>
        /// Get family chat.
        /// </summary>
        public string GetFamilyChatEndPoint { get; internal set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public ChatEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetMessagesEndPoint = $"{host}{GET_MESSAGES_API_KEY}";
            GetGroupChatEndPoint = $"{host}{GET_GROUP_CHAT_API_KEY}";
            GetTeamChatEndPoint = $"{host}{GET_TEAM_CHAT_API_KEY}";
            GetFamilyChatEndPoint = $"{host}{GET_FAMILY_CHAT_API_KEY}";
        }
    }
}
