using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.MediaViewers.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.MediaViewers {
    public class PicturesViewerViewModel : LoggedContentPageViewModel {

        public PicturesViewerViewModel() {
            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;
        }

        private string _itemsCounterOutput = "0/0";
        public string ItemsCounterOutput {
            get => _itemsCounterOutput;
            set => SetProperty<string>(ref _itemsCounterOutput, value);
        }

        private bool _isCarouselContentVisible;
        public bool IsCarouselContentVisible {
            get => _isCarouselContentVisible;
            set => SetProperty<bool>(ref _isCarouselContentVisible, value);
        }

        private int _currentlyViewingMediaSourceIndex;
        public int CurrentlyViewingMediaSourceIndex {
            get => _currentlyViewingMediaSourceIndex;
            set {
                SetProperty<int>(ref _currentlyViewingMediaSourceIndex, value);

                ItemsCounterOutput = string.Format("{0}/{1}", value + 1, MediaSource.Count);
            }
        }

        private List<ProfileMediaDTO> _mediaSource;
        public List<ProfileMediaDTO> MediaSource {
            get => _mediaSource;
            set => SetProperty<List<ProfileMediaDTO>>(ref _mediaSource, value);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is StartWatchingPicturesArgs) {
                InitializeStartWatchingPicturesArgs((StartWatchingPicturesArgs)navigationData);
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

        private void InitializeStartWatchingPicturesArgs(StartWatchingPicturesArgs startWatchingPicturesArgs) {
            IsBusy = true;

            MediaSource = startWatchingPicturesArgs.MediasSource.ToList<ProfileMediaDTO>();

            CurrentlyViewingMediaSourceIndex = startWatchingPicturesArgs.MediasSource.ToList().IndexOf(startWatchingPicturesArgs.TargetMedia);
            IsCarouselContentVisible = true;

            IsBusy = false;
        }

        private void OnProfileSettingsEventsAppBackgroundImageChanged(object sender, EventArgs e) => AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;
    }
}
