using PeakMVP.Models.Rests.DTOs;
using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class MediaEvents {

        //public event EventHandler<ProfileMediaDTO> NewMediaAdded = delegate { };
        public event EventHandler NewMediaAdded = delegate { };
        public event EventHandler<ProfileMediaDTO> MediaDeleted = delegate { };

        public void OnNewMediaAdded() {
            NewMediaAdded(this, new EventArgs());
        }
        //public void NewMediaAddedInvoke(object sender, ProfileMediaDTO neMedia) => NewMediaAdded.Invoke(sender, neMedia);
        public void MediaDeletedInvoke(object sender, ProfileMediaDTO deletedMedia) => MediaDeleted.Invoke(sender, deletedMedia);
    }
}
