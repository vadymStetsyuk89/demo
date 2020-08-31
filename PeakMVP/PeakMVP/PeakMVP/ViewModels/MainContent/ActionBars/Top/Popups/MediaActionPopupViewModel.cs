using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Arguments.InitializeArguments.Post;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.Albums;
using PeakMVP.Views.CompoundedViews.MainContent.ActionBars.Top.Popups;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ActionBars.Top.Popups {
    public class MediaActionPopupViewModel : PopupBaseViewModel {

        private CancellationTokenSource _addPictureCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _addVideoCancellationTokenSource = new CancellationTokenSource();

        private readonly IProfileMediaService _profileMediaService;
        private readonly IMediaPickerService _mediaPickerService;
        private readonly IFileDTOBuilder _fileDTOBuilder;

        public MediaActionPopupViewModel(
            IProfileMediaService profileMediaService,
            IMediaPickerService mediaPickerService,
            IFileDTOBuilder fileDTOBuilder) {

            _profileMediaService = profileMediaService;
            _mediaPickerService = mediaPickerService;
            _fileDTOBuilder = fileDTOBuilder;
        }

        //public ICommand AddPictureCommand => new Command(async () => {
        //    ResetCancellationTokenSource(ref _addPictureCancellationTokenSource);
        //    CancellationTokenSource cancellationTokenSource = _addPictureCancellationTokenSource;

        //    Guid busyKey = Guid.NewGuid();
        //    UpdateBusyVisualState(busyKey, true);

        //    try {
        //        MediaFile pickedPhotoMediaFile = await _mediaPickerService.PickPhotoAsync();

        //        if (pickedPhotoMediaFile != null) {
        //            Stream pickedPhotoStream = pickedPhotoMediaFile.GetStream();
        //            string base64Photo = await _mediaPickerService.ParseStreamToBase64(pickedPhotoStream);
        //            pickedPhotoStream.Close();
        //            pickedPhotoStream.Dispose();

        //            if (!(string.IsNullOrEmpty(base64Photo))) {
        //                MediaDTO media = await AddMediaAsync(base64Photo, ProfileMediaService.IMAGE_MEDIA_TYPE, cancellationTokenSource);

        //                if (media != null) {
        //                    GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAddedInvoke(this, media);
        //                }
        //            }
        //        }
        //    }
        //    catch (OperationCanceledException) { }
        //    catch (ObjectDisposedException) { }
        //    catch (ServiceAuthenticationException) { }
        //    catch (Exception exc) {
        //        Crashes.TrackError(exc);

        //        await DialogService.ToastAsync(exc.Message);
        //    }

        //    UpdateBusyVisualState(busyKey, false);
        //});

        public ICommand TakePictureCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                MediaFile takedPhotoMediaFile = await _mediaPickerService.TakePhotoAsync();

                if (takedPhotoMediaFile != null) {
                    PickedMedia = await _fileDTOBuilder.BuildAttachedPictureAsync(takedPhotoMediaFile.GetStream());
                    takedPhotoMediaFile.Dispose();
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

        public ICommand TakeVideoCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                MediaFile takedVideoMediaFile = await _mediaPickerService.TakeVideoAsync();

                if (takedVideoMediaFile != null) {
                    PickedMedia = await _fileDTOBuilder.BuildAttachedVideoAsync(takedVideoMediaFile.GetStream(), takedVideoMediaFile.Path);
                    takedVideoMediaFile.Dispose();
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

        public ICommand PickMediaCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                IPickedMediaFromGallery pickedMediaResult = await DependencyService.Get<IPickMediaDependencyService>().PickMediaAsync();

                if (pickedMediaResult.Completion) {
                    if (!string.IsNullOrEmpty(pickedMediaResult.DataBase64)) {
                        PickedMedia = await _fileDTOBuilder.BuildAttachedMediaAsync(pickedMediaResult);
                    }
                } else {
                    if (pickedMediaResult.Exception != null) {
                        throw new InvalidOperationException(
                            string.Format("{0}. {1}",
                                ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE,
                                pickedMediaResult.ErrorMessage ?? ""), pickedMediaResult.Exception);
                    } else if (!(string.IsNullOrEmpty(pickedMediaResult.ErrorMessage))) {
                        throw new InvalidOperationException(string.Format("{0}. {1}", ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE, pickedMediaResult.ErrorMessage));
                    } else {
                        throw new InvalidOperationException(ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE);
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

        public ICommand AddToAlbumOfferCommand => new Command(async () => {
            if (PickedMedia != null) {
                ResetCancellationTokenSource(ref _addVideoCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _addVideoCancellationTokenSource;

                Guid busyKey = Guid.NewGuid();
                UpdateBusyVisualState(busyKey, true);

                try {
                    //ProfileMediaDTO media = await AddMediaAsync(PickedMedia, cancellationTokenSource);
                    AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
                        File = PickedMedia.File,
                        Thumbnail = PickedMedia.Thumbnail,
                        MediaType = PickedMedia.MimeType
                    };

                    bool addedToProfile = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

                    if (addedToProfile) {
                        ClosePopupCommand.Execute(null);
                        //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.OnNewMediaAdded();
                        //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAddedInvoke(this, media);
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
        });

        public ICommand AttachToTheNewPostFeedOfferCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);
            try {
                if (PickedMedia != null) {
                    MediaDTO mediaDTO = await _profileMediaService.UploadMediaToTrayAsync(PickedMedia.File, new CancellationTokenSource());
                    GlobalSettings.Instance.AppMessagingEvents.PostEvents.AttachMediaTotheNewPostOfferInvoke(this, new AttachExternalMediaToNewPostArgs() { NewMedia = PickedMedia, MediaDTO = mediaDTO });
                    ClosePopupCommand.Execute(null);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine($"ERROR: {ex.Message}");
                Debugger.Break();
            }
            UpdateBusyVisualState(busyKey, false);
        });

        private AttachedFileDataModel _pickedMedia;
        public AttachedFileDataModel PickedMedia {
            get => _pickedMedia;
            private set => SetProperty<AttachedFileDataModel>(ref _pickedMedia, value);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _addPictureCancellationTokenSource);
            ResetCancellationTokenSource(ref _addVideoCancellationTokenSource);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _addPictureCancellationTokenSource);
            ResetCancellationTokenSource(ref _addVideoCancellationTokenSource);
        }

        public override Type RelativeViewType => typeof(MediaActionPopupView);

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            PickedMedia = null;
        }

        //private Task<ProfileMediaDTO> AddMediaAsync(AttachedFileDataModel attachedFileData, CancellationTokenSource cancellationTokenSource) =>
        //    Task<ProfileMediaDTO>.Run(async () => {
        //        AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
        //            File = attachedFileData.File,
        //            Thumbnail = attachedFileData.Thumbnail,
        //            MediaType = attachedFileData.MimeType
        //        };

        //        ProfileMediaDTO media = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

        //        return media;
        //    }, cancellationTokenSource.Token);

        //private Task<MediaDTO> AddMediaAsync(string base64Source, string base64SourceThumbnail, string mediaType, CancellationTokenSource cancellationTokenSource) =>
        //    Task<MediaDTO>.Run(async () => {
        //        AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {

        //            ///
        //            /// TODO: resolve name in morerobust way
        //            /// 
        //            File = new FileDTO() {
        //                Base64 = base64Source,
        //                Name = string.Format("{0}.{1}", Guid.NewGuid(), (mediaType == ProfileMediaService.IMAGE_MEDIA_TYPE) ? ProfileMediaService.PNG_IMAGE_FORMAT : ProfileMediaService.MP4_VIDEO_FORMAT)
        //            },
        //            Thumbnail = new FileDTO() {
        //                Base64 = base64SourceThumbnail
        //            },
        //            MediaType = mediaType
        //        };

        //        MediaDTO media = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

        //        return media;
        //    }, cancellationTokenSource.Token);
    }
}
