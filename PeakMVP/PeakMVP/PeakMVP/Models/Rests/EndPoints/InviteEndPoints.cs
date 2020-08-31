using System;

namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class InviteEndPoints {

        internal const string GET_INVITES_API_KEY = "api/invite";

        internal const string INVITE_MEMBER_TO_TEAM_BY_ID_API_KEY = "api/invite/team/{0}";

        internal const string TEAM_INVITE_CONFIRM = "api/invite/team/{0}";
        
        internal const string TEAM_INVITE_REJECT = "api/invite/team/{0}/reject";

        /// <summary>
        /// Get invites end point.
        /// </summary>
        public string GetInvitesEndPoint { get; private set; }

        /// <summary>
        /// Invites member to the team by team id
        /// </summary>
        public string InviteMemberToTeamByIdEndPoint { get; private set; }

        /// <summary>
        /// Team invite confirm.
        /// </summary>
        public string TeamInviteConfirmEndPoint { get; private set; }

        /// <summary>
        /// Team invite reject.
        /// </summary>
        public string TeamInviteRejectEndPoint { get; internal set; }

        public InviteEndPoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetInvitesEndPoint = $"{host}{GET_INVITES_API_KEY}";
            InviteMemberToTeamByIdEndPoint = string.Format("{0}{1}", host, INVITE_MEMBER_TO_TEAM_BY_ID_API_KEY);
            TeamInviteConfirmEndPoint = $"{host}{TEAM_INVITE_CONFIRM}";
            TeamInviteRejectEndPoint = $"{host}{TEAM_INVITE_REJECT}";
        }
    }
}
