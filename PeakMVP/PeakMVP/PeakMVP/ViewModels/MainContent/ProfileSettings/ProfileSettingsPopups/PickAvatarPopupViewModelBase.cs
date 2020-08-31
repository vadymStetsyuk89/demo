using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Identities.Medias;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileSettings.Popups;
using Plugin.Media.Abstractions;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings.ProfileSettingsPopups {
    public abstract class PickAvatarPopupViewModelBase : PopupBaseViewModel {

        protected static readonly string NO_IMAGE_FILE_CHOSEN_ERROR_MESSAGE = "No image file choosen";

        protected readonly IMediaPickerService _mediaPickerService;
        protected readonly IProfileService _profileService;
        protected readonly IIdentityUtilService _identityUtilService;
        protected readonly IProfileMediaService _profileMediaService;
        protected readonly IFileDTOBuilder _fileDTOBuilder;

        public PickAvatarPopupViewModelBase(
            IMediaPickerService mediaPickerService,
            IProfileService profileService,
            IIdentityUtilService identityUtilService,
            IProfileMediaService profileMediaService,
            IFileDTOBuilder fileDTOBuilder) {

            _mediaPickerService = mediaPickerService;
            _profileService = profileService;
            _identityUtilService = identityUtilService;
            _profileMediaService = profileMediaService;
            _fileDTOBuilder = fileDTOBuilder;
        }

        public abstract ICommand SaveAvatarCommand { get; }

        public ICommand PickAvatarImageCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            try {
                MediaFile pickedPhoto = await _mediaPickerService.PickPhotoAsync();

                if (pickedPhoto != null) {
                    PickedAvatar = await _mediaPickerService.BuildPickedImageAsync(pickedPhoto);
                    pickedPhoto.Dispose();
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public override Type RelativeViewType => typeof(PickAvatarPopup);

        private PickedImage _pickedAvatar;
        public PickedImage PickedAvatar {
            get => _pickedAvatar;
            protected set => SetProperty<PickedImage>(ref _pickedAvatar, value);
        }

        public void ResetValues() {
            PickedAvatar = null;
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (IsPopupVisible) {
                ResetValues();
            }

            Dispose();
        }
    }
}
