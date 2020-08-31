using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Validations;
using PeakMVP.ViewModels.Base;
using Plugin.Media.Abstractions;
using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations {
    public abstract class SelfInformationBase : NestedViewModel, ISelfInformationViewModel {

        protected static readonly string _APP_BACKGROUND_IMAGE_CHANGED_INFO_MESSAGE = "App background image updated";

        private readonly IProfileService _profileService;
        private readonly IMediaPickerService _mediaPickerService;
        private readonly IFileDTOBuilder _fileDTOBuilder;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly IProfileMediaService _profileMediaService;

        protected readonly IValidationObjectFactory _validationObjectFactory;

        private CancellationTokenSource _setAppBackggroundImageCancellationTokenSource = new CancellationTokenSource();

        public SelfInformationBase(
            IValidationObjectFactory validationObjectFactory,
            IProfileService profileService,
            IMediaPickerService mediaPickerService,
            IFileDTOBuilder fileDTOBuilder,
            IIdentityUtilService identityUtilService,
            IProfileMediaService profileMediaService) {

            _validationObjectFactory = validationObjectFactory;
            _profileService = profileService;
            _mediaPickerService = mediaPickerService;
            _fileDTOBuilder = fileDTOBuilder;
            _identityUtilService = identityUtilService;
            _profileMediaService = profileMediaService;
        }

        public ICommand SetAppBackgroundImageCommand => new Command(async () => {
            ResetCancellationTokenSource(ref _setAppBackggroundImageCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _setAppBackggroundImageCancellationTokenSource;

            Guid busyGuid = Guid.NewGuid();
            UpdateBusyVisualState(busyGuid, true);

            try {
                MediaFile mediaFile = await _mediaPickerService.PickPhotoAsync();

                if (mediaFile != null) {
                    FileDTO file = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, await _mediaPickerService.ParseStreamToBase64(mediaFile.GetStream()));

                    if (file != null) {
                        MediaDTO uploadedMedia = await _profileMediaService.UploadMediaToTrayAsync(file, cancellationTokenSource);

                        if (uploadedMedia != null) {
                            if (await _profileService.SetAppBackgroundImage(uploadedMedia.Id, cancellationTokenSource)) {
                                AppBackgroundImage = uploadedMedia;

                                _identityUtilService.ChargeUserProfileAppBackgroundImage(AppBackgroundImage);

                                await DialogService.ToastAsync(_APP_BACKGROUND_IMAGE_CHANGED_INFO_MESSAGE);

                                GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChangedInvoke(this, new EventArgs());
                            }
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

            UpdateBusyVisualState(busyGuid, false);
        });

        //public ICommand SetAppBackgroundImageCommand => new Command(async () => {
        //    ResetCancellationTokenSource(ref _setAppBackggroundImageCancellationTokenSource);
        //    CancellationTokenSource cancellationTokenSource = _setAppBackggroundImageCancellationTokenSource;

        //    Guid busyGuid = Guid.NewGuid();
        //    UpdateBusyVisualState(busyGuid, true);

        //    try {
        //        MediaFile mediaFile = await _mediaPickerService.PickPhotoAsync();

        //        if (mediaFile != null) {
        //            AppBackgroundImage = await _profileService.SetAppBackgroundImage(
        //                new BackgroundImageDataModel() {
        //                    File = _fileDTOBuilder.BuidFileDTO(MediaType.Picture, await _mediaPickerService.ParseStreamToBase64(mediaFile.GetStream()))
        //                }, cancellationTokenSource);

        //            _identityUtilService.ChargeUserProfileAppBackgroundImage(AppBackgroundImage);

        //            await DialogService.ToastAsync(_APP_BACKGROUND_IMAGE_CHANGED_INFO_MESSAGE);

        //            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChangedInvoke(this, new EventArgs());
        //        }
        //    }
        //    catch (OperationCanceledException) { }
        //    catch (ObjectDisposedException) { }
        //    catch (ServiceAuthenticationException) { }
        //    catch (Exception exc) {
        //        Crashes.TrackError(exc);

        //        await DialogService.ToastAsync(exc.Message);
        //    }

        //    UpdateBusyVisualState(busyGuid, false);
        //});

        private ValidatableObject<string> _aboutYou;
        public ValidatableObject<string> AboutYou {
            get => _aboutYou;
            set => SetProperty<ValidatableObject<string>>(ref _aboutYou, value);
        }

        private ValidatableObject<string> _mySports;
        public ValidatableObject<string> MySports {
            get => _mySports;
            set => SetProperty<ValidatableObject<string>>(ref _mySports, value);
        }

        private MediaDTO _appBackgroundImage;
        public MediaDTO AppBackgroundImage {
            get => _appBackgroundImage;
            private set {
                SetProperty<MediaDTO>(ref _appBackgroundImage, value);
            }
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _setAppBackggroundImageCancellationTokenSource);
        }

        public virtual SetProfileDataModel GetInputValues() {
            SetProfileDataModel setProfileDataModel = new SetProfileDataModel();
            setProfileDataModel.About = AboutYou.Value;
            setProfileDataModel.MySports = MySports.Value;

            return setProfileDataModel;
        }

        public virtual bool ValidateForm() {
            bool isValidResult = false;

            AboutYou.Validate();
            MySports.Validate();

            isValidResult = AboutYou.IsValid && MySports.IsValid;

            return isValidResult;
        }

        public virtual void ResetInputForm() {
            ResetValidationObjects();
        }

        public virtual void ResolveProfileInfo() {
            AboutYou.Value = GlobalSettings.Instance.UserProfile.About;
            MySports.Value = GlobalSettings.Instance.UserProfile.MySports;
            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage;
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _setAppBackggroundImageCancellationTokenSource);
        }

        protected virtual void ResetValidationObjects() {
            AboutYou = _validationObjectFactory.GetValidatableObject<string>();
            MySports = _validationObjectFactory.GetValidatableObject<string>();
        }
    }
}
