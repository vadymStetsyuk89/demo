using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class ProfileSettingsEvents {

        public event EventHandler AppBackgroundImageChanged = delegate { };
        public event EventHandler<ProfileUpdatedArgs> ProfileUpdated = delegate { };
        public event EventHandler OuterProfileInfoUpdated = delegate { };
        public event EventHandler ChildrenUpdated = delegate { };

        public void AppBackgroundImageChangedInvoke(object sender, EventArgs args) => AppBackgroundImageChanged(sender, args);
        public void ProfileUpdatedInvoke(object sender, ProfileUpdatedArgs args) => ProfileUpdated(sender, args);
        public void OuterProfileInfoUpdatedInvoke(object sender) => OuterProfileInfoUpdated(sender, null);
        public void ChildrenUpdatedInvoke(object sender) => ChildrenUpdated(sender, null);

        /// <summary>
        /// Occurs when user change `app background image`.
        /// Subscription type pattern: <object sender>
        /// </summary>
        //public string AppBackgroundImageChanged => "ProfileSettingsEvents.app_background_image_changed";
    }
}
