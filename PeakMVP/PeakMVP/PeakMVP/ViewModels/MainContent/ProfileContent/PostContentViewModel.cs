using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Arguments.AppEventsArguments.Posts;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Services.Posts;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.MediaViewers;
using PeakMVP.ViewModels.MainContent.MediaViewers.Arguments;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class PostContentViewModel : NestedViewModel {

        private static readonly string _VIEW_MEDIA_UNKNOWN_MEDIA_FORMAT_ERROR_MESSAGE = "Unknown media format";
        private static readonly string _ADD_COMMENT_ERROR_MESSAGE = "Can't add comment at the moment";
        private static readonly string _EMPTY_COMMENT_ERROR_MESSAGE = "Type what's in your mind";

        private readonly IPostService _postService;
        private readonly IContentViewModelFactory _contentViewModelFactory;

        private CancellationTokenSource _addComentCancellationTokenSource = new CancellationTokenSource();

        public PostContentViewModel(IPostService postService,
            IContentViewModelFactory contentViewModelFactory) {
            _postService = postService;
            _contentViewModelFactory = contentViewModelFactory;
        }

        public ICommand ShowCommentsCommand => new Command(() => {
            HasDisplayComments = !HasDisplayComments;

            GlobalSettings.Instance.AppMessagingEvents.PostEvents.StartWatchingPostCommentsInvoke(this, this);
        });

        public ICommand EditPostCommand => new Command(() => GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostEditingInvoke(this, new EditPostArgs() { TargetPost = this }));

        public ICommand DeletePostCommand => new Command(() => GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostDeletingInvoke(this, new DeletePostArgs() { TargetPost = this }));

        public ICommand PublishCommentCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _addComentCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _addComentCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                if (string.IsNullOrEmpty(CommentMessage) || string.IsNullOrWhiteSpace(CommentMessage)) {
                    throw new InvalidOperationException(_EMPTY_COMMENT_ERROR_MESSAGE);
                }
                else {
                    CommentDTO comment = await _postService.PublishCommentAsync(new PublishCommentDataModel {
                        PostId = Data.Id,
                        ProfileId = GlobalSettings.Instance.UserProfile.Id,
                        ProfileType = GlobalSettings.Instance.UserProfile.ProfileType.ToString(),
                        Text = CommentMessage
                    }, cancellationTokenSource.Token);

                    if (comment != null) {
                        if (Comments == null) {
                            Comments = new ObservableCollection<CommentContentViewModel>();
                        }

                        Comments.Add(_contentViewModelFactory.CreateComment(comment, GlobalSettings.Instance.UserProfile));

                        CommentMessage = "";
                        CountComments = Comments.Count;
                    }
                    else {
                        throw new InvalidOperationException(_ADD_COMMENT_ERROR_MESSAGE);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public ICommand ViewPostAuthorCommand => new Command(async () => {
            await DialogService.ToastAsync("View post author in developing.");
        });

        public ICommand WatchPostMediaCommand => new Command(async (object param) => {
            Media targetMedia = (Media)param;

            if (targetMedia.Data.Mime == ProfileMediaService.MIME_IMAGE_TYPE) {
                await NavigationService.NavigateToAsync<PicturesViewerViewModel>(new StartWatchingPicturesArgs() {
                    MediasSource = new ProfileMediaDTO[] { targetMedia.Data },
                    TargetMedia = targetMedia.Data
                });
            }
            else if (targetMedia.Data.Mime == ProfileMediaService.MIME_VIDEO_TYPE) {
                await NavigationService.NavigateToAsync<VideoViewerViewModel>(targetMedia.Data);
            }
            else {
                await DialogService.ToastAsync(PostContentViewModel._VIEW_MEDIA_UNKNOWN_MEDIA_FORMAT_ERROR_MESSAGE);
            }
        });

        public PostDTO Data { get; set; }

        long _id;
        public long Id {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        string _displayName;
        public string DisplayName {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        string _text;
        public string Text {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        bool _hasDisplayComments;
        public bool HasDisplayComments {
            get { return _hasDisplayComments; }
            set { SetProperty(ref _hasDisplayComments, value); }
        }

        bool _isAnyMedia = true;
        public bool IsAnyMedia {
            get { return _isAnyMedia; }
            set { SetProperty(ref _isAnyMedia, value); }
        }

        int _countComments;
        public int CountComments {
            get { return _countComments; }
            set { SetProperty(ref _countComments, value); }
        }

        DateTime _publishTime;
        public DateTime PublishTime {
            get { return _publishTime; ; }
            set { SetProperty(ref _publishTime, value); }
        }

        string _avatar;
        public string Avatar {
            get { return _avatar; }
            set { SetProperty(ref _avatar, value); }
        }

        bool _isEditable;
        public bool IsEditable {
            get { return _isEditable; }
            set { SetProperty(ref _isEditable, value); }
        }

        private bool _canBeDeleted;
        public bool CanBeDeleted {
            get => _canBeDeleted;
            set => SetProperty<bool>(ref _canBeDeleted, value);
        }

        ObservableCollection<CommentContentViewModel> _comments = new ObservableCollection<CommentContentViewModel>();
        public ObservableCollection<CommentContentViewModel> Comments {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }


        ObservableCollection<Media> _medias = new ObservableCollection<Media>();
        public ObservableCollection<Media> Medias {
            get { return _medias; }
            set { SetProperty(ref _medias, value); }
        }

        string _commentMessage;
        public string CommentMessage {
            get { return _commentMessage; }
            set { SetProperty(ref _commentMessage, value); }
        }

        private string _publicityScopeName;
        public string PublicityScopeName {
            get => _publicityScopeName;
            set => SetProperty<string>(ref _publicityScopeName, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _addComentCancellationTokenSource);
            Comments?.ForEach(cVM => cVM.Dispose());
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _addComentCancellationTokenSource);
        }
    }
}
