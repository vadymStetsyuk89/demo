using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Services.Posts;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace PeakMVP.Factories.MainContent {
    public class ContentViewModelFactory : IContentViewModelFactory {

        private static readonly string _BUILD_COMMENT_ERROR = "Exception occured while creating comment";
        private static readonly string _BUILD_POST_ERROR = "Exception occured while creating post";

        public List<PostContentViewModel> CreatePostContentViewModels(IEnumerable<PostDTO> foundedPosts) {
            try {
                List<PostContentViewModel> postContentViewModels = new List<PostContentViewModel>();

                foreach (var post in foundedPosts) {
                    postContentViewModels.Add(new PostContentViewModel(ViewModelLocator.Resolve<IPostService>(), ViewModelLocator.Resolve<IContentViewModelFactory>()) {
                        Data = post,
                        Id = post.Id,
                        IsAnyMedia = post.Media.Any(),
                        DisplayName = post.Author.DisplayName,
                        Medias = post.Media.Any() ? SetMedia(post.Media) : null,
                        Avatar = (post.Author.Avatar?.Url != null)
                           ? string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, post.Author.Avatar?.Url)
                           : null,
                        Text = post.Text,
                        CountComments = post.Comments.Any() ? post.Comments.Length : 0,
                        Comments = post.Comments.Any() ? CreateComments(post.Comments) : null,
                        PublishTime = post.PublishTime.ToLocalTime(),
                        IsEditable = post.Author.Id.Equals(GlobalSettings.Instance.UserProfile.Id),
                        //IsEditable = (post.Author.Id.Equals(GlobalSettings.Instance.UserProfile.Id) && post.PostPolicyType == PostPolicyType.Public.ToString()),
                        CanBeDeleted = post.Author.Id.Equals(GlobalSettings.Instance.UserProfile.Id),
                        PublicityScopeName = post.PostPolicyName
                    });
                }

                return postContentViewModels;
            }
            catch (Exception exc) {
                Debugger.Break();
                throw new InvalidOperationException(_BUILD_POST_ERROR, exc);
            }
        }

        public CommentContentViewModel CreateComment(CommentDTO source, UserProfile author) {
            try {
                source.CreationTime = source.CreationTime.ToLocalTime();

                CommentContentViewModel commentResult = BuildCommentWithoutAuthorDetails(
                    source,
                    author.DisplayName,
                    (author.Avatar?.Url != null)
                       ? string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, author.Avatar?.Url)
                       : null);

                return commentResult;
            }
            catch (Exception exc) {
                Debugger.Break();
                throw new InvalidOperationException(_BUILD_COMMENT_ERROR, exc);
            }
        }

        public ObservableCollection<CommentContentViewModel> CreateComments(IEnumerable<CommentDTO> source) {
            try {
                ObservableCollection<CommentContentViewModel> comments = new ObservableCollection<CommentContentViewModel>();

                foreach (CommentDTO comment in source) {
                    CommentContentViewModel commentResult = BuildCommentWithoutAuthorDetails(
                        comment,
                        comment.Author.DisplayName,
                        (comment.Author.Avatar?.Url != null)
                           ? string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, comment.Author.Avatar?.Url)
                           : null);

                    comments.Add(commentResult);
                }

                return comments;
            }
            catch (Exception exc) {
                Debugger.Break();
                throw new InvalidOperationException(_BUILD_COMMENT_ERROR, exc);
            }
        }

        private ObservableCollection<Media> SetMedia(IEnumerable<ProfileMediaDTO> source) {
            ObservableCollection<Media> medias = new ObservableCollection<Media>();
            foreach (ProfileMediaDTO mediaDTO in source) {

                //mediaDTO.Category = mediaDTO.Mime == ProfileMediaService.MIME_VIDEO_TYPE ?
                //    ProfileMediaService.VIDEO_MEDIA_CATEGORY : ProfileMediaService.IMAGE_MEDIA_CATEGORY;

                ///
                /// TODO: Temporary, remove when pictures will contain valid thumbnail URL
                /// 
                if (mediaDTO.Mime == ProfileMediaService.MIME_IMAGE_TYPE) {
                    mediaDTO.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, mediaDTO.Url);
                }
                else {
                    mediaDTO.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, mediaDTO.ThumbnailUrl);
                }
                mediaDTO.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, mediaDTO.Url);

                ///
                /// TODO: resolve MediaType for new Media in another robust way
                /// 
                medias.Add(new Media {
                    Data = mediaDTO,
                    Id = mediaDTO.Id,
                    ThumbnailUrl = mediaDTO.ThumbnailUrl,
                    Url = mediaDTO.Url,
                    MediaType = (mediaDTO.Mime == ProfileMediaService.MIME_IMAGE_TYPE) ? MediaType.Picture : MediaType.Video
                });
            }

            return medias;
        }

        private CommentContentViewModel BuildCommentWithoutAuthorDetails(CommentDTO source, string authorDisplayName, string authorAvatarUrl) =>
            new CommentContentViewModel {
                DisplayName = authorDisplayName,
                Avatar = authorAvatarUrl,
                Text = source.Text,
                CreationTime = source.CreationTime.ToLocalTime(),
                FormatedComment = new Xamarin.Forms.FormattedString {
                    Spans = {
                        new Span {
                            Text = string.Format("{0}: ", authorDisplayName),
                            FontAttributes = FontAttributes.Bold,
                            FontFamily = App.Current.Resources["MontserratBold"].ToString(),
                            FontSize = 16
                        },
                        new Span {
                            Text = source.Text,
                            FontSize = 16
                        },
                        new Span {
                            Text = string.Format(new System.Globalization.CultureInfo("en-GB"),"- {0:M/d/yy h:mm tt}", source.CreationTime).ToUpper(),
                            ForegroundColor = (Color)App.Current.Resources["LightGrayColor"],
                            FontSize = 13
                        }
                    }
                }
            };
    }
}
