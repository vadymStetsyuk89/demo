using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.InitializeArguments.Post;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Posts;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ProfileContent.Arguments;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Character {
    public class CreateFeedViewModel : NestedViewModel, IAskToRefresh, IInputForm {

        private static readonly string _FEED_POST_WITHOUT_TEXT_ERROR_MESSAGE = "Type what's in your mind.";
        private readonly IProfileMediaService _profileMediaService;
        private readonly IMediaPickerService _mediaPickerService;
        private readonly IPostService _postService;
        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IFileDTOBuilder _fileDTOBuilder;
        private readonly IMediaFactory _mediaFactory;
        private readonly IStateService _stateService;

        private readonly List<MediaDTO> _mediaDTOs = new List<MediaDTO>();

        private CancellationTokenSource _publishPostCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getPostPublicityScopesSourceCancellationTokenSource = new CancellationTokenSource();

        public CreateFeedViewModel(IProfileMediaService profileMediaService,
                                   IMediaFactory mediaFactory,
                                   IMediaPickerService mediaPickerService,
                                   IPostService postService,
                                   IValidationObjectFactory validationObjectFactory,
                                   IFileDTOBuilder fileDTOBuilder,
                                   IStateService stateService) {
            _profileMediaService = profileMediaService;
            _mediaPickerService = mediaPickerService;
            _postService = postService;
            _validationObjectFactory = validationObjectFactory;
            _fileDTOBuilder = fileDTOBuilder;
            _mediaFactory = mediaFactory;
            _stateService = stateService;

            IsAnyMedia = AttachedMedia.Any();
            ResetValidationObjects();
        }

        public ICommand PostNewFeedMessageCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _publishPostCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _publishPostCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                PublishPostDataModel publishPostDataModel = await PrepareRequestPostDataModelAsync(cancellationTokenSource);
                bool posted = await _postService.PublishPostAsync(publishPostDataModel, cancellationTokenSource);

                if (posted) {
                    ResetInputForm();

                    //await NavigationService.LastPageViewModel.InitializeAsync(new NewFeedPostPublishedArgs() {
                    //    PublishedPost = postDTO
                    //});
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

        public ICommand AttachFeedPhotoCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                MediaFile pickedPhotoMediaFile = await _mediaPickerService.PickPhotoAsync();

                if (pickedPhotoMediaFile != null) {
                    Stream pickedPhotoStream = pickedPhotoMediaFile.GetStream();
                    string base64Photo = await _mediaPickerService.ParseStreamToBase64(pickedPhotoStream);
                    pickedPhotoStream.Close();
                    pickedPhotoStream.Dispose();

                    if (!string.IsNullOrEmpty(pickedPhotoMediaFile.Path) && !string.IsNullOrEmpty(base64Photo)) {
                        FileDTO fileDTO = new FileDTO { Base64 = base64Photo, Name = Path.GetFileName(pickedPhotoMediaFile.Path) };

                        MediaDTO media = await _profileMediaService.UploadMediaToTrayAsync(fileDTO, new CancellationTokenSource());

                        if (media != null) {
                            if (!string.IsNullOrEmpty(base64Photo)) {
                                AttachedMedia.Add(_mediaFactory.BuidAttachedMedia(MediaType.Picture, base64Photo, media));
                            }
                        }
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            IsAnyMedia = AttachedMedia.Any();
            UpdateBusyVisualState(busyKey, false);
        });

        public ICommand AttachFeedVideoCommand {
            get {
                return new Command(async () => {
                    Guid busyKey = Guid.NewGuid();
                    UpdateBusyVisualState(busyKey, true);

                    try {
                        IPickedMediaFromGallery pickedMediaFromGallery = await DependencyService.Get<IPickMediaDependencyService>().PickVideoAsync();

                        if (pickedMediaFromGallery.Completion) {
                            if (!string.IsNullOrEmpty(pickedMediaFromGallery.DataBase64) && !string.IsNullOrEmpty(pickedMediaFromGallery.DataThumbnailBase64)) {
                                FileDTO fileDTO = new FileDTO {
                                    Base64 = pickedMediaFromGallery.DataBase64,
                                    Name = Path.GetFileName(pickedMediaFromGallery.FilePath)
                                };

                                MediaDTO media = await _profileMediaService.UploadMediaToTrayAsync(fileDTO, new CancellationTokenSource());

                                if (media != null) {
                                    AttachedMedia.Add(_mediaFactory.BuidAttachedMedia(MediaType.Video, pickedMediaFromGallery.DataBase64, media, pickedMediaFromGallery.DataThumbnailBase64));
                                }
                            }
                        }
                    }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);

                        await DialogService.ToastAsync(exc.Message);
                    }

                    IsAnyMedia = AttachedMedia.Any();
                    UpdateBusyVisualState(busyKey, false);
                });
            }
        }

        public ICommand RemoveAttachedMediaCommand => new Command((object param) => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);
            try {
                AttachedMedia.Remove(param as AttachedFeedMedia);
                _profileMediaService.DeleteMediaFromTrayAsync((param as AttachedFeedMedia).Id, new CancellationTokenSource());
            }
            catch (Exception ex) {
                Debug.WriteLine($"ERROR: {ex.Message}");
                Debugger.Break();
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public bool IsPossibleToHandleAttachExternalMediaArgs { get; set; }

        private ObservableCollection<AttachedFeedMedia> _attachedMedia = new ObservableCollection<AttachedFeedMedia>();
        public ObservableCollection<AttachedFeedMedia> AttachedMedia {
            get => _attachedMedia;
            set => SetProperty(ref _attachedMedia, value);
        }

        private bool _isAnyMedia;
        public bool IsAnyMedia {
            get => _isAnyMedia;
            set => SetProperty(ref _isAnyMedia, value);
        }

        private ValidatableObject<string> _mainFeedMessage;
        public ValidatableObject<string> MainFeedMessage {
            get => _mainFeedMessage;
            set => SetProperty<ValidatableObject<string>>(ref _mainFeedMessage, value);
        }

        private List<PostPublicityScope> _postPublicityScopesSource;
        public List<PostPublicityScope> PostPublicityScopesSource {
            get => _postPublicityScopesSource;
            set {
                SetProperty<List<PostPublicityScope>>(ref _postPublicityScopesSource, value);

                if (value != null) {
                    SelectedPostPublicityScope = value.FirstOrDefault();
                }
            }
        }

        private PostPublicityScope _selectedPostPublicityScope;
        public PostPublicityScope SelectedPostPublicityScope {
            get => _selectedPostPublicityScope;
            set => SetProperty<PostPublicityScope>(ref _selectedPostPublicityScope, value);
        }

        public Task AskToRefreshAsync() => UpdatePostPublicityScopesSourceAsync();

        public bool ValidateForm() {
            bool isValidResult = false;

            isValidResult = MainFeedMessage.Validate();

            return isValidResult;
        }

        public void ResetInputForm() {
            ResetValidationObjects();

            AttachedMedia = new ObservableCollection<AttachedFeedMedia>();
            IsAnyMedia = AttachedMedia.Any();

            SelectedPostPublicityScope = PostPublicityScopesSource?.FirstOrDefault();
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ContentPageBaseViewModel) {
                ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync);
            } else if (navigationData is AttachExternalMediaToNewPostArgs externalMediaToNewPostArgs) {
                if (IsPossibleToHandleAttachExternalMediaArgs) {
                    AttachedMedia.Add(_mediaFactory.BuidAttachedMedia(externalMediaToNewPostArgs.NewMedia, externalMediaToNewPostArgs.MediaDTO));
                    IsAnyMedia = AttachedMedia.Any();
                }
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _publishPostCancellationTokenSource);
            ResetCancellationTokenSource(ref _getPostPublicityScopesSourceCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            UpdatePostPublicityScopesSourceAsync();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _publishPostCancellationTokenSource);
            ResetCancellationTokenSource(ref _getPostPublicityScopesSourceCancellationTokenSource);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Instance.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, args) => ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync));
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated += OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted += OnTeamEventsTeamDeleted;
            _stateService.ChangedGroups += OnStateServiceChangedGroups;
            _stateService.ChangedTeams += OnStateServiceChangedTeams;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Instance.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreated -= OnTeamEventsNewTeamCreated;
            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.TeamDeleted -= OnTeamEventsTeamDeleted;
            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        private Task UpdatePostPublicityScopesSourceAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getPostPublicityScopesSourceCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getPostPublicityScopesSourceCancellationTokenSource;

                try {
                    List<PostPublicityScope> scopes = await _postService.GetPossiblePostPublicityScopesAsync(cancellationTokenSource);

                    Device.BeginInvokeOnMainThread(() => PostPublicityScopesSource = scopes);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private Task<PublishPostDataModel> PrepareRequestPostDataModelAsync(CancellationTokenSource cancelationTokenSource) =>
            Task.Run(() => {
                //AttachedFileDataModel[] attachedFiles = AttachedMedia
                //    .Select(aM => (aM.MediaType == MediaType.Picture)
                //        ? new AttachedFileDataModel() {
                //            File = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, aM.SourceBase64)
                //        }
                //        : new AttachedFileDataModel() {
                //            File = _fileDTOBuilder.BuidFileDTO(MediaType.Video, aM.SourceBase64),
                //            Thumbnail = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, aM.ThumbnailBase64)
                //        })
                //    .ToArray();

                PublishPostDataModel publishPostDataModel = new PublishPostDataModel {
                    Files = AttachedMedia.Select(m => m.Id).ToArray(),
                    Text = MainFeedMessage.Value,
                    PostPolicyType = SelectedPostPublicityScope.PolicyType,
                    GroupId = SelectedPostPublicityScope.Id
                };

                //PublishPostDataModel publishPostDataModel = new PublishPostDataModel {
                //    Files = attachedFiles,
                //    Text = MainFeedMessage.Value,
                //    GroupId = SelectedPostPublicityScope.Id,
                //    PostPolicyType = SelectedPostPublicityScope.PolicyType
                //};

                return publishPostDataModel;
            }, cancelationTokenSource.Token);

        private void ResetValidationObjects() {
            MainFeedMessage = _validationObjectFactory.GetValidatableObject<string>();
            MainFeedMessage.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = CreateFeedViewModel._FEED_POST_WITHOUT_TEXT_ERROR_MESSAGE });
            MainFeedMessage.Value = "";
        }

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) => ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync);

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) => ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync);

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync);

        private void OnTeamEventsNewTeamCreated(object sender, TeamDTO e) => ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync);

        private void OnTeamEventsTeamDeleted(object sender, TeamDTO e) => ExecuteActionWithBusy(UpdatePostPublicityScopesSourceAsync);
    }
}
