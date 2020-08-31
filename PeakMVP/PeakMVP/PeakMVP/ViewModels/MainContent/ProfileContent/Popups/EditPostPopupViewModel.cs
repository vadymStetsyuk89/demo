using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.AppEventsArguments.Posts;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Posts;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.ProfileContent.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileContent.Popups;
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
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ProfileContent.Popups {
    public class EditPostPopupViewModel : PopupBaseViewModel, IProfileInfoDependent, IInputForm {

        private static readonly string _FEED_POST_WITHOUT_TEXT_ERROR_MESSAGE = "Type what's in your mind.";
        private static readonly string _CHARGING_POST_ERROR_MESSAGE = "Can't edit post. Error was catched while preparing the post";

        private List<long> _currentMediaIds = new List<long>();
        private List<long> _attachedMediaIds = new List<long>();
        private List<long> _markedToRemoveMedias = new List<long>();

        private readonly IPostService _postService;
        private readonly IMediaPickerService _mediaPickerService;
        private readonly IMediaFactory _mediaFactory;
        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IProfileMediaService _profileMediaService;
        private readonly IFileDTOBuilder _fileDTOBuilder;

        private CancellationTokenSource _getPostPublicityScopesSourceCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _postChargingCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _editPostCancellationTokenSource = new CancellationTokenSource();

        public EditPostPopupViewModel(IProfileMediaService profileMediaService,
                                      IMediaPickerService mediaPickerService,
                                      IValidationObjectFactory validationObjectFactory,
                                      IPostService postService,
                                      IFileDTOBuilder fileDTOBuilder,
                                      IMediaFactory mediaFactory) {
            _profileMediaService = profileMediaService;
            _fileDTOBuilder = fileDTOBuilder;
            _validationObjectFactory = validationObjectFactory;
            _postService = postService;
            _mediaPickerService = mediaPickerService;
            _mediaFactory = mediaFactory;

            ResolveProfileInfo();
        }

        public ICommand EditPostCommand => new Command(async () => {
            if (!ValidateForm()) {
                await DialogService.ToastAsync(string.Format(MessageToEdit.Validations.First().ValidationMessage));
                return;
            }

            ResetCancellationTokenSource(ref _editPostCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _editPostCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                PublishPostDataModel publishPostDataModel = new PublishPostDataModel {
                    Files = _attachedMediaIds.ToArray(),
                    Text = !string.IsNullOrEmpty(MessageToEdit.Value) ? MessageToEdit.Value : "",
                    PostPolicyType = PostPolicyType.Public,
                    MarkedPostMedia = _markedToRemoveMedias.ToArray()
                };

                bool edited = await _postService.EditPostAsync(PostToEdit.Id, publishPostDataModel, cancellationTokenSource);

                if (edited) {
                    ClosePopupCommand.Execute(null);

                    PostWasEditedArgs postWasEditedArgs = new PostWasEditedArgs() { };
                    NavigationService.CurrentViewModelsNavigationStack.ForEach(vMB => vMB.InitializeAsync(postWasEditedArgs));
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

        public ICommand AttachPhotoCommand => new Command(async () => {
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
                            _attachedMediaIds.Add(media.Id);

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

            UpdateBusyVisualState(busyKey, false);
        });

        public ICommand AttachVideoCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                IPickedMediaFromGallery pickedMediaFromGallery = await DependencyService.Get<IPickMediaDependencyService>().PickVideoAsync();

                if (pickedMediaFromGallery.Completion) {
                    if (!(string.IsNullOrEmpty(pickedMediaFromGallery.DataBase64)) && !(string.IsNullOrEmpty(pickedMediaFromGallery.DataThumbnailBase64))) {

                        FileDTO fileDTO = new FileDTO { Base64 = pickedMediaFromGallery.DataBase64, Name = Path.GetFileName(pickedMediaFromGallery.FilePath) };

                        MediaDTO media = await _profileMediaService.UploadMediaToTrayAsync(fileDTO, new CancellationTokenSource());

                        if (media != null) {
                            _attachedMediaIds.Add(media.Id);

                            AttachedMedia.Add(_mediaFactory.BuidAttachedMedia(MediaType.Video, pickedMediaFromGallery.DataBase64, media, pickedMediaFromGallery.DataThumbnailBase64));
                        }
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public ICommand RemoveAttachedMediaCommand => new Command((object param) => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);
            try {
                AttachedFeedMedia removedMedia = param as AttachedFeedMedia;
                AttachedMedia.Remove(removedMedia);

                if (_attachedMediaIds.Any()) {

                    _profileMediaService.DeleteMediaFromTrayAsync(removedMedia.Id, new CancellationTokenSource());

                    _attachedMediaIds.Remove(removedMedia.Id);
                }

                if (_currentMediaIds.Contains(removedMedia.Id)) {
                    _currentMediaIds.Remove(removedMedia.Id);
                    _markedToRemoveMedias.Add(removedMedia.Id);
                }

            }
            catch (Exception ex) {
                Debug.WriteLine($"ERROR: {ex.Message}");
                Debugger.Break();
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public override Type RelativeViewType => typeof(EditPostPopup);

        public PostContentViewModel PostToEdit { get; private set; }

        private string _authorAvatar;
        public string AuthorAvatar {
            get => _authorAvatar;
            private set => SetProperty<string>(ref _authorAvatar, value);
        }

        private ValidatableObject<string> _messageToEdit;
        public ValidatableObject<string> MessageToEdit {
            get => _messageToEdit;
            set => SetProperty<ValidatableObject<string>>(ref _messageToEdit, value);
        }

        private ObservableCollection<AttachedFeedMedia> _attachedMedia = new ObservableCollection<AttachedFeedMedia>();
        public ObservableCollection<AttachedFeedMedia> AttachedMedia {
            get => _attachedMedia;
            private set => SetProperty<ObservableCollection<AttachedFeedMedia>>(ref _attachedMedia, value);
        }


        private PostPublicityScope _selectedPostPublicityScope;
        public PostPublicityScope SelectedPostPublicityScope {
            get => _selectedPostPublicityScope;
            set => SetProperty<PostPublicityScope>(ref _selectedPostPublicityScope, value);
        }

        private List<PostPublicityScope> _postPublicityScopesSource;
        public List<PostPublicityScope> PostPublicityScopesSource {
            get => _postPublicityScopesSource;
            set {
                SetProperty(ref _postPublicityScopesSource, value);

                if (value != null) {
                    ResolveSelectedPostPublicity();
                }
            }
        }

        public void ResetInputForm() {
            try {
                _currentMediaIds.Clear();
                if (_attachedMediaIds.Any()) {
                    foreach (long id in _attachedMediaIds) {
                        _profileMediaService.DeleteMediaFromTrayAsync(id, new CancellationTokenSource());
                    }
                    _attachedMediaIds.Clear();
                }
                _markedToRemoveMedias.Clear();

                PostToEdit = null;
                AttachedMedia = new ObservableCollection<AttachedFeedMedia>();
                ResetValidationObjects();
                SelectedPostPublicityScope = PostPublicityScopesSource?.FirstOrDefault();
            }
            catch (Exception ex) {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }

        public bool ValidateForm() {
            bool isValidResult = true;

            //isValidResult = MessageToEdit.Validate();

            return isValidResult;
        }

        public void ResolveProfileInfo() {
            AuthorAvatar = GlobalSettings.Instance.UserProfile.Avatar?.Url;
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostEditing += OnPostEventsRequestToStartPostEditing;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.PostEvents.RequestToStartPostEditing -= OnPostEventsRequestToStartPostEditing;
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getPostPublicityScopesSourceCancellationTokenSource);
            ResetCancellationTokenSource(ref _postChargingCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getPostPublicityScopesSourceCancellationTokenSource);
            ResetCancellationTokenSource(ref _postChargingCancellationTokenSource);
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
            }
        }

        private async void CharePostForEditing(PostContentViewModel targetPost) {
            ResetCancellationTokenSource(ref _postChargingCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _postChargingCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                ResetInputForm();

                PostToEdit = targetPost;
                MessageToEdit.Value = targetPost.Text;
                ResolveSelectedPostPublicity();

                //List<AttachedFeedMedia> alreadyAttachedFeedMedias = new List<AttachedFeedMedia>();

                //if (targetPost.Medias != null && targetPost.Medias.Any()) {
                //    await Task.Run(async () => {
                //        await targetPost.Medias?.ForEachAsync(async (m) => {
                //            alreadyAttachedFeedMedias.Add(_mediaFactory.BuidAttachedMedia(m));
                //        });
                //        await targetPost.Medias?.ForEachAsync(async (m) => {
                //            alreadyAttachedFeedMedias.Add(_mediaFactory.BuidAttachedMedia(m,
                //                await _mediaPickerService.ParseStreamToBase64(await _mediaPickerService.ExtractStreamFromMediaUrlAsync(m.Url)),
                //                await _mediaPickerService.ParseStreamToBase64(await _mediaPickerService.ExtractStreamFromMediaUrlAsync(m.ThumbnailUrl))));
                //        });

                //        alreadyAttachedFeedMedias.ForEach(a => a.IsCanBeDetached = true);
                //        ///
                //        /// Previous flow
                //        /// 
                //        //alreadyAttachedFeedMedias.ForEach(a => a.IsCanBeDetached = false);
                //    }, cancellationTokenSource.Token);
                //}
                if (targetPost.Medias != null && targetPost.Medias.Any()) {
                    targetPost.Medias?.ForEach(m => AttachedMedia.Add(_mediaFactory.BuidAttachedMedia(m)));
                }

                //alreadyAttachedFeedMedias.ForEach(a => AttachedMedia.Add(a));
                AttachedMedia.ForEach(i => _currentMediaIds.Add(i.Id));
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(_CHARGING_POST_ERROR_MESSAGE);
                UpdatePopupScopeVisibility(false);
            }

            UpdateBusyVisualState(busyKey, false);
        }

        private async void UpdatePostPublicityScopesSource() {
            ResetCancellationTokenSource(ref _getPostPublicityScopesSourceCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _getPostPublicityScopesSourceCancellationTokenSource;

            try {
                PostPublicityScopesSource = await _postService.GetPossiblePostPublicityScopesAsync(cancellationTokenSource);
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }
        }

        private void ResolveSelectedPostPublicity() {
            if (PostPublicityScopesSource != null && PostToEdit != null) {
                PostPublicityScope targetPublicityScope = PostPublicityScopesSource.FirstOrDefault(p => p.Title == PostToEdit.PublicityScopeName);

                if (targetPublicityScope != null) {
                    SelectedPostPublicityScope = targetPublicityScope;
                }
            } else if (PostPublicityScopesSource != null) {
                SelectedPostPublicityScope = PostPublicityScopesSource.FirstOrDefault();
            }
        }

        private void ResetValidationObjects() {
            MessageToEdit = _validationObjectFactory.GetValidatableObject<string>();
            MessageToEdit.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = _FEED_POST_WITHOUT_TEXT_ERROR_MESSAGE });
        }

        private Task<PublishPostDataModel> PrepareRequestPostDataModelAsync(CancellationTokenSource cancelationTokenSource) =>
            Task.Run(() => {

                //AttachedFileDataModel[] attachedFiles = AttachedMedia
                //    .Where(aM => aM.IsCanBeDetached)
                //    .Select<AttachedFeedMedia, AttachedFileDataModel>(aM => (aM.MediaType == MediaType.Picture)
                //        ? new AttachedFileDataModel() {
                //            File = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, aM.SourceBase64)
                //        }
                //        : new AttachedFileDataModel() {
                //            File = _fileDTOBuilder.BuidFileDTO(MediaType.Video, aM.SourceBase64),
                //            Thumbnail = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, aM.ThumbnailBase64)
                //        })
                //    .ToArray<AttachedFileDataModel>();

                //EditPostDataModel editPostDataModel = new EditPostDataModel();
                //editPostDataModel.Files = AttachedMedia.Select(m => m.Id).ToArray();
                //editPostDataModel.Text = !string.IsNullOrEmpty(MessageToEdit.Value) ? MessageToEdit.Value : "";

                //PublishPostDataModel publishPostDataModel = new PublishPostDataModel {
                //    Files = AttachedMedia.Select(m => m.Id).ToArray(),
                //    Text = !string.IsNullOrEmpty(MessageToEdit.Value) ? MessageToEdit.Value : "",
                //    PostPolicyType = PostPolicyType.Public,
                //    MarkedPostMedia = new object[] { }
                //};
                ///
                /// Uncoment if it will be necessary to change post publicity scope (while editing)
                /// At the moment `backend` doesn't implement this behaviour...
                /// 
                //editPostDataModel.GroupId = SelectedPostPublicityScope.Id;
                //editPostDataModel.PostPolicyType = SelectedPostPublicityScope.PolicyType.ToString();

                return new PublishPostDataModel();
            }, cancelationTokenSource.Token);

        private void OnPostEventsRequestToStartPostEditing(object sender, EditPostArgs e) {
            ShowPopupCommand.Execute(null);
            CharePostForEditing(e.TargetPost);

            ///
            /// Uncoment if it will be necessary to change post publicity scope (while editing)
            /// At the moment `backend` doesn't implement this behaviour...
            /// 
            //UpdatePostPublicityScopesSource();
        }
    }
}
