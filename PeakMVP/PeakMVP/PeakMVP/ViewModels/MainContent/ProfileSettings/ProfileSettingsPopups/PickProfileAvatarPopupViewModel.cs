using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings.ProfileSettingsPopups {
    public class PickProfileAvatarPopupViewModel : PickAvatarPopupViewModelBase {

        private CancellationTokenSource _setAvatarCancellationTokenSource = new CancellationTokenSource();

        public PickProfileAvatarPopupViewModel(
            IMediaPickerService mediaPickerService,
            IProfileService profileService,
            IIdentityUtilService identityUtilService,
            IProfileMediaService profileMediaService,
            IFileDTOBuilder fileDTOBuilder)
            : base(
                  mediaPickerService,
                  profileService,
                  identityUtilService,
                  profileMediaService,
                  fileDTOBuilder) { }

        //public ICommand SaveAvatarCommand => new Command(async () => {
        //    Guid busyKey = Guid.NewGuid();
        //    UpdateBusyVisualState(busyKey, true);

        //    ResetCancellationTokenSource(ref _setAvatarCancellationTokenSource);
        //    CancellationTokenSource cancellationTokenSource = _setAvatarCancellationTokenSource;

        //    try {
        //        if (PickedAvatar == null) {
        //            throw new InvalidOperationException(PickAvatarPopupViewModel._NO_IMAGE_FILE_CHOSEN_ERROR_MESSAGE);
        //        }
        //        else {
        //            ProfileMediaDTO newAvatarMediaDTO = await _profileService.SetAvatarAsync(PickedAvatar.DataBase64, cancellationTokenSource);

        //            _identityUtilService.ChargeUserProfileAvatar(newAvatarMediaDTO);

        //            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdatedInvoke(this, new ProfileUpdatedArgs());

        //            ClosePopupCommand.Execute(null);
        //        }
        //    }
        //    catch (OperationCanceledException) { }
        //    catch (ObjectDisposedException) { }
        //    catch (ServiceAuthenticationException) { }
        //    catch (Exception exc) {
        //        PickedAvatar = null;
        //        await DialogService.ToastAsync(exc.Message);
        //    }

        //    UpdateBusyVisualState(busyKey, false);
        //});

        public override ICommand SaveAvatarCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            ResetCancellationTokenSource(ref _setAvatarCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _setAvatarCancellationTokenSource;

            try {
                if (PickedAvatar == null) {
                    throw new InvalidOperationException(PickAvatarPopupViewModelBase.NO_IMAGE_FILE_CHOSEN_ERROR_MESSAGE);
                }
                else {
                    FileDTO avatarFile = _fileDTOBuilder.BuidFileDTO(PickedAvatar);

                    if (avatarFile != null) {
                        MediaDTO media = await _profileMediaService.UploadMediaToTrayAsync(avatarFile, cancellationTokenSource);

                        if (media != null) {
                            await _profileService.SetAvatarAsync(media.Id, cancellationTokenSource);
                            _identityUtilService.ChargeUserProfileAvatar(media);

                            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdatedInvoke(this, new ProfileUpdatedArgs());
                        }
                        else {
                            throw new InvalidOperationException(ProfileMediaService.CANT_UPLOAD_MEDIA_COMMON_ERROR);
                        }
                    }

                    ClosePopupCommand.Execute(null);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                PickedAvatar = null;
                await DialogService.ToastAsync(exc.Message);
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _setAvatarCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _setAvatarCancellationTokenSource);
        }
    }
}
