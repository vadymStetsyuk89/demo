using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Arguments.AppEventsArguments.Posts;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Posts;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Character;
using PeakMVP.ViewModels.MainContent.ProfileContent.Arguments;
using PeakMVP.ViewModels.MainContent.ProfileContent.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class ProfileContentViewModel : NestedViewModel, IProfileInfoDependent {

        private readonly IPostService _postService;
        private readonly IContentViewModelFactory _contentViewModelFactory;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getFeedPostsCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _deletePostCancellationTokenSource = new CancellationTokenSource();

        public ProfileContentViewModel(
            IPostService postService,
            IContentViewModelFactory contentViewModelFactory,
            IStateService stateService) {

            _postService = postService;
            _contentViewModelFactory = contentViewModelFactory;
            _stateService = stateService;

            IsNestedPullToRefreshEnabled = true;

            InvitesContentViewModel = ViewModelLocator.Resolve<InvitesContentViewModel>();
            InvitesContentViewModel.InitializeAsync(this);

            CreateFeedViewModel = ViewModelLocator.Resolve<CreateFeedViewModel>();
            CreateFeedViewModel.InitializeAsync(this);

            EditPostPopupViewModel = ViewModelLocator.Resolve<EditPostPopupViewModel>();
            EditPostPopupViewModel.InitializeAsync(this);
        }

        private UserTypeDependentProfileContentBaseViewModel _userTypeSpecificViewModel;
        public UserTypeDependentProfileContentBaseViewModel UserTypeSpecificViewModel {
            get => _userTypeSpecificViewModel;
            private set {
                _userTypeSpecificViewModel?.Dispose();
                _userTypeSpecificViewModel = value;
            }
        }

        private InvitesContentViewModel _invitesContentViewModel;
        public InvitesContentViewModel InvitesContentViewModel {
            get => _invitesContentViewModel;
            private set {
                _invitesContentViewModel?.Dispose();
                _invitesContentViewModel = value;
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

        private EditPostPopupViewModel _editPostPopupViewModel;
        public EditPostPopupViewModel EditPostPopupViewModel {
            get => _editPostPopupViewModel;
            private set {
                _editPostPopupViewModel?.Dispose();
                _editPostPopupViewModel = value;
            }
        }

        private ObservableCollection<PostContentViewModel> _posts;
        public ObservableCollection<PostContentViewModel> Posts {
            get { return _posts; }
            set {
                _posts?.ForEach(pCVM => pCVM.Dispose());
                SetProperty(ref _posts, value);
            }
        }

        private string _fullName;
        public string FullName {
            get { return _fullName; }
            set { SetProperty(ref _fullName, value); }
        }

        private string _about;
        public string About {
            get { return _about; }
            set { SetProperty(ref _about, value); }
        }

        private string _mySports;
        public string MySports {
            get { return _mySports; }
            set { SetProperty(ref _mySports, value); }
        }

        string _avatar;
        public string Avatar {
            get { return _avatar; }
            set { SetProperty(ref _avatar, value); }
        }

        public void ResolveProfileInfo() {
            FullName = GlobalSettings.Instance.UserProfile.DisplayName;
            MySports = GlobalSettings.Instance.UserProfile.MySports;
            About = GlobalSettings.Instance.UserProfile.About;

            Avatar = (GlobalSettings.Instance.UserProfile.Avatar?.Url != null)
                ? string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, GlobalSettings.Instance.UserProfile.Avatar?.Url) : null;
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ContentPageBaseViewModel) {
                switch (GlobalSettings.Instance.UserProfile.ProfileType) {
                    case ProfileType.Fan:
                        UserTypeSpecificViewModel = ViewModelLocator.Resolve<FanProfileContentViewModel>();
                        break;
                    case ProfileType.Player:
                        UserTypeSpecificViewModel = ViewModelLocator.Resolve<PlayerProfileContentViewModel>();
                        break;
                    case ProfileType.Parent:
                        UserTypeSpecificViewModel = ViewModelLocator.Resolve<ParentProfileContentViewModel>();
                        break;
                    case ProfileType.Organization:
                        UserTypeSpecificViewModel = ViewModelLocator.Resolve<OrganizationProfileContentViewModel>();
                        break;
                    case ProfileType.Coach:
                        UserTypeSpecificViewModel = ViewModelLocator.Resolve<CoachProfileContentViewModel>();
                        break;
                    default:
                        Debugger.Break();
                        break;
                }

                UserTypeSpecificViewModel?.InitializeAsync(this);
            }
            else if (navigationData is NewFeedPostPublishedArgs || navigationData is PostWasEditedArgs) {
                ResolveProfileInfo();
                ExecuteActionWithBusy(GetOwnPostsAsync);
            }

            UserTypeSpecificViewModel?.InitializeAsync(navigationData);
            InvitesContentViewModel?.InitializeAsync(navigationData);
            CreateFeedViewModel?.InitializeAsync(navigationData);
            EditPostPopupViewModel?.InitializeAsync(navigationData);
            Posts?.ForEach<PostContentViewModel>(postViewModel => postViewModel.InitializeAsync(navigationData));

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            CancellationService.Cancel();
            ResetCancellationTokenSource(ref _getFeedPostsCancellationTokenSource);
            ResetCancellationTokenSource(ref _deletePostCancellationTokenSource);

            UserTypeSpecificViewModel?.Dispose();
            InvitesContentViewModel?.Dispose();
            CreateFeedViewModel?.Dispose();
            EditPostPopupViewModel?.Dispose();

            Posts?.ForEach<PostContentViewModel>(postViewModel => postViewModel.Dispose());
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            ResolveProfileInfo();
            ExecuteActionWithBusy(GetOwnPostsAsync);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            CancellationService.Cancel();
            ResetCancellationTokenSource(ref _getFeedPostsCancellationTokenSource);
            ResetCancellationTokenSource(ref _deletePostCancellationTokenSource);
        }

        protected override Task NestedRefreshAction() =>
            Task.Run(async () => {
                Device.BeginInvokeOnMainThread(() => ResolveProfileInfo());
                await GetOwnPostsAsync();
                await InvitesContentViewModel?.AskToRefreshAsync();
                await UserTypeSpecificViewModel?.AskToRefreshAsync();
                await CreateFeedViewModel.AskToRefreshAsync();
            });

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated += OnProfileSettingsEventsProfileUpdated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.StartWatchingPostComments += OnPostEventsStartWatchingPostComments;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostDeleting += OnPostEventsRequestToStartPostDeleting;
            _stateService.ChangedGroups += OnStateServiceChangedGroups;
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.ChangedProfile += OnStateServiceChangedProfile;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdated -= OnProfileSettingsEventsProfileUpdated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.StartWatchingPostComments -= OnPostEventsStartWatchingPostComments;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostDeleting -= OnPostEventsRequestToStartPostDeleting;
            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.ChangedProfile -= OnStateServiceChangedProfile;
        }

        private Task GetOwnPostsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getFeedPostsCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getFeedPostsCancellationTokenSource;

                try {
                    List<PostDTO> foundedPosts = await _postService.GetPostsAsync(cancellationToken: cancellationTokenSource.Token);
                    ObservableCollection<PostContentViewModel> preparedPosts = (foundedPosts != null) ? (_contentViewModelFactory.CreatePostContentViewModels(foundedPosts.Where<PostDTO>(post => post.Author?.Id == GlobalSettings.Instance.UserProfile.Id))).ToObservableCollection() : new ObservableCollection<PostContentViewModel>();

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

        private void OnProfileSettingsEventsProfileUpdated(object sender, Models.Arguments.AppEventsArguments.UserProfile.ProfileUpdatedArgs e) {
            ResolveProfileInfo();
            ExecuteActionWithBusy(GetOwnPostsAsync);
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

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) => ExecuteActionWithBusy(GetOwnPostsAsync);

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs e) => ExecuteActionWithBusy(GetOwnPostsAsync);

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => ExecuteActionWithBusy(GetOwnPostsAsync);

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => ExecuteActionWithBusy(GetOwnPostsAsync);

        private void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) => ExecuteActionWithBusy(GetOwnPostsAsync);
    }
}