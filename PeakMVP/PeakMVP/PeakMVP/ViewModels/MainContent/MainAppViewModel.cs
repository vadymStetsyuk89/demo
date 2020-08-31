using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.AppNavigation;
using PeakMVP.Models.Arguments.InitializeArguments.Post;
using PeakMVP.Models.Arguments.InitializeArguments.Search;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent {
    public class MainAppViewModel : LoggedContentPageViewModel {

        private static readonly string _ATTACH_EXTERNAL_MEDIA_TO_NEW_POST_WARNING = "Can't handle just picked media";

        public MainAppViewModel() {

            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = true;
            ((ModeActionBarViewModel)ActionBarViewModel).InitializeAsync(this);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ViewSelfInfoArgs) {
                try {
                    ModeActionBarViewModel modeActionBar = (ModeActionBarViewModel)ActionBarViewModel;

                    if (modeActionBar.SelectedMode is SportMode) {
                        modeActionBar.ToggleModeCommand.Execute(null);
                    }

                    modeActionBar.SelectedMode.SelectedBarItemIndex = 1;
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw exc;
                }
            }

            return base.InitializeAsync(navigationData);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged += OnProfileSettingsEventsAppBackgroundImageChanged;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.AttachMediaTotheNewPostOffer += OnPostEventsAttachMediaTotheNewPostOffer;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged -= OnProfileSettingsEventsAppBackgroundImageChanged;
            GlobalSettings.Instance.AppMessagingEvents.PostEvents.AttachMediaTotheNewPostOffer -= OnPostEventsAttachMediaTotheNewPostOffer;
        }

        private void OnProfileSettingsEventsAppBackgroundImageChanged(object sender, EventArgs e) => AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

        private async void OnPostEventsAttachMediaTotheNewPostOffer(object sender, AttachExternalMediaToNewPostArgs e) {
            try {
                await ActionBarViewModel.InitializeAsync(e);
                await NavigationService.NavigateToAsync<MainAppViewModel>();
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(_ATTACH_EXTERNAL_MEDIA_TO_NEW_POST_WARNING);
            }
        }
    }
}
