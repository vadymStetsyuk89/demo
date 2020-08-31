using PeakMVP.Models.Arguments.AppEventsArguments.Posts;
using PeakMVP.Models.Arguments.InitializeArguments.Post;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class PostEvents {

        public event EventHandler<EditPostArgs> RequestToStartPostEditing = delegate { };
        public event EventHandler<DeletePostArgs> RequestToStartPostDeleting = delegate { };
        public event EventHandler<PostContentViewModel> StartWatchingPostComments = delegate { };
        public event EventHandler<AttachExternalMediaToNewPostArgs> AttachMediaTotheNewPostOffer = delegate { };

        public void RequestToStartPostEditingInvoke(object sender, EditPostArgs args) => RequestToStartPostEditing(sender, args);
        public void RequestToStartPostDeletingInvoke(object sender, DeletePostArgs args) => RequestToStartPostDeleting(sender, args);
        public void StartWatchingPostCommentsInvoke(object sender, PostContentViewModel args) => StartWatchingPostComments(sender, args);
        public void AttachMediaTotheNewPostOfferInvoke(object sender, AttachExternalMediaToNewPostArgs args) => AttachMediaTotheNewPostOffer(sender, args);
    }
}
