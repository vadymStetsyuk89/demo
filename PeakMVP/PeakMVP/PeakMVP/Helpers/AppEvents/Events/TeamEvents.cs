using PeakMVP.Models.Arguments.AppEventsArguments.TeamEvents;
using PeakMVP.Models.Rests.DTOs;
using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class TeamEvents {

        public event EventHandler<TeamDTO> NewTeamCreated = delegate { };
        public event EventHandler<TeamDTO> TeamDeleted = delegate { };
        public event EventHandler<ViewMembersFromTargetTeamArgs> ViewMembersFromTargetTeam = delegate { };

        public void NewTeamCreatedInvoke(object sender, TeamDTO team) => NewTeamCreated(sender, team);
        public void TeamDeletedInvoke(object sender, TeamDTO team) => TeamDeleted(sender, team);
        public void ViewMembersFromTargetTeamInvoke(object sender, ViewMembersFromTargetTeamArgs args) => ViewMembersFromTargetTeam(sender, args);

        /// <summary>
        /// Occurs when you (as team manager) allow external request to join the team
        /// Subscription type pattern: <object sender>
        /// </summary>
        public string RequestAcceptedForNewTeamMember => "TeamEvents.request_accepted_for_new_team_member";

        /// <summary>
        /// Occurs when you (as team manager) decline external request to join the team
        /// Subscription type pattern: <object sender>
        /// </summary>
        public string RequestDeclinedForNewTeamMember => "TeamEvents.request_declined_for_new_team_member";

        /// <summary>
        /// Occurs when you accept invite to join to the team.
        /// Subscription type pattern: <object sender, long teamId>
        /// </summary>
        public string InviteAccepted => "TeamEvents.invite_accepted";

        /// <summary>
        /// Occurs when you decline invite to join to the team.
        /// Subscription type pattern: <object sender, long teamId>
        /// </summary>
        public string InviteDeclined => "TeamEvents.invite_declined";

        /// <summary>
        /// Occurs when we stopped partnership with organization for our team
        /// Subscription type pattern: <object sender, TeamDTO targetTeam>
        /// </summary>
        public string PartnershipWithOrganizationStopped => "TeamEvents.partnership_with_organization_stopped";
    }
}


//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace PeakMVP.Helpers.AppEvents.Events {
//    public class TeamEvents {

//        /// <summary>
//        /// Occurs when new team was created.
//        /// Subscription type pattern: <object sender, TeamDTO createdTeam>
//        /// </summary>
//        public string NewTeamCreated => "TeamEvents.new_team_created";

//        /// <summary>
//        /// Occurs when team was deleted. 
//        /// Subscription type pattern: <object sender, TeamDTO deletedTeam>
//        /// </summary>
//        public string TeamDeleted => "TeamEvents.team_deleted";

//        /// <summary>
//        /// Occurs when you (as team manager) allow external request to join the team
//        /// Subscription type pattern: <object sender>
//        /// </summary>
//        public string RequestAcceptedForNewTeamMember => "TeamEvents.request_accepted_for_new_team_member";

//        /// <summary>
//        /// Occurs when you (as team manager) decline external request to join the team
//        /// Subscription type pattern: <object sender>
//        /// </summary>
//        public string RequestDeclinedForNewTeamMember => "TeamEvents.request_declined_for_new_team_member";

//        /// <summary>
//        /// Occurs when you accept invite to join to the team.
//        /// Subscription type pattern: <object sender, long teamId>
//        /// </summary>
//        public string InviteAccepted => "TeamEvents.invite_accepted";

//        /// <summary>
//        /// Occurs when you decline invite to join to the team.
//        /// Subscription type pattern: <object sender, long teamId>
//        /// </summary>
//        public string InviteDeclined => "TeamEvents.invite_declined";

//        /// <summary>
//        /// Occurs when we stopped partnership with organization for our team
//        /// Subscription type pattern: <object sender, TeamDTO targetTeam>
//        /// </summary>
//        public string PartnershipWithOrganizationStopped => "TeamEvents.partnership_with_organization_stopped";
//    }
//}

