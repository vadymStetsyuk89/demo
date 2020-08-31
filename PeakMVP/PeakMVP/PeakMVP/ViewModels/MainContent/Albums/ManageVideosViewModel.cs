using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.MediaViewers;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileContent.Popups;
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

namespace PeakMVP.ViewModels.MainContent.Albums {
    public class ManageVideosViewModel : PopupBaseViewModel, IAskToRefresh {

        public static readonly string COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE = "Can't attach current video";
        public static readonly string COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE = "Can't attach media";

        private readonly IMediaPickerService _mediaPickerService;
        private readonly IProfileMediaService _profileMediaService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getProfileMediaCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _addVideoCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _deleteVideoCancellationTokenSource = new CancellationTokenSource();

        public ManageVideosViewModel(
            IMediaPickerService mediaPickerService,
            IProfileMediaService profileMediaService,
            IStateService stateService) {

            _mediaPickerService = mediaPickerService;
            _profileMediaService = profileMediaService;
            _stateService = stateService;

            IsAnyVideos = AddedVideos.Any();
        }

        public ICommand DeleteVideoCommand => new Command((object param) => {
            if (AddedVideos.Contains((ProfileMediaDTO)param)) {
                ResetCancellationTokenSource(ref _deleteVideoCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _deleteVideoCancellationTokenSource;

                Guid busyKey = Guid.NewGuid();
                UpdateBusyVisualState(busyKey, true);

                try {
                    _profileMediaService.RemoveProfileMediaById(((ProfileMediaDTO)param).Id, cancellationTokenSource);
                    AddedVideos.Remove(((ProfileMediaDTO)param));
                    IsAnyVideos = AddedVideos.Any();

                    GlobalSettings.Instance.AppMessagingEvents.MediaEvents.MediaDeletedInvoke(this, ((ProfileMediaDTO)param));
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debug.WriteLine($"ERROR: -{exc.Message}");
                    Debugger.Break();
                }

                UpdateBusyVisualState(busyKey, false);
            }

            IsAnyVideos = AddedVideos.Any();
        });

        public ICommand WatchVideoCommand => new Command(async (object param) => {
            await NavigationService.NavigateToAsync<VideoViewerViewModel>(param);
        });

        public ICommand AddVideoCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _addVideoCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _addVideoCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                IPickedMediaFromGallery pickedVideoResult = await DependencyService.Get<IPickMediaDependencyService>().PickVideoAsync();

                if (pickedVideoResult.Completion) {
                    if (!(string.IsNullOrEmpty(pickedVideoResult.DataBase64)) && !(string.IsNullOrEmpty(pickedVideoResult.DataThumbnailBase64))) {
                        //ProfileMediaDTO media = await AddMediaAsync(pickedVideoResult.DataBase64, pickedVideoResult.DataThumbnailBase64, ProfileMediaService.VIDEO_MEDIA_TYPE, cancellationTokenSource);

                        AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
                            File = new FileDTO() {
                                Base64 = pickedVideoResult.DataBase64,
                                Name = string.Format("{0}.{1}", Guid.NewGuid(), ProfileMediaService.MP4_VIDEO_FORMAT)
                            },
                            Thumbnail = new FileDTO() {
                                Base64 = pickedVideoResult.DataThumbnailBase64
                            },
                            MediaType = ProfileMediaService.VIDEO_MEDIA_TYPE
                        };

                        bool addedToProfile = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

                        if (addedToProfile) {
                            //AddedVideos.Add(media);
                            //IsAnyVideos = AddedVideos.Any();

                            await DialogService.ToastAsync(ProfileMediaService.ADD_VIDEO_SUCCESSFUL_COMPLETION_MESSAGE);

                            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.OnNewMediaAdded();
                            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAddedInvoke(this, media);
                        }
                    }
                }
                else {
                    if (pickedVideoResult.Exception != null) {
                        throw new InvalidOperationException(
                            string.Format("{0}. {1}", COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE, pickedVideoResult.ErrorMessage ?? ""), pickedVideoResult.Exception);
                    }
                    else if (!(string.IsNullOrEmpty(pickedVideoResult.ErrorMessage))) {
                        throw new InvalidOperationException(string.Format("{0}. {1}", COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE, pickedVideoResult.ErrorMessage));
                    }
                    else {
                        throw new InvalidOperationException(COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE);
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

            IsAnyVideos = AddedVideos.Any();
            UpdateBusyVisualState(busyKey, false);
        });

        public override Type RelativeViewType => typeof(AddVideosPopup);

