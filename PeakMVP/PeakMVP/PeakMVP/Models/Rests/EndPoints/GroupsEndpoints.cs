namespace PeakMVP.Models.Rests.EndPoints {
    public class GroupsEndpoints {
        private static readonly string _CREATE_GROUP_END_POINT = "api/group";

        private static readonly string _GET_GROUPS_END_POINT = "api/group";

        private static readonly string _DELETE_GROUP_BY_ID_END_POINT = "api/group/{0}/remove";

        private static readonly string _GET_GROUP_BY_ID_END_POINT = "api/group/{0}";

        private static readonly string _INVITE_MEMBERS_TO_THE_GROUP_END_POINT = "/api/group/{0}/add-members";

        internal const string GROUP_REQUEST_CONFIRM = "api/group/confirmMember";

        internal const string GROUP_REQUEST_DECLINE = "api/group/decline";

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public GroupsEndpoints(string host) {
            UpdateEndpoint(host);
        }

        public string CreateGroup { get; private set; }

        public string GetGroups { get; private set; }

        public string DeletePostById { get; private set; }

        public string GetGroupById { get; private set; }

        public string InviteMembersToTheGroup { get; private set; }

        /// <summary>
        /// Confirm group request.
        /// </summary>
        public string GroupRequestConfirmEndPoint { get; private set; }

        /// <summary>
        /// Decline group request.
        /// </summary>
        public string GroupRequestDeclineEndPoint { get; private set; }

        private void UpdateEndpoint(string host) {
            CreateGroup = string.Format("{0}{1}", host, GroupsEndpoints._CREATE_GROUP_END_POINT);
            GetGroups = string.Format("{0}{1}", host, GroupsEndpoints._GET_GROUPS_END_POINT);
            DeletePostById = string.Format("{0}{1}", host, GroupsEndpoints._DELETE_GROUP_BY_ID_END_POINT);
            GetGroupById = string.Format("{0}{1}", host, GroupsEndpoints._GET_GROUP_BY_ID_END_POINT);
            InviteMembersToTheGroup = string.Format("{0}{1}", host, GroupsEndpoints._INVITE_MEMBERS_TO_THE_GROUP_END_POINT);
            GroupRequestConfirmEndPoint = $"{host}{GROUP_REQUEST_CONFIRM}";
            GroupRequestDeclineEndPoint = $"{host}{GROUP_REQUEST_DECLINE}";
        }
    }
}
