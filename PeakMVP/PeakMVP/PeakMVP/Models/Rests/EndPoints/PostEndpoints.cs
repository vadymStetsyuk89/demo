namespace PeakMVP.Models.Rests.EndPoints {
    public sealed class PostEndpoints {

        internal const string GET_POSTS_API_KEY = "api/post?Author={0}&PageId={1}&ProfileId={2}&ProfileType={3}";

        internal const string DELETE_POST_BY_POST_ID_API_KEY = "api/post/{0}/remove";

        internal const string PUBLISH_NEW_COMMENT_API_KEY = "api/post/comment";

        internal const string PUBLISH_NEW_POST_API_KEY = "api/post";

        //internal const string EDIT_POST_API_KEY = "api/post/edit";
        internal const string EDIT_POST_API_KEY = "api/post/edit/{0}";

        /// <summary>
        /// Get profile.
        /// </summary>
        public string GetPostsPoint { get; private set; }

        /// <summary>
        /// Publish new post endpoint
        /// </summary>
        public string PublishNewPostEndPoint { get; private set; }

        /// <summary>
        /// Edit post endpoint
        /// </summary>
        public string EditPostEndPoint { get; private set; }

        /// <summary>
        /// Publish new comment endpoint
        /// </summary>
        public string PublishCommentEndPoint { get; private set; }

        /// <summary>
        /// Delete post by post id endpoint
        /// </summary>
        public string DeletePostByPostIdEndPoint { get; private set; }

        /// <summary>
        ///     CTOR().
        /// </summary>
        /// <param name="host"></param>
        public PostEndpoints(string host) {
            UpdateEndpoint(host);
        }

        private void UpdateEndpoint(string host) {
            GetPostsPoint = $"{host}{GET_POSTS_API_KEY}";
            PublishNewPostEndPoint = string.Format("{0}{1}", host, PUBLISH_NEW_POST_API_KEY);
            PublishCommentEndPoint = $"{host}{PUBLISH_NEW_COMMENT_API_KEY}";
            DeletePostByPostIdEndPoint = string.Format("{0}{1}", host, DELETE_POST_BY_POST_ID_API_KEY);
            EditPostEndPoint = string.Format("{0}{1}", host, EDIT_POST_API_KEY);
        }
    }
}
