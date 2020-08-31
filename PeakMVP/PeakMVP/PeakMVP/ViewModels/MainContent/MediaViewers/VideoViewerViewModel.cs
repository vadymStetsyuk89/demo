using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.MediaViewers {
    public class VideoViewerViewModel : LoggedContentPageViewModel {

        public VideoViewerViewModel() {
            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;
        }

        private ProfileMediaDTO _targetMedia;
        public ProfileMediaDTO TargetMedia {
            get => _targetMedia;
            set => SetProperty<ProfileMediaDTO>(ref _targetMedia, value);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is ProfileMediaDTO) {
                InitializeMedia((ProfileMediaDTO)navigationData);
            }

            return base.InitializeAsync(navigationData);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged += OnProfileSettingsEventsAppBackgroundImageChanged;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged -= OnProfileSettingsEventsAppBackgroundImageChanged;
        }

        private async void InitializeMedia(ProfileMediaDTO targetMedia) {

            if (targetMedia.Mime == ProfileMediaService.MIME_VIDEO_TYPE) {
                TargetMedia = targetMedia;
            }
            else {
                await NavigationService.GoBackAsync();
            }
        }

        private void OnProfileSettingsEventsAppBackgroundImageChanged(object sender, EventArgs e) => AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;
    }
}