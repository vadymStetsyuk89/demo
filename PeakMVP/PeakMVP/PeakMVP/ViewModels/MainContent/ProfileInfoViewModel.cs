using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Friends;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Friends;
using PeakMVP.Services.Posts;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.Albums;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.ViewModels.MainContent.MediaViewers;
using PeakMVP.ViewModels.MainContent.MediaViewers.Arguments;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent {
    public sealed class ProfileInfoViewModel : LoggedContentPageViewModel {

        private static readonly string _CANT_DECLINE_FRIENDSHIP_ERROR = "Can't reject friendship now";
        private static readonly string _CANT_CONFIRM_FRIENDSHIP_ERROR = "Can't confirm friendship now";
        private static readonly string _CANT_ADD_FRIEND_ERROR = "Can't add new friend now";
        private static readonly string _FRIENDSHIP_REJECTED = "Friend rejected";

        private const string REMOVE_FRIENDS = "REMOVE FRIEND";
        private const string FRIENDS = "FRIEND";
        private const string INVITE_SENT = "INVITE SENT";
        private const string ADD_TO_FRIENDS = "ADD TO FRIENDS";
        private const string CONFIRM_REQUEST = "CONFIRM REQUEST";

        private readonly IContentViewModelFactory _contentViewModelFactory;
        private readonly IProfileService _profileService;
        private readonly IFriendService _friendService;
        private readonly IPostService _postService;
        private readonly IStateService _stateService;
        private readonly IMediaFactory _mediaFactory;

        private CancellationTokenSource _resolveProfileCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getProfilePostsCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _declineFriendshipCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _confirmFriendshipCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _addFriendCancellationTokenSource = new CancellationTokenSource();

        public ProfileInfoViewModel(
            IContentViewModelFactory contentViewModelFactory,
            IProfileService profileService,
            IFriendService friendService,
            IPostService postService,
            IStateService stateService,
            IMediaFactory mediaFactory) {

            _postService = postService;
            _friendService = friendService;
            _profileService = profileService;
            _contentViewModelFactory = contentViewModelFactory;
            _stateService = stateService;
            _mediaFactory = mediaFactory;

            ActionBarViewModel = ViewModelLocator.Resolve<ModeActionBarViewModel>();
            ActionBarViewModel.InitializeAsync(this);
            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = false;

            RefreshCommand = new Command(async () => {
                IsRefreshing = true;

                ResolveProfile(Profile);

                await Task.Delay(500);
                IsRefreshing = false;
            });
        }

        public ICommand FriendshipStatusCommand => new Command(() => {
            if (FriendshipStatus == REMOVE_FRIENDS || FriendshipStatus == FRIENDS) {
                DeclineFriendship();
            }
            else if (FriendshipStatus == INVITE_SENT) {
                DeclineFriendship();
            }
            else if (FriendshipStatus == ADD_TO_FRIENDS) {
                AddFriend();
            }
            else if (FriendshipStatus == CONFIRM_REQUEST) {
                ConfirmFriendship();
            }
            else {
                Debugger.Break();
            }
        });

        public ICommand WatchPicturesCommand => new Command((object parameter) => {
            NavigationService.NavigateToAsync<PicturesViewerViewModel>(new StartWatchingPicturesArgs() {
                TargetMedia = (ProfileMediaDTO)parameter,
                MediasSource = ProfilePictures.ToArray<ProfileMediaDTO>()
            });
        });

        public ICommand WatchVideoCommand => new Command(async (object param) => await NavigationService.NavigateToAsync<VideoViewerViewModel>(param));

        ObservableCollection<PostContentViewModel> _posts;
        public ObservableCollection<PostContentViewModel> Posts {
            get { return _posts; }
            private set { SetProperty(ref _posts, value); }
        }

        bool _isFriendStatusButtonAvailable;
        public bool IsFriendStatusButtonAvailable {
            get { return _isFriendStatusButtonAvailable; }
            private set { SetProperty(ref _isFriendStatusButtonAvailable, value); }
        }

        bool _isProfileFriend = false;
        public bool IsProfileFriend {
            get { return _isProfileFriend; }
            private set { SetProperty(ref _isProfileFriend, value); }
        }

        string _aboutProfile;
        public string AboutProfile {
            get { return _aboutProfile; }
            private set { SetProperty(ref _aboutProfile, value); }
        }

        string _profileSports;
        public string ProfileSports {
            get { return _profileSports; }
            private set { SetProperty(ref _profileSports, value); }
        }

        string _friendshipStatus;
        public string FriendshipStatus {
            get { return _friendshipStatus; }
            private set { SetProperty(ref _friendshipStatus, value); }
        }

        ProfileDTO _profile;
        public ProfileDTO Profile {
            get { return _profile; }
            private set { SetProperty(ref _profile, value); }
        }

        private ObservableCollection<ProfileMediaDTO> _profilePictures = new ObservableCollection<ProfileMediaDTO>();
        public ObservableCollection<ProfileMediaDTO> ProfilePictures {
            get => _profilePictures;
            set {
                SetProperty(ref _profilePictures, value);

                if (_profilePictures != null) {
                    _profilePictures.ForEach<ProfileMediaDTO>(media => _mediaFactory.BuildValidUrlPath(media));
                }
            }
        }

        private ObservableCollection<ProfileMediaDTO> _profileVideos = new ObservableCollection<ProfileMediaDTO>();
        public ObservableCollection<ProfileMediaDTO> ProfileVideos {
            get => _profileVideos;
            set {
                SetProperty(ref _profileVideos, value);

                if (_profileVideos != null) {
                    _profileVideos.ForEach<ProfileMediaDTO>(media => _mediaFactory.BuildValidUrlPath(media));
                }
            }
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is FoundSingleUserDataItem foundSingleUserDataItem) {
                ResolveProfile(foundSingleUserDataItem.UserProfile);
            }
            else if (navigationData is FriendshipInviteViewModel friendshipInviteViewModel) {
                ResolveProfile(friendshipInviteViewModel.Profile);
            }
            else if (navigationData is ChildItemViewModel childItemViewModel) {
                ResolveProfile(childItemViewModel.Profile);
            }
            else if (navigationData is ProfileDTO profile) {
                ResolveProfile(profile);
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _resolveProfileCancellationTokenSource);
            ResetCancellationTokenSource(ref _getProfilePostsCancellationTokenSource);
            ResetCancellationTokenSource(ref _declineFriendshipCancellationTokenSource);
            ResetCancellationTokenSource(ref _confirmFriendshipCancellationTokenSource);
            ResetCancellationTokenSource(ref _addFriendCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _resolveProfileCancellationTokenSource);
            ResetCancellationTokenSource(ref _getProfilePostsCancellationTokenSource);
            ResetCancellationTokenSource(ref _declineFriendshipCancellationTokenSource);
            ResetCancellationTokenSource(ref _confirmFriendshipCancellationTokenSource);
            ResetCancellationTokenSource(ref _addFriendCancellationTokenSource);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged += OnProfileSettingsEventsAppBackgroundImageChanged;
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged -= OnProfileSettingsEventsAppBackgroundImageChanged;
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs args) => ResolveProfile(Profile);

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs args) => ResolveProfile(Profile);

        private async void ResolveProfile(ProfileDTO targetProfile) {
            ResetCancellationTokenSource(ref _resolveProfileCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _resolveProfileCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                GetFriendByIdResponse getFriendByIdResponse = await _friendService.GetFriendByIdAsync(targetProfile.Id, cancellationTokenSource.Token);

                GetProfileResponse getprofileResponse = await _profileService.GetProfileByShortIdAsync(targetProfile.ShortId, _resolveProfileCancellationTokenSource.Token);
                if (getprofileResponse != null) {
                    AboutProfile = getprofileResponse.About;
                    ProfileSports = getprofileResponse.MySports;

                    if (getprofileResponse.BackgroundImage != null) {
                        AppBackgroundImage = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, getprofileResponse.BackgroundImage.Url);
                    }
                }

                if (getFriendByIdResponse.Profile == null && !(getFriendByIdResponse.IsConfirmed) && !(getFriendByIdResponse.IsRequest)) {
                    IsProfileFriend = false;
                    Posts = new ObservableCollection<PostContentViewModel>();
                    FriendshipStatus = ADD_TO_FRIENDS;

                    ProfilePictures = new ObservableCollection<ProfileMediaDTO>();
                    ProfileVideos = new ObservableCollection<ProfileMediaDTO>();
                }
                else if (getFriendByIdResponse.Profile != null && getFriendByIdResponse.IsConfirmed && !(getFriendByIdResponse.IsRequest)) {
                    if (!IsProfileFriend) {
                        GetPosts(getFriendByIdResponse.Profile.ShortId);
                    }

                    IsProfileFriend = true;
                    FriendshipStatus = REMOVE_FRIENDS;

                    FriendshipStatus = (GlobalSettings.Instance.UserProfile.Children != null && GlobalSettings.Instance.UserProfile.Children.Any(profile => profile.Id == targetProfile.Id))
                        ? FRIENDS : REMOVE_FRIENDS;

                    ProfilePictures = getprofileResponse?.Media != null ? new ObservableCollection<ProfileMediaDTO>(getprofileResponse.Media.Where<ProfileMediaDTO>(media => media.Mime == ProfileMediaService.MIME_IMAGE_TYPE)) : new ObservableCollection<ProfileMediaDTO>();
                    ProfileVideos = getprofileResponse?.Media != null ? new ObservableCollection<ProfileMediaDTO>(getprofileResponse.Media.Where<ProfileMediaDTO>(media => media.Mime == ProfileMediaService.MIME_VIDEO_TYPE)) : new ObservableCollection<ProfileMediaDTO>();
                }
                else if (getFriendByIdResponse.Profile != null && !(getFriendByIdResponse.IsConfirmed) && getFriendByIdResponse.IsRequest) {
                    IsProfileFriend = false;
                    FriendshipStatus = CONFIRM_REQUEST;
                    Posts = new ObservableCollection<PostContentViewModel>();

                    ProfilePictures = new ObservableCollection<ProfileMediaDTO>();
                    ProfileVideos = new ObservableCollection<ProfileMediaDTO>();
                }
                else if (getFriendByIdResponse.Profile != null && !(getFriendByIdResponse.IsConfirmed) && !(getFriendByIdResponse.IsRequest)) {
                    IsProfileFriend = false;
                    FriendshipStatus = INVITE_SENT;
                    Posts = new ObservableCollection<PostContentViewModel>();

                    ProfilePictures = new ObservableCollection<ProfileMediaDTO>();
                    ProfileVideos = new ObservableCollection<ProfileMediaDTO>();
                }
                else {
                    Debugger.Break();
                }

                if (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent && getprofileResponse?.ParentId.HasValue == true && getprofileResponse?.ParentId.Value == GlobalSettings.Instance.UserProfile.Id) {
                    IsFriendStatusButtonAvailable = false;
                }
                else {
                    IsFriendStatusButtonAvailable = true;
                }

                /// Crutch. GetFriendByIdResponse profile will not contain avatars. Remove it when response profile will be valid
                if (getFriendByIdResponse.Profile != null) {
                    getFriendByIdResponse.Profile.Avatar = targetProfile.Avatar;
                }

                Profile = getFriendByIdResponse.Profile ?? targetProfile;
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }

        private async void GetPosts(string profileShortId) {
            ResetCancellationTokenSource(ref _getProfilePostsCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _getProfilePostsCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                List<PostDTO> foundedPosts = await _postService.GetPostsAsync(profileShortId, string.Empty, cancellationTokenSource.Token);

                if (foundedPosts != null && foundedPosts.Any()) {

                    var filtered = from p in foundedPosts
                                   where p.Author.ShortId == profileShortId
                                   select p;

                    Posts = (_contentViewModelFactory.CreatePostContentViewModels(filtered)).ToObservableCollection();
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }

        private async void AddFriend() {
            ResetCancellationTokenSource(ref _addFriendCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _addFriendCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                AddFriendResponse addFriendResponse = await _friendService.AddFriendAsync(Profile.ShortId, cancellationTokenSource.Token);

                if (addFriendResponse != null) {
                    IsProfileFriend = false;
                    FriendshipStatus = INVITE_SENT;

                    await DialogService.ToastAsync("Friend request sent!");
                }
                else {
                    throw new InvalidOperationException(_CANT_ADD_FRIEND_ERROR);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }

        private async void DeclineFriendship() {
            ResetCancellationTokenSource(ref _declineFriendshipCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _declineFriendshipCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                DeleteFriendResponse deleteFriendResponse = await _friendService.RequestDeleteAsync(Profile.Id, cancellationTokenSource.Token);

                if (deleteFriendResponse != null) {
                    IsProfileFriend = false;
                    FriendshipStatus = ADD_TO_FRIENDS;

                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted, Profile.Id);

                    await DialogService.ToastAsync(_FRIENDSHIP_REJECTED);
                }
                else {
                    throw new InvalidOperationException(_CANT_DECLINE_FRIENDSHIP_ERROR);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }

        private async void ConfirmFriendship() {
            ResetCancellationTokenSource(ref _confirmFriendshipCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _confirmFriendshipCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                ConfirmFriendResponse confirmFriendResponse = await _friendService.RequestConfirmAsync(Profile.Id, cancellationTokenSource.Token);

                if (confirmFriendResponse != null) {
                    IsProfileFriend = true;
                    FriendshipStatus = REMOVE_FRIENDS;

                    GetPosts(confirmFriendResponse.ShortId);

                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteAccepted, Profile.Id);

                    await DialogService.ToastAsync("Friend request confirmed!");
                }
                else {
                    throw new InvalidOperationException(_CANT_CONFIRM_FRIENDSHIP_ERROR);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }

        private void OnProfileSettingsEventsAppBackgroundImageChanged(object sender, EventArgs e) => AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;
    }
}
