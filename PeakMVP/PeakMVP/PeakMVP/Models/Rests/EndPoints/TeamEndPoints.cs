namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class TeamEndPoints {

        internal const string CREATE_TEAM_API_KEY = "api/team/create";
        internal const string GET_MEMBERS_BY_TEAM_ID_API_KEY = "api/team/{0}/member";
        internal const string REMOVE_TEAM_ID_API_KEY = "api/team/{0}/remove";
        internal const string GET_ALL_TEAMS_API_KEY = "api/team";
        internal const string GET_TEAM_BY_ID_API_KEY = "api/team/{0}";
        internal const string SEND_TEAM_APPOINTMENT_REQUEST_API_KEY = "api/teamRequest/{0}";
        internal const string NIX_TEAM_APPOINTMENT_REQUEST_API_KEY = "api/teamRequest/{0}";
        internal const string CHECK_TEAM_APPOINTMENT_STATUS_API_KEY = "api/teamRequest/{0}/status";
        internal const string GET_TEAM_REQUESTS_API_KEY = "api/teamRequest/{0}";
        internal const string APPROVE_TEAM_REQUESTS_API_KEY = "api/teamRequest/{0}/approve/{1}";
        internal const string REJECT_TEAM_REQUESTS_API_KEY = "api/teamRequest/{0}/reject/{1}";
        internal const string TEAM_END_PARTNERSHIP_WITH_ORGANIZATION_API_KEY = "api/team/{0}/endPartnership";
        private readonly string REQUEST_TO_JOIN_TO_TEAM = "api/teamRequest/{0}";
        private readonly string STATUS_FOR_REQUEST_TO_JOIN_TO_TEAM = "api/teamRequest/{0}/status";
        private readonly string INVITE_EXTERNAL_MEMBER_TO_THE_TEAM = "api/invite/profile";
        private readonly string EXTERNAL_INVITES_BY_TEAM_ID = "api/invite/team/{0}/profile";
        private readonly string RESEND_EXTERNAL_MEMBER_INVITE = "api/invite/team/{0}/profile";

        public string RequestToJoinToTeam { get; private set; }

        public string StatusRequestToJoinToTeam { get; private set; }

        public string GetTeamById { get; private set; }

        /// <summary>
        /// Reject request.
        /// </summary>
        public string RejectTeamRequestRndpoint { get; private set; }

        /// <summary>
        /// Approve request.
        /// </summary>
        public string ApproveTeamRequestRndpoint { get; private set; }

        /// <summary>
        /// Create team.
        /// </summary>
        public string CreateTeamEndPoint { get; private set; }

        /// <summary>
        /// Get members by team id
        /// </summary>
        public string GetMembersByTeamIdEndPoint { get; private set; }

        /// <summary>
        /// Remove team by team id
        /// </summary>
        public string RemoveTeamByIdEndPoint { get; private set; }

        /// <summary>
        /// Send team appointment request
        /// </summary>
        public string SendTeamAppointmentRequest { get; private set; }

        /// <summary>
        /// Cancel team appointment request
        /// </summary>
        public string NixTeamAppointmentRequest { get; private set; }

        /// <summary>
        /// Check team appointment status
        /// </summary>
        public string CheckTeamAppointmentStatusApiKey { get; private set; }

        /// <summary>
        /// Get all teams
        /// </summary>
        public string GetAllTeams { get; set; }

        /// <summary>
        /// Get team requests by id.
        /// </summary>
        public string GetTeamRequestsEndPoint { get; internal set; }

        /// <summary>
        /// Ends partner between team and it't relative organization
        /// </summary>
        public string TeamEndPartnershipWithOrganizationApiKey { get; private set; }

        public string InviteExternalMemberToTheTeam { get; private set; }

        public string ExternalInvitesByTeamId { get; private set; }

        public string ResendExternalMemberInvite { get; private set; }

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="host"></param>
        public TeamEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            ApproveTeamRequestRndpoint = $"{host}{APPROVE_TEAM_REQUESTS_API_KEY}";
            RejectTeamRequestRndpoint = $"{host}{REJECT_TEAM_REQUESTS_API_KEY}";
            CreateTeamEndPoint = $"{host}{CREATE_TEAM_API_KEY}";
            GetTeamRequestsEndPoint = $"{host}{GET_TEAM_REQUESTS_API_KEY}";
            GetMembersByTeamIdEndPoint = string.Format("{0}{1}", host, GET_MEMBERS_BY_TEAM_ID_API_KEY);
            RemoveTeamByIdEndPoint = string.Format("{0}{1}", host, REMOVE_TEAM_ID_API_KEY);
            GetAllTeams = string.Format("{0}{1}", host, GET_ALL_TEAMS_API_KEY);
            SendTeamAppointmentRequest = string.Format("{0}{1}", host, SEND_TEAM_APPOINTMENT_REQUEST_API_KEY);
            NixTeamAppointmentRequest = string.Format("{0}{1}", host, NIX_TEAM_APPOINTMENT_REQUEST_API_KEY);
            CheckTeamAppointmentStatusApiKey = string.Format("{0}{1}", host, CHECK_TEAM_APPOINTMENT_STATUS_API_KEY);
            TeamEndPartnershipWithOrganizationApiKey = string.Format("{0}{1}", host, TEAM_END_PARTNERSHIP_WITH_ORGANIZATION_API_KEY);
            RequestToJoinToTeam = string.Format("{0}{1}", host, REQUEST_TO_JOIN_TO_TEAM);
            StatusRequestToJoinToTeam = string.Format("{0}{1}", host, STATUS_FOR_REQUEST_TO_JOIN_TO_TEAM);
            GetTeamById = string.Format("{0}{1}", host, GET_TEAM_BY_ID_API_KEY);
            InviteExternalMemberToTheTeam = string.Format("{0}{1}", host, INVITE_EXTERNAL_MEMBER_TO_THE_TEAM);
            ExternalInvitesByTeamId = string.Format("{0}{1}", host, EXTERNAL_INVITES_BY_TEAM_ID);
            ResendExternalMemberInvite = string.Format("{0}{1}", host, RESEND_EXTERNAL_MEMBER_INVITE);
        }
    }
}
