using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System;

namespace PeakMVP.Models.Arguments.AppEventsArguments.TeamEvents {
    public class ViewMembersFromTargetTeamArgs : EventArgs {
        public TeamMember TargetTeamMember { get; set; }
    }
}
