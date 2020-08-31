using PeakMVP.Models.AppNavigation;
using PeakMVP.Models.Arguments.AppEventsArguments.TeamEvents;
using PeakMVP.Models.Arguments.InitializeArguments.Post;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top.Popups;
using PeakMVP.ViewModels.MainContent.Character;
using PeakMVP.ViewModels.MainContent.Members;
using PeakMVP.ViewModels.MainContent.Search;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ActionBars.Top {
    public class ModeActionBarViewModel : NestedViewModel, IModeActionBar {

        private CancellationTokenSource _pickMediaCancellationTokenSource = new CancellationTokenSource();

        private readonly INavigationContext _navigationContext;
        private readonly IProfileMediaService _profileMediaService;

        public ModeActionBarViewModel(
            INavigationContext bottomBarDataItems,
            IProfileMediaService profileMediaService) {

            _navigationContext = bottomBarDataItems;
            _profileMediaService = profileMediaService;

            SearchViewModel = ViewModelLocator.Resolve<CommonActionBarSearchViewModel>();
            SearchViewModel.InitializeAsync(this);

            MediaActionPopupViewModel = ViewModelLocator.Resolve<MediaActionPopupViewModel>();
            MediaActionPopupViewModel.InitializeAsync(this);
        }

        public ICommand ToggleModeCommand => new Command(() => SelectedMode = GetNextMode(), () => IsModesAvailable);

        /// <summary>
        /// Old flow. Opens platform dependent file explorer and uploads medias
        /// </summary>
        //public ICommand MediaActionCommand => new Command(async () => {
        //    ResetCancellationTokenSource(ref _pickMediaCancellationTokenSource);
        //    CancellationTokenSource cancellationTokenSource = _pickMediaCancellationTokenSource;

        //    Guid busyKey = Guid.NewGuid();
        //    UpdateBusyVisualState(busyKey, true);

        //    try {
        //        IPickedMediaFromGallery pickedMediaResult = await DependencyService.Get<IPickMediaDependencyService>().PickMediaAsync();

        //        if (pickedMediaResult.Completion) {
        //            if (!(string.IsNullOrEmpty(pickedMediaResult.DataBase64)) && !(string.IsNullOrEmpty(pickedMediaResult.DataThumbnailBase64))) {
        //                MediaDTO media = await AddMediaAsync(pickedMediaResult.DataBase64, pickedMediaResult.DataThumbnailBase64, pickedMediaResult.MimeType == "image" ? ProfileMediaService.IMAGE_MEDIA_TYPE : ProfileMediaService.VIDEO_MEDIA_TYPE, cancellationTokenSource);

        //                if (media != null) {
        //                    GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAddedInvoke(this, media);
        //                }
        //            }
        //        }
        //        else {
        //            if (pickedMediaResult.Exception != null) {
        //                throw new InvalidOperationException(
        //                    string.Format("{0}. {1}",
        //                        ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE,
        //                        pickedMediaResult.ErrorMessage ?? ""), pickedMediaResult.Exception);
        //            }
        //            else if (!(string.IsNullOrEmpty(pickedMediaResult.ErrorMessage))) {
        //                throw new InvalidOperationException(string.Format("{0}. {1}", ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE, pickedMediaResult.ErrorMessage));
        //            }
        //            else {
        //                throw new InvalidOperationException(ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_VIDEO_ERROR_MESSAGE);
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

        public ICommand MediaActionCommand => new Command(() => {
            MediaActionPopupViewModel.ShowPopupCommand.Execute(null);
        });

        /// <summary>
        /// Old flow allow to take photo/video
        /// </summary>
        //public ICommand MediaActionCommand => new Command(async () => {
        //    ResetCancellationTokenSource(ref _pickMediaCancellationTokenSource);
        //    CancellationTokenSource cancellationTokenSource = _pickMediaCancellationTokenSource;

        //    Guid busyKey = Guid.NewGuid();
        //    UpdateBusyVisualState(busyKey, true);

        //    try {
        //        IPickedMediaFromGallery takedMedia = await DependencyService.Get<IPickMediaDependencyService>().TakePhotoOrVideoAsync();

        //        if (takedMedia.Completion) {
        //            if (!(string.IsNullOrEmpty(takedMedia.DataBase64)) && !(string.IsNullOrEmpty(takedMedia.DataThumbnailBase64))) {
        //                MediaDTO media = await AddMediaAsync(takedMedia.DataBase64, takedMedia.DataThumbnailBase64, takedMedia.MimeType, cancellationTokenSource);

        //                if (media != null) {
        //                    await DialogService.ToastAsync((takedMedia.MimeType == ProfileMediaService.IMAGE_MEDIA_TYPE)
        //                        ? ProfileMediaService.ADD_IMAGE_SUCCESSFUL_COMPLETION_MESSAGE
        //                        : (takedMedia.MimeType == ProfileMediaService.VIDEO_MEDIA_TYPE)
        //                            ? ProfileMediaService.ADD_VIDEO_SUCCESSFUL_COMPLETION_MESSAGE
        //                            : ProfileMediaService.ADD_MEDIA_SUCCESSFUL_COMPLETION_MESSAGE);

        //                    GlobalSettings.Instance.AppMessagingEvents.MediaEvents.NewMediaAddedInvoke(this, media);
        //                }
        //            }
        //        }
        //        else {
        //            if (takedMedia.Exception != null) {
        //                throw new InvalidOperationException(
        //                    string.Format("{0}. {1}",
        //                        ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE,
        //                        takedMedia.ErrorMessage ?? ""), takedMedia.Exception);
        //            }
        //            else if (!(string.IsNullOrEmpty(takedMedia.ErrorMessage))) {
        //                throw new InvalidOperationException(string.Format("{0}. {1}", ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE, takedMedia.ErrorMessage));
        //            }
        //            else {
        //                throw new InvalidOperationException(ManageVideosViewModel.COMMON_CAN_NOT_ATTACH_MEDIA_ERROR_MESSAGE);
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

        public ContentPageBaseViewModel RelativeContentPageBaseViewModel { get; private set; }

        public CommonActionBarSearchViewModel SearchViewModel { get; private set; }

        private MediaActionPopupViewModel _mediaActionPopupViewModel;
        public MediaActionPopupViewModel MediaActionPopupViewModel {
            get => _mediaActionPopupViewModel;
            private set {
                _mediaActionPopupViewModel?.Dispose();
                SetProperty<MediaActionPopupViewModel>(ref _mediaActionPopupViewModel, value);
            }
        }

        private IReadOnlyCollection<NavigationModeBase> _modes;
        public IReadOnlyCollection<NavigationModeBase> Modes {
            get => _modes;
            private set => SetProperty<IReadOnlyCollection<NavigationModeBase>>(ref _modes, value);
        }

        private NavigationModeBase _selectedMode;
        public NavigationModeBase SelectedMode {
            get => _selectedMode;
            private set {
                SetProperty<NavigationModeBase>(ref _selectedMode, value);
            }
        }

        private bool _isModeToggleAvailable;
        public bool IsModesAvailable {
            get => _isModeToggleAvailable;
            set {
                SetProperty<bool>(ref _isModeToggleAvailable, value);

                if (value) {
                    Modes?.ForEach(m => m.Dispose());

                    Modes = _navigationContext.BuildModes(GlobalSettings.Instance.UserProfile.ProfileType);
                    SelectedMode = GetNextMode();
                }
                else {
                    Modes?.ForEach(m => m.Dispose());

                    Modes = null;
                    SelectedMode = null;
                }
            }
        }

        bool _hasBackButton;
        public bool HasBackButton {
            get { return _hasBackButton; }
            private set { SetProperty(ref _hasBackButton, value); }
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _pickMediaCancellationTokenSource);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _pickMediaCancellationTokenSource);

            SearchViewModel?.Dispose();
            Modes?.ForEach(m => m.Dispose());
            MediaActionPopupViewModel?.Dispose();
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ContentPageBaseViewModel contentPageBaseViewModel) {
                RelativeContentPageBaseViewModel = contentPageBaseViewModel;

                HasBackButton = NavigationService.CanVisibleBackButton;
            }
            else if (navigationData is AttachExternalMediaToNewPostArgs externalMediaToNewPostArgs) {
                SelectedMode = Modes.First(nMB => nMB is SocialMode);
                SelectedMode.SelectTab(typeof(CharacterViewModel));
            }

            Modes?.ForEach(m => m.BarItems?.ForEach(barItem => barItem.InitializeAsync(navigationData)));

            MediaActionPopupViewModel?.InitializeAsync(navigationData);
            SearchViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.ViewMembersFromTargetTeam += OnTeamEventsViewMembersFromTargetTeam;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.TeamEvents.ViewMembersFromTargetTeam -= OnTeamEventsViewMembersFromTargetTeam;
        }

        private NavigationModeBase GetNextMode() {
            NavigationModeBase targetMode = null;

            if (SelectedMode == null || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Fan) {
                targetMode = Modes.FirstOrDefault(nMB => nMB is SocialMode);
            }
            else {
                targetMode = Modes.ElementAt<NavigationModeBase>((Modes.IndexOf(SelectedMode) + 1) % Modes.Count);
            }

            return targetMode;
        }

        private async void OnTeamEventsViewMembersFromTargetTeam(object sender, ViewMembersFromTargetTeamArgs e) {
            if (IsModesAvailable && SelectedMode.BarModeType == BarMode.Sport) {
                IBottomBarTab membersViewTab = SelectedMode.BarItems.FirstOrDefault<IBottomBarTab>(barTab => barTab is MembersViewModel);

                if (membersViewTab != null) {
                    await membersViewTab.InitializeAsync(e);
                    SelectedMode.SelectedBarItemIndex = SelectedMode.BarItems.IndexOf(membersViewTab);
                }
            }
        }

        private Task<ProfileMediaDTO> AddMediaAsync(string base64Source, string base64SourceThumbnail, string mediaType, CancellationTokenSource cancellationTokenSource) =>
            Task<ProfileMediaDTO>.Run(async () => {
                AddMediaDataModel addProfileMediaRequestDataModel = new AddMediaDataModel() {
                    File = new Models.Rests.DTOs.FileDTO() {
                        Base64 = base64Source,
                        Name = string.Format("{0}.{1}", Guid.NewGuid(), mediaType == ProfileMediaService.IMAGE_MEDIA_TYPE ? ProfileMediaService.PNG_IMAGE_FORMAT : ProfileMediaService.MP4_VIDEO_FORMAT)
                    },
                    Thumbnail = new Models.Rests.DTOs.FileDTO() {
                        Base64 = base64SourceThumbnail
                    },
                    MediaType = mediaType
                };
                Debugger.Break();
                ProfileMediaDTO media = null;
                //ProfileMediaDTO media = await _profileMediaService.AddProfileMedia(addProfileMediaRequestDataModel, cancellationTokenSource);

                return media;
            }, cancellationTokenSource.Token);
    }
}
