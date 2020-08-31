namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class TeamMemberEndPoints {

        internal const string GET_TEAM_MEMBERS_API_KEY = "api/team-member";
        internal const string GET_TEAM_MEMBERS_BY_MEMBER_ID_API_KEY = "api/team-member?memberId={0}";
        private static readonly string ADD_CONTACT_INFO_API_KEY = "api/team-member/{0}/contacts";
        private static readonly string EDIT_CONTACT_INFO_API_KEY = "api/team-member/{0}/contacts";
        private static readonly string DELETE_CONTACT_INFO_API_KEY = "api/team-member/{0}/contacts";

        /// <summary>
        /// Get team members.
        /// </summary>
        public string GetTeamMembersEndPoint { get; private set; }

        public string GetTeamMembersByMemberIdEndPoint { get; private set; }

        public string AddContactInfoEndpoint { get; private set; }

        public string EditContactInfoEndpoint { get; private set; }

        public string DeleteContactInfoEndpoint { get; private set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public TeamMemberEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetTeamMembersEndPoint = $"{host}{GET_TEAM_MEMBERS_API_KEY}";
            AddContactInfoEndpoint = string.Format("{0}{1}", host, ADD_CONTACT_INFO_API_KEY);
            EditContactInfoEndpoint = string.Format("{0}{1}", host, EDIT_CONTACT_INFO_API_KEY);
            DeleteContactInfoEndpoint = string.Format("{0}{1}", host, DELETE_CONTACT_INFO_API_KEY);
            GetTeamMembersByMemberIdEndPoint = string.Format("{0}{1}", host, GET_TEAM_MEMBERS_BY_MEMBER_ID_API_KEY);
        }
    }
}
