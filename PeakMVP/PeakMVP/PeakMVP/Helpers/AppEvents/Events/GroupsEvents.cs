using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class GroupsEvents {

        /// <summary>
        /// Occurs when new group was created. 
        /// Subscription type pattern: <object sender, GroupDTO newGroup>
        /// </summary>
        public string NewGroupCreated => "GroupsEvents.new_group_created";

        /// <summary>
        /// Occurs when group was deleted.
        /// Subscription type pattern: <object sender, GroupDTO deletedGroup>
        /// </summary>
        public string GroupDeleted => "GroupsEvents.group_deleted";

        /// <summary>
        /// Occurs when you accept invite to join to the group.
        /// Subscription type pattern: <object sender, long groupId>
        /// </summary>
        public string InviteAccepted => "GroupsEvents.invite_accepted";

        /// <summary>
        /// Occurs when you decline invite to join to the group.
        /// Subscription type pattern: <object sender, long groupId>
        /// </summary>
        public string InviteDeclined => "GroupsEvents.invite_declined";
    }
}
