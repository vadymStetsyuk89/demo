namespace PeakMVP.Helpers.AppEvents.Events {
    public class FriendEvents {

        /// <summary>
        /// Occurs when new friend was completely added to friend list.
        /// Subscription type pattern: <object sender, long newFriendId>
        /// </summary>
        public string FriendshipInviteAccepted => "FriendEvents.friendship_invite_accepted";

        /// <summary>
        /// Occurs when you decline friendship invite
        /// Subscription type pattern: <object sender, long possibleFriendId>
        /// </summary>
        public string FriendshipInviteDeclined => "TeamEvents.friendship_invite_declined";

        /// <summary>
        /// Occurs when friend was removed from friend list. 
        /// Subscription type pattern: <object sender, long oldFriendId>
        /// </summary>
        public string FriendDeleted => "FriendEvents.friend_deleted";
    }
}
