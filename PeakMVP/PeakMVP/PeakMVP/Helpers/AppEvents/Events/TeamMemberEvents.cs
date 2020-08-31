using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class TeamMemberEvents {

        public event EventHandler AddedTeamMemberContactInfo = delegate { };
        public event EventHandler DeletedTeamMemberContactInfo = delegate { };

        public void AddedTeamMemberContactInfoInvoke(object sender, EventArgs e) => AddedTeamMemberContactInfo.Invoke(sender, e);

        public void DeletedTeamMemberContactInfoInvoke(object sender, EventArgs e) => DeletedTeamMemberContactInfo.Invoke(sender, e);
    }
}
