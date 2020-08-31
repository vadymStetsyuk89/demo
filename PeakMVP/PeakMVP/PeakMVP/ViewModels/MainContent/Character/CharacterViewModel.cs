using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Arguments.AppEventsArguments.Posts;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.Posts;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using PeakMVP.ViewModels.MainContent.ProfileContent.Arguments;
using PeakMVP.ViewModels.MainContent.ProfileContent.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Character;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Character {
    public class CharacterViewModel : TabbedViewModel, IProfileInfoDependent {

        private readonly IPostService _postService;
        private readonly IStateService _stateService;
        private readonly IContentViewModelFactory _contentViewModelFactory;

        private CancellationTokenSource _deletePostCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getFeedPostsCancellationTokenSource = new CancellationTokenSource();

        public CharacterViewModel(
            IPostService postService,
            IStateService stateService,
            IContentViewModelFactory contentViewModelFactory) {

            _postService = postService;
            _stateService = stateService;
            _contentViewModelFactory = contentViewModelFactory;

            CreateFeedViewModel = ViewModelLocator.Resolve<CreateFeedViewModel>();
            CreateFeedViewModel.InitializeAsync(this);
            CreateFeedViewModel.IsPossibleToHandleAttachExternalMediaArgs = true;

            EditPostPopupViewModel = ViewModelLocator.Resolve<EditPostPopupViewModel>();
            EditPostPopupViewModel.InitializeAsync(this);

            IsNestedPullToRefreshEnabled = true;
        }

        private string _avatar = GlobalSettings.Instance.UserProfile.Avatar?.Url;
        public string Avatar {
            get => _avatar;
            private set => SetProperty<string>(ref _avatar, value);
        }

        private string _displayName = GlobalSettings.Instance.UserProfile.DisplayName;
        public string DisplayName {
            get { return _displayName; }
            private set { SetProperty(ref _displayName, value); }
        }

        private ObservableCollection<PostContentViewModel> _posts;
        public ObservableCollection<PostContentViewModel> Posts {
            get { return _posts; }
            set {
                _posts?.ForEach(pCVM => pCVM.Dispose());
                SetProperty(ref _posts, value);
            }
        }

        private EditPostPopupViewModel _editPostPopupViewModel;
        public EditPostPopupViewModel EditPostPopupViewModel {
            get => _editPostPopupViewModel;
            private set {
                _editPostPopupViewModel?.Dispose();
                _editPostPopupViewModel = value;
            }
        }

        private CreateFeedViewModel _createFeedViewModel;
        public CreateFeedViewModel CreateFeedViewModel {
            get => _createFeedViewModel;
            private set {
                _createFeedViewModel?.Dispose();
                _createFeedViewModel = value;
            }
        }

        public void ResolveProfileInfo() {
            Avatar = GlobalSettings.Instance.UserProfile.Avatar?.Url;
            DisplayName = GlobalSettings.Instance.UserProfile.DisplayName;
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is NewFeedPostPublishedArgs || navigationData is PostWasEditedArgs) {
                ResolveProfileInfo();
                ExecuteActionWithBusy(GetPostsAsync);
            }

            CreateFeedViewModel?.InitializeAsync(navigationData);
            EditPostPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _deletePostCancellationTokenSource);
            ResetCancellationTokenSource(ref _getFeedPostsCancellationTokenSource);

            CreateFeedViewModel?.Dispose();
            EditPostPopupViewModel?.Dispose();

            Posts?.ForEach(pVM => pVM.Dispose());
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ResolveProfileInfo();
            ExecuteActionWithBusy(GetPostsAsync);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _deletePostCancellationTokenSource);
            ResetCancellationTokenSource(ref _getFeedPostsCancellationTokenSource);
        }

        protected override Task NestedRefreshAction() =>
            Task.Run(async () => {
                Device.BeginInvokeOnMainThread(() => ResolveProfileInfo());
                await GetPostsAsync();
                await CreateFeedViewModel.AskToRefreshAsync();
            });

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.FEED_TITLE;
            TabIcon = NavigationContext.FEED_IMAGE_PATH;
            RelativeViewType = typeof(CharacterView);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted, (sender, args) => {
                PostContentViewModel[] postsToDelete = Posts.Where(pVM => pVM.Data?.Author?.Id == args).ToArray();
                postsToDelete.ForEach(pCVM => Posts.Remove(pCVM));
            });

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostDeleting += OnPostEventsRequestToStartPostDeleting;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.StartWatchingPostComments += OnPostEventsStartWatchingPostComments;

            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.ChangedGroups += OnStateServiceChangedGroups;
            _stateService.ChangedProfile += OnStateServiceChangedProfile;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted);

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostDeleting -= OnPostEventsRequestToStartPostDeleting;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.StartWatchingPostComments -= OnPostEventsStartWatchingPostComments;

            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
            _stateService.ChangedProfile -= OnStateServiceChangedProfile;
        }

        private Task GetPostsAsync() => Task.Run(async () => {
            ResetCancellationTokenSource(ref _getFeedPostsCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _getFeedPostsCancellationTokenSource;

            try {
                List<PostDTO> foundedPosts = await _postService.GetPostsAsync(cancellationToken: cancellationTokenSource.Token);
                ObservableCollection<PostContentViewModel> preparedPosts = (foundedPosts != null) ? (_contentViewModelFactory.CreatePostContentViewModels(foundedPosts)).ToObservableCollection() : new ObservableCollection<PostContentViewModel>();

                Device.BeginInvokeOnMainThread(() => Posts = preparedPosts);
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }
        });

        private async void OnPostEventsRequestToStartPostDeleting(object sender, DeletePostArgs deletePostArgs) {
            ResetCancellationTokenSource(ref _deletePostCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _deletePostCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                bool deleteCompletion = await _postService.DeletePostByPostId(deletePostArgs.TargetPost.Data.Id, cancellationTokenSource);

                if (deleteCompletion) {
                    Posts?.Remove(deletePostArgs.TargetPost);
                }
                else {
                    await DialogService.ToastAsync(PostService.POST_DELETE_ERROR_MESSAGE);
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
        }

        private void OnPostEventsStartWatchingPostComments(object sender, PostContentViewModel e) {
            try {
                if (e.HasDisplayComments) {
                    Posts.ForEach<PostContentViewModel>(postContent => postContent.HasDisplayComments = postContent.Id == e.Id);
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();
            }
        }

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) => ExecuteActionWithBusy(GetPostsAsync);

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs e) => ExecuteActionWithBusy(GetPostsAsync);

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => ExecuteActionWithBusy(GetPostsAsync);

        private void OnProfileSettingsEventsProfileUpdated(object sender, Models.Arguments.AppEventsArguments.UserProfile.ProfileUpdatedArgs e) {
            ResolveProfileInfo();
            ExecuteActionWithBusy(GetPostsAsync);
        }

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => ExecuteActionWithBusy(GetPostsAsync);

        private void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) => ExecuteActionWithBusy(GetPostsAsync);
    }
}
