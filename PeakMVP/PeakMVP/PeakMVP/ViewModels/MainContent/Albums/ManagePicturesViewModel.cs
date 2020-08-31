using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.MediaViewers;
using PeakMVP.ViewModels.MainContent.MediaViewers.Arguments;
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

namespace PeakMVP.ViewModels.MainContent.Albums {
    public class ManagePicturesViewModel : PopupBaseViewModel, IAskToRefresh {

        private readonly IMediaPickerService _mediaPickerService;
        private readonly IProfileMediaService _profileMediaService;
        private readonly IFileDTOBuilder _fileDTOBuilder;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getProfileMediaCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _addPictureCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _deletePictureCancellationTokenSource = new CancellationTokenSource();

        public ManagePicturesViewModel(
            IMediaPickerService mediaPickerService,
            IProfileMediaService profileMediaService,
            IFileDTOBuilder fileDTOBuilder,
            IStateService stateService) {

            _mediaPickerService = mediaPickerService;
            _profileMediaService = profileMediaService;
            _fileDTOBuilder = fileDTOBuilder;
            _stateService = stateService;

            AddedPictures = new ObservableCollection<ProfileMediaDTO>();
            IsAnyPictures = AddedPictures.Any();
        }

        public ICommand WatchPicturesCommand => new Command((object parameter) => {
            NavigationService.NavigateToAsync<PicturesViewerViewModel>(new StartWatchingPicturesArgs() {
                TargetMedia = (ProfileMediaDTO)parameter,
                MediasSource = AddedPictures.ToArray<ProfileMediaDTO>()
            });
        });

        public ICommand AddPictureCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _addPictureCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _addPictureCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                MediaFile pickedPhotoMediaFile = await _mediaPickerService.PickPhotoAsync();

                if (pickedPhotoMediaFile != null) {
                    Stream pickedPhotoStream = pickedPhotoMediaFile.GetStream();
                    string base64Photo = await _mediaPickerService.ParseStreamToBase64(pickedPhotoStream);
                    pickedPhotoStream.Close();
                    pickedPhotoStream.Dispose();

                    if (!(string.IsNullOrEmpty(base64Photo))) {
                        AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
                            File = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, base64Photo),
                            MediaType = ProfileMediaService.IMAGE_MEDIA_TYPE
                        };

                        bool addedToProfile = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

                        if (addedToProfile) {
                            //AddedPictures.Add(media);
                            //IsAnyPictures = AddedPictures.Any();

                            await DialogService.ToastAsync(ProfileMediaService.ADD_IMAGE_SUCCESSFUL_COMPLETION_MESSAGE);

                            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.OnNewMediaAdded();
                            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAddedInvoke(this, media);
                        }
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

        public ICommand DeletePictureCommand => new Command((object param) => {
            ResetCancellationTokenSource(ref _deletePictureCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _deletePictureCancellationTokenSource;

            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                _profileMediaService.RemoveProfileMediaById(((ProfileMediaDTO)param).Id, cancellationTokenSource);

                AddedPictures.Remove(((ProfileMediaDTO)param));
                IsAnyPictures = AddedPictures.Any();

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
        });

        public override Type RelativeViewType => typeof(AddPicturesPopup);

        private bool _isAnyPictures;
        public bool IsAnyPictures {
            get => _isAnyPictures;
            set => SetProperty(ref _isAnyPictures, value);
        }

        private ObservableCollection<ProfileMediaDTO> _addedPictures;
        public ObservableCollection<ProfileMediaDTO> AddedPictures {
            get => _addedPictures;
            set => SetProperty(ref _addedPictures, value);
        }

        public Task AskToRefreshAsync() => GetProfileMediaAsync();

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetProfileMediaAsync);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getProfileMediaCancellationTokenSource);
            ResetCancellationTokenSource(ref _addPictureCancellationTokenSource);
            ResetCancellationTokenSource(ref _deletePictureCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getProfileMediaCancellationTokenSource);
            ResetCancellationTokenSource(ref _addPictureCancellationTokenSource);
            ResetCancellationTokenSource(ref _deletePictureCancellationTokenSource);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAdded += OnMediaEventsNewMediaAdded;
            //GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAdded += MediaEvents_NewMediaAdded;
            GlobalSettings.Instance.AppMessagingEvents.MediaEvents.MediaDeleted += OnMediaEventsMediaDeleted;

            _stateService.ChangedProfile += OnStateServiceChangedProfile;
        }

        private void MediaEvents_NewMediaAdded(object sender, EventArgs e) {

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
                    ObservableCollection<ProfileMediaDTO> resolvedAddedPictures = (foundMedia != null && foundMedia.Any()) ? new ObservableCollection<ProfileMediaDTO>(foundMedia.Where<ProfileMediaDTO>(media => media.Mime == ProfileMediaService.MIME_IMAGE_TYPE)) : new ObservableCollection<ProfileMediaDTO>();

                    Device.BeginInvokeOnMainThread(() => {
                        AddedPictures = resolvedAddedPictures;
                        IsAnyPictures = AddedPictures.Any();
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Device.BeginInvokeOnMainThread(() => {
                        AddedPictures.Clear();
                        IsAnyPictures = AddedPictures.Any();
                    });

                    await DialogService.ToastAsync(string.Format("{0}", exc.Message));
                }
            });

        //private Task<ProfileMediaDTO> AddMediaAsync(string base64Source, string mediaType, CancellationTokenSource cancellationTokenSource) =>
        //    Task<ProfileMediaDTO>.Run(async () => {
        //        AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
        //            File = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, base64Source),
        //            MediaType = mediaType
        //        };

        //        ProfileMediaDTO media = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

        //        return media;
        //    }, cancellationTokenSource.Token);

        private void OnMediaEventsNewMediaAdded(object sender, ProfileMediaDTO e) {
            if (e.Mime == ProfileMediaService.MIME_IMAGE_TYPE) {
                ProfileMediaDTO alsoExistMedia = AddedPictures?.FirstOrDefault(media => media.Id == e.Id);

                if (alsoExistMedia == null) {
                    AddedPictures.Add(e);
                    IsAnyPictures = AddedPictures.Any();
                }
            }
        }

        private void OnMediaEventsMediaDeleted(object sender, ProfileMediaDTO e) {
            if (e.Mime == ProfileMediaService.MIME_IMAGE_TYPE) {
                ProfileMediaDTO mediaToRemove = AddedPictures?.FirstOrDefault<ProfileMediaDTO>(media => media.Id == e.Id);

                if (mediaToRemove != null) {
                    AddedPictures.Remove(mediaToRemove);
                    IsAnyPictures = AddedPictures.Any();
                }
            }
        }

        private void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) => ExecuteActionWithBusy(GetProfileMediaAsync);
    }
}