        private ObservableCollection<ProfileMediaDTO> _addedVideos = new ObservableCollection<ProfileMediaDTO>();
        public ObservableCollection<ProfileMediaDTO> AddedVideos {
            get => _addedVideos;
            set => SetProperty(ref _addedVideos, value);
        }

        private bool _isAnyVideos;
        public bool IsAnyVideos {
            get => _isAnyVideos;
            set => SetProperty(ref _isAnyVideos, value);
        }

        public Task AskToRefreshAsync() => GetProfileMediaAsync();

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetProfileMediaAsync);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getProfileMediaCancellationTokenSource);
            ResetCancellationTokenSource(ref _addVideoCancellationTokenSource);
            ResetCancellationTokenSource(ref _deleteVideoCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getProfileMediaCancellationTokenSource);
            ResetCancellationTokenSource(ref _addVideoCancellationTokenSource);
            ResetCancellationTokenSource(ref _deleteVideoCancellationTokenSource);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAdded += OnMediaEventsNewMediaAdded;
            GlobalSettings.Instance.AppMessagingEvents.MediaEvents.MediaDeleted += OnMediaEventsMediaDeleted;

            _stateService.ChangedProfile += OnStateServiceChangedProfile;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAdded -= OnMediaEventsNewMediaAdded;
            GlobalSettings.Instance.AppMessagingEvents.MediaEvents.MediaDeleted -= OnMediaEventsMediaDeleted;

            _stateService.ChangedProfile -= OnStateServiceChangedProfile;
        }

        private Task GetProfileMediaAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getProfileMediaCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getProfileMediaCancellationTokenSource;

                try {
                    IEnumerable<ProfileMediaDTO> foundMedia = await _profileMediaService.GetProfileMedia(cancellationTokenSource);
                    ObservableCollection<ProfileMediaDTO> resolvedAddedVideos = (foundMedia != null && foundMedia.Any()) ? new ObservableCollection<ProfileMediaDTO>(foundMedia.Where<ProfileMediaDTO>(media => media.Mime == ProfileMediaService.MIME_VIDEO_TYPE)) : new ObservableCollection<ProfileMediaDTO>();

                    Device.BeginInvokeOnMainThread(() => {
                        AddedVideos = resolvedAddedVideos;
                        IsAnyVideos = AddedVideos.Any();
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Device.BeginInvokeOnMainThread(() => {
                        AddedVideos.Clear();
                        IsAnyVideos = AddedVideos.Any();
                    });

                    await DialogService.ToastAsync(string.Format("{0}", exc.Message));
                }
            });

        private Task<ProfileMediaDTO> AddMediaAsync(string base64Source, string base64SourceThumbnail, string mediaType, CancellationTokenSource cancellationTokenSource) =>
            Task<ProfileMediaDTO>.Run(() => {
                AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
                    File = new Models.Rests.DTOs.FileDTO() {
                        Base64 = base64Source,
                        Name = string.Format("{0}.{1}", Guid.NewGuid(), ProfileMediaService.MP4_VIDEO_FORMAT)
                    },
                    Thumbnail = new Models.Rests.DTOs.FileDTO() {
                        Base64 = base64SourceThumbnail
                    },
                    MediaType = mediaType
                };

                ProfileMediaDTO media = null;
                //ProfileMediaDTO media = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

                return media;
            }, cancellationTokenSource.Token);

        private Task<Stream> GetVideoThumbnailStreamAsync(string path) =>
            Task<Stream>.Run(() => {
                Stream thumbnailStream = null;

                try {
                    thumbnailStream = DependencyService.Get<IPickMediaDependencyService>().GetThumbnail(path);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }

                return thumbnailStream;
            });

        private void OnMediaEventsNewMediaAdded(object sender, ProfileMediaDTO e) {
            if (e.Mime == ProfileMediaService.MIME_VIDEO_TYPE) {
                ProfileMediaDTO alsoExistMedia = AddedVideos?.FirstOrDefault<ProfileMediaDTO>(media => media.Id == e.Id);

                if (alsoExistMedia == null) {
                    AddedVideos.Add(e);
                    IsAnyVideos = AddedVideos.Any();
                }
            }
        }

        private void OnMediaEventsMediaDeleted(object sender, ProfileMediaDTO e) {
            if (e.Mime == ProfileMediaService.MIME_VIDEO_TYPE) {
                ProfileMediaDTO mediaToRemove = AddedVideos?.FirstOrDefault<ProfileMediaDTO>(media => media.Id == e.Id);

                if (mediaToRemove != null) {
                    AddedVideos.Remove(mediaToRemove);
                    IsAnyVideos = AddedVideos.Any();
                }
            }
        }

        private void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) => ExecuteActionWithBusy(GetProfileMediaAsync);
    }
}