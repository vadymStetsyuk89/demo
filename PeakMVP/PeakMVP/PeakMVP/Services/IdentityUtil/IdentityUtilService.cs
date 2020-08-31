using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Dialog;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.RequestProvider;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Authorization;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.Services.IdentityUtil {
    public class IdentityUtilService : IIdentityUtilService {

        private readonly INavigationService _navigationService;
        private readonly IRequestProvider _requestProvider;
        private readonly IStateService _stateService;
        private readonly IMessagesService _messagesService;
        private readonly IGroupMessagingService _groupMessagingService;
        protected readonly IDialogService _dialogService;

        public IdentityUtilService(
            INavigationService navigationService,
            IRequestProvider requestProvider,
            IStateService stateService,
            IMessagesService messagesService,
            IGroupMessagingService groupMessagingService,
            IDialogService dialogService) {

            _navigationService = navigationService;
            _requestProvider = requestProvider;
            _stateService = stateService;
            _messagesService = messagesService;
            _groupMessagingService = groupMessagingService;
            _dialogService = dialogService;

            ///
            /// TODO: make services dispodable. In that case `unsubscribe` is not important because current service is registered as singleton.
            /// 
            _stateService.ChangedProfile += OnStateServiceChangedProfile;
        }

        public Task<bool> ChargeUserProfileAsync(UserProfile userProfile, bool restartSignalHubs) =>
            Task<bool>.Run(async () => {

                if (userProfile != null) {
                    GlobalSettings.Instance.UserProfile.About = userProfile.About;
                    GlobalSettings.Instance.UserProfile.MySports = userProfile.MySports;
                    GlobalSettings.Instance.UserProfile.Children = userProfile.Children;
                    GlobalSettings.Instance.UserProfile.Id = userProfile.Id;
                    GlobalSettings.Instance.UserProfile.Type = userProfile.Type;
                    GlobalSettings.Instance.UserProfile.FirstName = userProfile.FirstName;
                    GlobalSettings.Instance.UserProfile.LastName = userProfile.LastName;
                    GlobalSettings.Instance.UserProfile.DisplayName = userProfile.DisplayName;
                    GlobalSettings.Instance.UserProfile.ShortId = userProfile.ShortId;
                    GlobalSettings.Instance.UserProfile.IsEmailConfirmed = userProfile.IsEmailConfirmed;
                    GlobalSettings.Instance.UserProfile.DateOfBirth = userProfile.DateOfBirth;
                    GlobalSettings.Instance.UserProfile.Avatar = userProfile.Avatar;
                    GlobalSettings.Instance.UserProfile.Availability = userProfile.Availability;
                    GlobalSettings.Instance.UserProfile.LastSeen = userProfile.LastSeen;
                    GlobalSettings.Instance.UserProfile.ParentId = userProfile.ParentId;
                    GlobalSettings.Instance.UserProfile.ChildUserName = userProfile.ChildUserName;
                    GlobalSettings.Instance.UserProfile.ChildPassword = userProfile.ChildPassword;
                    GlobalSettings.Instance.UserProfile.Contact = userProfile.Contact;
                    GlobalSettings.Instance.UserProfile.Address = userProfile.Address;
                    GlobalSettings.Instance.UserProfile.BackgroundImage = userProfile.BackgroundImage;
                    GlobalSettings.Instance.UserProfile.BrandImage = userProfile.BrandImage;
                    GlobalSettings.Instance.UserProfile.AccesToken = userProfile.AccesToken;
                    GlobalSettings.Instance.UserProfile.ProfileType = userProfile.ProfileType;
                    GlobalSettings.Instance.UserProfile.AppBackgroundImage = userProfile.AppBackgroundImage;
                    GlobalSettings.Instance.UserProfile.ImpersonateProfile = userProfile.ImpersonateProfile;

                    Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                    if (restartSignalHubs) {
                        await RestartSocketServicesAsync();
                    }

                    return true;
                }

                return false;
            });

        public Task<bool> ChargeUserProfileAsync(GetProfileResponse getProfileResponse, bool restartSignalHubs) =>
            Task<bool>.Run(async () => {
                if (getProfileResponse != null) {
                    GlobalSettings.Instance.UserProfile.About = getProfileResponse.About;
                    GlobalSettings.Instance.UserProfile.MySports = getProfileResponse.MySports;
                    GlobalSettings.Instance.UserProfile.Children = getProfileResponse.Children;
                    GlobalSettings.Instance.UserProfile.Id = getProfileResponse.Id;
                    GlobalSettings.Instance.UserProfile.Type = getProfileResponse.Type;
                    GlobalSettings.Instance.UserProfile.FirstName = getProfileResponse.FirstName;
                    GlobalSettings.Instance.UserProfile.LastName = getProfileResponse.LastName;
                    GlobalSettings.Instance.UserProfile.DisplayName = getProfileResponse.DisplayName;
                    GlobalSettings.Instance.UserProfile.ShortId = getProfileResponse.ShortId;
                    GlobalSettings.Instance.UserProfile.IsEmailConfirmed = getProfileResponse.IsEmailConfirmed;
                    GlobalSettings.Instance.UserProfile.DateOfBirth = getProfileResponse.DateOfBirth;
                    GlobalSettings.Instance.UserProfile.Avatar = getProfileResponse.Avatar;
                    GlobalSettings.Instance.UserProfile.Availability = getProfileResponse.Availability;
                    GlobalSettings.Instance.UserProfile.LastSeen = getProfileResponse.LastSeen;
                    GlobalSettings.Instance.UserProfile.ParentId = getProfileResponse.ParentId;
                    GlobalSettings.Instance.UserProfile.ChildUserName = getProfileResponse.ChildUserName;
                    GlobalSettings.Instance.UserProfile.ChildPassword = getProfileResponse.ChildPassword;
                    GlobalSettings.Instance.UserProfile.Contact = getProfileResponse.Contact;
                    GlobalSettings.Instance.UserProfile.Address = getProfileResponse.Address;
                    GlobalSettings.Instance.UserProfile.BackgroundImage = getProfileResponse.BackgroundImage;
                    GlobalSettings.Instance.UserProfile.BrandImage = getProfileResponse.BrandImage;

                    if (getProfileResponse.BackgroundImage != null) {
                        getProfileResponse.BackgroundImage.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, getProfileResponse.BackgroundImage.Url);
                        getProfileResponse.BackgroundImage.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, getProfileResponse.BackgroundImage.ThumbnailUrl);
                    }

                    GlobalSettings.Instance.UserProfile.AppBackgroundImage = getProfileResponse.BackgroundImage;

                    GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), getProfileResponse.Type);

                    Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                    if (restartSignalHubs) {
                        await RestartSocketServicesAsync();
                    }

                    return true;
                }

                return false;
            });


        public Task<bool> ChargeUserProfileAsync(SetProfileSettingsResponse setProfileSettingsResponse, bool restartSignalHubs) =>
            Task<bool>.Run(async () => {
                if (setProfileSettingsResponse != null) {
                    GlobalSettings.Instance.UserProfile.About = setProfileSettingsResponse.About;
                    GlobalSettings.Instance.UserProfile.MySports = setProfileSettingsResponse.MySports;
                    GlobalSettings.Instance.UserProfile.Children = setProfileSettingsResponse.Children;
                    GlobalSettings.Instance.UserProfile.Id = setProfileSettingsResponse.Id;
                    GlobalSettings.Instance.UserProfile.Type = setProfileSettingsResponse.Type;
                    GlobalSettings.Instance.UserProfile.FirstName = setProfileSettingsResponse.FirstName;
                    GlobalSettings.Instance.UserProfile.LastName = setProfileSettingsResponse.LastName;
                    GlobalSettings.Instance.UserProfile.DisplayName = setProfileSettingsResponse.DisplayName;
                    GlobalSettings.Instance.UserProfile.ShortId = setProfileSettingsResponse.ShortId;
                    GlobalSettings.Instance.UserProfile.IsEmailConfirmed = setProfileSettingsResponse.IsEmailConfirmed;
                    GlobalSettings.Instance.UserProfile.DateOfBirth = setProfileSettingsResponse.DateOfBirth;
                    ///
                    /// Returned avatars list is empty
                    ///
                    //GlobalSettings.Instance.UserProfile.Avatar = setProfileSettingsResponse.Avatar;
                    //GlobalSettings.Instance.UserProfile.BackgroundImage = setProfileSettingsResponse.BackgroundImage;
                    GlobalSettings.Instance.UserProfile.Availability = setProfileSettingsResponse.Availability;
                    GlobalSettings.Instance.UserProfile.LastSeen = setProfileSettingsResponse.LastSeen;
                    GlobalSettings.Instance.UserProfile.ParentId = setProfileSettingsResponse.ParentId;
                    GlobalSettings.Instance.UserProfile.ChildUserName = setProfileSettingsResponse.ChildUserName;
                    GlobalSettings.Instance.UserProfile.ChildPassword = setProfileSettingsResponse.ChildPassword;
                    GlobalSettings.Instance.UserProfile.Contact = setProfileSettingsResponse.Contact;
                    GlobalSettings.Instance.UserProfile.Address = setProfileSettingsResponse.Address;
                    GlobalSettings.Instance.UserProfile.BrandImage = setProfileSettingsResponse.BrandImage;
                    GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), setProfileSettingsResponse.Type);

                    Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                    if (restartSignalHubs) {
                        await RestartSocketServicesAsync();
                    }

                    return true;
                }

                return false;
            });


        public bool ChargeUserProfileAvatar(ProfileMediaDTO mediaDTO) {
            if (mediaDTO != null) {
                GlobalSettings.Instance.UserProfile.Avatar = new MediaDTO() {
                    Id = mediaDTO.Id,
                    Name = mediaDTO.Name,
                    ThumbnailUrl = mediaDTO.ThumbnailUrl,
                    Url = mediaDTO.Url,
                    Mime = mediaDTO.Mime
                };

                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                return true;
            }

            return false;
        }

        public bool ChargeUserProfileAvatar(MediaDTO mediaDTO) {
            if (mediaDTO != null) {
                GlobalSettings.Instance.UserProfile.Avatar = new MediaDTO() {
                    Id = mediaDTO.Id,
                    Name = mediaDTO.Name,
                    ThumbnailUrl = mediaDTO.ThumbnailUrl,
                    Url = mediaDTO.Url,
                    Mime = mediaDTO.Mime
                };

                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                return true;
            }

            return false;
        }

        public bool ChargeUserProfileAppBackgroundImage(MediaDTO profileMediaDTO) {
            if (profileMediaDTO != null) {
                profileMediaDTO.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, profileMediaDTO.Url);
                profileMediaDTO.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, profileMediaDTO.ThumbnailUrl);

                GlobalSettings.Instance.UserProfile.AppBackgroundImage = new MediaDTO() {
                    Id = profileMediaDTO.Id,
                    Name = profileMediaDTO.Name,
                    ThumbnailUrl = profileMediaDTO.ThumbnailUrl,
                    Url = profileMediaDTO.Url,
                    Mime = profileMediaDTO.Mime
                };

                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                return true;
            }

            return false;
        }

        public Task ChargeImpersonateUserProfileAsync(string accessToken, GetProfileResponse getProfileResponse) =>
            Task.Run(async () => {
                UserProfile impersonateUserProfile = new UserProfile() {
                    About = GlobalSettings.Instance.UserProfile.About,
                    MySports = GlobalSettings.Instance.UserProfile.MySports,
                    Children = GlobalSettings.Instance.UserProfile.Children,
                    Id = GlobalSettings.Instance.UserProfile.Id,
                    Type = GlobalSettings.Instance.UserProfile.Type,
                    FirstName = GlobalSettings.Instance.UserProfile.FirstName,
                    LastName = GlobalSettings.Instance.UserProfile.LastName,
                    DisplayName = GlobalSettings.Instance.UserProfile.DisplayName,
                    ShortId = GlobalSettings.Instance.UserProfile.ShortId,
                    IsEmailConfirmed = GlobalSettings.Instance.UserProfile.IsEmailConfirmed,
                    DateOfBirth = GlobalSettings.Instance.UserProfile.DateOfBirth,
                    Avatar = GlobalSettings.Instance.UserProfile.Avatar,
                    Availability = GlobalSettings.Instance.UserProfile.Availability,
                    LastSeen = GlobalSettings.Instance.UserProfile.LastSeen,
                    ParentId = GlobalSettings.Instance.UserProfile.ParentId,
                    ChildUserName = GlobalSettings.Instance.UserProfile.ChildUserName,
                    ChildPassword = GlobalSettings.Instance.UserProfile.ChildPassword,
                    Contact = GlobalSettings.Instance.UserProfile.Contact,
                    Address = GlobalSettings.Instance.UserProfile.Address,
                    BackgroundImage = GlobalSettings.Instance.UserProfile.BackgroundImage,
                    BrandImage = GlobalSettings.Instance.UserProfile.BrandImage,
                    AccesToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    ProfileType = GlobalSettings.Instance.UserProfile.ProfileType,
                    AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage,
                    ImpersonateProfile = GlobalSettings.Instance.UserProfile.ImpersonateProfile
                };

                await ChargeUserProfileAsync(getProfileResponse, false);

                GlobalSettings.Instance.UserProfile.AccesToken = accessToken;
                GlobalSettings.Instance.UserProfile.ImpersonateProfile = impersonateUserProfile;

                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                await RestartSocketServicesAsync();
            });

        public void RefreshToken() {
            Device.BeginInvokeOnMainThread(async () => {
                GlobalSettings.Instance.UserProfile.ClearProfile();
                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);
                _requestProvider.ClientTokenReset();

                await _navigationService.NavigateToAsync<LoginViewModel>();
                await _navigationService.RemoveBackStackAsync();

                await StopSocketServicesAsync();
            });
        }

        public async void LogOut() {
            GlobalSettings.Instance.UserProfile.ClearProfile();
            Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);
            _requestProvider.ClientTokenReset();

            await _navigationService.NavigateToAsync<LoginViewModel>();
            await _navigationService.RemoveBackStackAsync();
            await StopSocketServicesAsync();
        }

        public async void ImpersonateLogOut() {
            _navigationService.DisposeStack();

            if (await ChargeUserProfileAsync(GlobalSettings.Instance.UserProfile.ImpersonateProfile, true)) {
                Device.BeginInvokeOnMainThread(() => {
                    _navigationService.Initialize(true);
                });
            }
        }

        private async Task RestartSocketServicesAsync() {
            await StopSocketServicesAsync();

            await _stateService.StartAsync(GlobalSettings.Instance.UserProfile.AccesToken);
            await _messagesService.StartAsync(GlobalSettings.Instance.UserProfile.AccesToken);
            await _groupMessagingService.StartAsync(GlobalSettings.Instance.UserProfile.AccesToken);
        }

        private async Task StopSocketServicesAsync() {
            await _stateService.StopAsync();
            await _messagesService.StopAsync();
            await _groupMessagingService.StopAsync();
        }

        private async void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) {
            try {
                if (e.Profile.BackgroundImage != null) {
                    e.Profile.BackgroundImage.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, e.Profile.BackgroundImage.Url);
                    e.Profile.BackgroundImage.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, e.Profile.BackgroundImage.ThumbnailUrl);
                }

                bool fieldsDifference = GlobalSettings.Instance.UserProfile.About == e.Profile.About &&
                    GlobalSettings.Instance.UserProfile.MySports == e.Profile.MySports &&
                    GlobalSettings.Instance.UserProfile.Type == e.Profile.Type &&
                    GlobalSettings.Instance.UserProfile.FirstName == e.Profile.FirstName &&
                    GlobalSettings.Instance.UserProfile.LastName == e.Profile.LastName &&
                    GlobalSettings.Instance.UserProfile.DisplayName == e.Profile.DisplayName &&
                    GlobalSettings.Instance.UserProfile.ShortId == e.Profile.ShortId &&
                    GlobalSettings.Instance.UserProfile.IsEmailConfirmed == e.Profile.IsEmailConfirmed &&
                    GlobalSettings.Instance.UserProfile.DateOfBirth == e.Profile.DateOfBirth &&
                    GlobalSettings.Instance.UserProfile.Availability == e.Profile.Availability &&
                    GlobalSettings.Instance.UserProfile.LastSeen == e.Profile.LastSeen &&
                    GlobalSettings.Instance.UserProfile.ParentId == e.Profile.ParentId &&
                    GlobalSettings.Instance.UserProfile.ChildUserName == e.Profile.ChildUserName &&
                    GlobalSettings.Instance.UserProfile.ChildPassword == e.Profile.ChildPassword &&
                    GlobalSettings.Instance.UserProfile.Contact == e.Profile.Contact &&
                    GlobalSettings.Instance.UserProfile.Address.City == e.Profile.Address.City &&
                    GlobalSettings.Instance.UserProfile.Address.State == e.Profile.Address.State &&
                    GlobalSettings.Instance.UserProfile.Address.Street == e.Profile.Address.Street &&
                    GlobalSettings.Instance.UserProfile.Address.Zip == e.Profile.Address.Zip;

                //bool fieldsDifference = GlobalSettings.Instance.UserProfile.FirstName == e.Profile.FirstName &&
                //GlobalSettings.Instance.UserProfile.LastName == e.Profile.LastName &&
                //GlobalSettings.Instance.UserProfile.Id == e.Profile.Id &&
                //GlobalSettings.Instance.UserProfile.ParentId == e.Profile.ParentId &&
                //GlobalSettings.Instance.UserProfile.Contact.Email == e.Profile.Contact.Email &&
                //GlobalSettings.Instance.UserProfile.Contact.Phone == e.Profile.Contact.Phone &&
                //GlobalSettings.Instance.UserProfile.DateOfBirth == e.Profile.DateOfBirth &&
                //GlobalSettings.Instance.UserProfile.Address.City == e.Profile.Address.City &&
                //GlobalSettings.Instance.UserProfile.Address.State == e.Profile.Address.State &&
                //GlobalSettings.Instance.UserProfile.Address.Street == e.Profile.Address.Street &&
                //GlobalSettings.Instance.UserProfile.Address.Zip == e.Profile.Address.Zip &&
                //GlobalSettings.Instance.UserProfile.DisplayName == e.Profile.DisplayName &&
                //GlobalSettings.Instance.UserProfile.ShortId == e.Profile.ShortId &&
                //GlobalSettings.Instance.UserProfile.About == e.Profile.About &&
                //GlobalSettings.Instance.UserProfile.MySports == e.Profile.MySports;


                string currentAvatar = GlobalSettings.Instance.UserProfile.Avatar?.Url;
                string newAvatar = e.Profile.Avatar?.Url;
                bool avatarsDifference = currentAvatar == newAvatar;

                //if (GlobalSettings.Instance.UserProfile.Avatar?.Url != null && e.Profile.Avatar != null) {
                //    avatarsDifference = GlobalSettings.Instance.UserProfile.Avatars.Last().Url == e.Profile.Avatars.Last().Url;
                //}

                string currentBackground = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;
                string newBackground = e.Profile.BackgroundImage?.Url;
                bool backgroundDifference = currentBackground == newBackground;

                //if (GlobalSettings.Instance.UserProfile.AppBackgroundImage != null && e.Profile.BackgroundImage != null) {
                //    backgroundDifference = GlobalSettings.Instance.UserProfile.AppBackgroundImage.Url == e.Profile.BackgroundImage.Url;
                //}

                bool isNecessaryToUpdate = !fieldsDifference || !avatarsDifference || !backgroundDifference;

                if (isNecessaryToUpdate) {
                    //GlobalSettings.Instance.UserProfile.FirstName = e.Profile.FirstName;
                    //GlobalSettings.Instance.UserProfile.LastName = e.Profile.LastName;
                    //GlobalSettings.Instance.UserProfile.Id = e.Profile.Id;
                    //GlobalSettings.Instance.UserProfile.ParentId = e.Profile.ParentId;
                    //GlobalSettings.Instance.UserProfile.Contact = e.Profile.Contact;
                    //GlobalSettings.Instance.UserProfile.DateOfBirth = e.Profile.DateOfBirth;
                    //GlobalSettings.Instance.UserProfile.Address = e.Profile.Address;
                    //GlobalSettings.Instance.UserProfile.DisplayName = e.Profile.DisplayName;
                    //GlobalSettings.Instance.UserProfile.ShortId = e.Profile.ShortId;
                    //GlobalSettings.Instance.UserProfile.Avatars = e.Profile.Avatars;
                    //GlobalSettings.Instance.UserProfile.About = e.Profile.About;
                    //GlobalSettings.Instance.UserProfile.Children = (e.Profile.Children != null && e.Profile.Children.Any()) ? e.Profile.Children : null;
                    //GlobalSettings.Instance.UserProfile.MySports = e.Profile.MySports;
                    //GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), e.Profile.Type);

                    GlobalSettings.Instance.UserProfile.About = e.Profile.About;
                    GlobalSettings.Instance.UserProfile.MySports = e.Profile.MySports;
                    GlobalSettings.Instance.UserProfile.Children = e.Profile.Children;
                    GlobalSettings.Instance.UserProfile.Id = e.Profile.Id;
                    GlobalSettings.Instance.UserProfile.Type = e.Profile.Type;
                    GlobalSettings.Instance.UserProfile.FirstName = e.Profile.FirstName;
                    GlobalSettings.Instance.UserProfile.LastName = e.Profile.LastName;
                    GlobalSettings.Instance.UserProfile.DisplayName = e.Profile.DisplayName;
                    GlobalSettings.Instance.UserProfile.ShortId = e.Profile.ShortId;
                    GlobalSettings.Instance.UserProfile.IsEmailConfirmed = e.Profile.IsEmailConfirmed;
                    GlobalSettings.Instance.UserProfile.DateOfBirth = e.Profile.DateOfBirth;
                    GlobalSettings.Instance.UserProfile.Avatar = e.Profile.Avatar;
                    GlobalSettings.Instance.UserProfile.Availability = e.Profile.Availability;
                    GlobalSettings.Instance.UserProfile.LastSeen = e.Profile.LastSeen;
                    GlobalSettings.Instance.UserProfile.ParentId = e.Profile.ParentId;
                    GlobalSettings.Instance.UserProfile.ChildUserName = e.Profile.ChildUserName;
                    GlobalSettings.Instance.UserProfile.ChildPassword = e.Profile.ChildPassword;
                    GlobalSettings.Instance.UserProfile.Contact = e.Profile.Contact;
                    GlobalSettings.Instance.UserProfile.Address = e.Profile.Address;
                    GlobalSettings.Instance.UserProfile.BackgroundImage = e.Profile.BackgroundImage;
                    GlobalSettings.Instance.UserProfile.AppBackgroundImage = e.Profile.BackgroundImage;
                    GlobalSettings.Instance.UserProfile.BrandImage = e.Profile.BrandImage;
                    GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), e.Profile.Type);

                    if (backgroundDifference) {
                        GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChangedInvoke(this, new EventArgs());
                    }

                    Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

                    GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdatedInvoke(this, new ProfileUpdatedArgs() { });
                }
            }
            catch (Exception exc) {
                Debugger.Break();

                await _dialogService.ToastAsync(StateService.CHANGED_PROFILE_HANDLING_ERROR);
            }
        }
    }
}




//using Microsoft.AppCenter.Crashes;
//using Newtonsoft.Json;
//using PeakMVP.Helpers;
//using PeakMVP.Models.Arguments.AppEventsArguments.UserProfile;
//using PeakMVP.Models.DataItems.Autorization;
//using PeakMVP.Models.Identities;
//using PeakMVP.Models.Rests.DTOs;
//using PeakMVP.Models.Rests.Responses.ProfileResponses;
//using PeakMVP.Models.Sockets.StateArgs;
//using PeakMVP.Services.Dialog;
//using PeakMVP.Services.Navigation;
//using PeakMVP.Services.RequestProvider;
//using PeakMVP.Services.SignalR.GroupMessaging;
//using PeakMVP.Services.SignalR.Messages;
//using PeakMVP.Services.SignalR.StateNotify;
//using PeakMVP.ViewModels.Authorization;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using Xamarin.Forms;

//namespace PeakMVP.Services.IdentityUtil {
//    public class IdentityUtilService : IIdentityUtilService {

//        private readonly INavigationService _navigationService;
//        private readonly IRequestProvider _requestProvider;
//        private readonly IStateService _stateService;
//        private readonly IMessagesService _messagesService;
//        private readonly IGroupMessagingService _groupMessagingService;
//        protected readonly IDialogService _dialogService;

//        public IdentityUtilService(
//            INavigationService navigationService,
//            IRequestProvider requestProvider,
//            IStateService stateService,
//            IMessagesService messagesService,
//            IGroupMessagingService groupMessagingService,
//            IDialogService dialogService) {

//            _navigationService = navigationService;
//            _requestProvider = requestProvider;
//            _stateService = stateService;
//            _messagesService = messagesService;
//            _groupMessagingService = groupMessagingService;
//            _dialogService = dialogService;

//            ///
//            /// TODO: make services dispodable. In that case `unsubscribe` is not important because current service is registered as singleton.
//            /// 
//            _stateService.ChangedProfile += OnStateServiceChangedProfile;
//        }

//        public bool ChargeUserProfile(UserProfile userProfile) {
//            if (userProfile != null) {
//                GlobalSettings.Instance.UserProfile.AccesToken = userProfile.AccesToken;
//                GlobalSettings.Instance.UserProfile.FirstName = userProfile.FirstName;
//                GlobalSettings.Instance.UserProfile.LastName = userProfile.LastName;
//                GlobalSettings.Instance.UserProfile.Id = userProfile.Id;
//                GlobalSettings.Instance.UserProfile.ParentId = userProfile.ParentId;
//                GlobalSettings.Instance.UserProfile.Contact = userProfile.Contact;
//                GlobalSettings.Instance.UserProfile.DateOfBirth = userProfile.DateOfBirth;
//                GlobalSettings.Instance.UserProfile.Address = userProfile.Address;
//                GlobalSettings.Instance.UserProfile.DisplayName = userProfile.DisplayName;
//                GlobalSettings.Instance.UserProfile.ShortId = userProfile.ShortId;
//                GlobalSettings.Instance.UserProfile.Avatars = userProfile.Avatars;
//                GlobalSettings.Instance.UserProfile.About = userProfile.About;
//                GlobalSettings.Instance.UserProfile.Children = (userProfile.Children != null && userProfile.Children.Any()) ? userProfile.Children : null;
//                GlobalSettings.Instance.UserProfile.MySports = userProfile.MySports;
//                GlobalSettings.Instance.UserProfile.AppBackgroundImage = userProfile.AppBackgroundImage;
//                GlobalSettings.Instance.UserProfile.ProfileType = userProfile.ProfileType;
//                GlobalSettings.Instance.UserProfile.ImpersonateProfile = userProfile.ImpersonateProfile;

//                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//                RestartSocketServicesAsync();

//                return true;
//            }

//            return false;
//        }

//        public bool ChargeUserProfile(GetProfileResponse getProfileResponse) {
//            if (getProfileResponse != null) {
//                GlobalSettings.Instance.UserProfile.FirstName = getProfileResponse.FirstName;
//                GlobalSettings.Instance.UserProfile.LastName = getProfileResponse.LastName;
//                GlobalSettings.Instance.UserProfile.Id = getProfileResponse.Id;
//                GlobalSettings.Instance.UserProfile.ParentId = getProfileResponse.ParentId;
//                GlobalSettings.Instance.UserProfile.Contact = getProfileResponse.Contact;
//                GlobalSettings.Instance.UserProfile.DateOfBirth = getProfileResponse.DateOfBirth;
//                GlobalSettings.Instance.UserProfile.Address = getProfileResponse.Address;
//                GlobalSettings.Instance.UserProfile.DisplayName = getProfileResponse.DisplayName;
//                GlobalSettings.Instance.UserProfile.ShortId = getProfileResponse.ShortId;
//                GlobalSettings.Instance.UserProfile.Avatars = getProfileResponse.Avatars;
//                GlobalSettings.Instance.UserProfile.About = getProfileResponse.About;
//                GlobalSettings.Instance.UserProfile.Children = getProfileResponse.Children.Any() ? getProfileResponse.Children : null;
//                GlobalSettings.Instance.UserProfile.MySports = getProfileResponse.MySports;

//                if (getProfileResponse.BackgroundImage != null) {
//                    getProfileResponse.BackgroundImage.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, getProfileResponse.BackgroundImage.Url);
//                    getProfileResponse.BackgroundImage.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, getProfileResponse.BackgroundImage.ThumbnailUrl);
//                }
//                GlobalSettings.Instance.UserProfile.AppBackgroundImage = getProfileResponse.BackgroundImage;

//                GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), getProfileResponse.Type);

//                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//                RestartSocketServicesAsync();

//                return true;
//            }

//            return false;
//        }

//        public bool ChargeUserProfile(SetProfileSettingsResponse setProfileSettingsResponse) {
//            if (setProfileSettingsResponse != null) {
//                GlobalSettings.Instance.UserProfile.FirstName = setProfileSettingsResponse.FirstName;
//                GlobalSettings.Instance.UserProfile.LastName = setProfileSettingsResponse.LastName;
//                GlobalSettings.Instance.UserProfile.Id = setProfileSettingsResponse.Id;
//                GlobalSettings.Instance.UserProfile.ParentId = setProfileSettingsResponse.ParentId;
//                GlobalSettings.Instance.UserProfile.Contact = setProfileSettingsResponse.Contact;
//                GlobalSettings.Instance.UserProfile.DateOfBirth = setProfileSettingsResponse.DateOfBirth;
//                GlobalSettings.Instance.UserProfile.Address = setProfileSettingsResponse.Address;
//                GlobalSettings.Instance.UserProfile.DisplayName = setProfileSettingsResponse.DisplayName;
//                GlobalSettings.Instance.UserProfile.ShortId = setProfileSettingsResponse.ShortId;
//                ///
//                /// Returned avatars list is empty
//                ///
//                //GlobalSettings.Instance.UserProfile.Avatars = setProfileSettingsResponse.Avatars;
//                //GlobalSettings.Instance.UserProfile.AppBackgroundImage = getProfileResponse.BackgroundImage;
//                GlobalSettings.Instance.UserProfile.About = setProfileSettingsResponse.About;
//                GlobalSettings.Instance.UserProfile.MySports = setProfileSettingsResponse.MySports;
//                GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), setProfileSettingsResponse.Type);

//                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//                RestartSocketServicesAsync();

//                return true;
//            }

//            return false;
//        }

//        public bool ChargeUserProfileAvatar(MediaDTO mediaDTO) {
//            if (mediaDTO != null) {
//                GlobalSettings.Instance.UserProfile.Avatars = new[] {
//                    new ProfileMediaDTO() {
//                        Id = mediaDTO.Id,
//                        Name = mediaDTO.Name,
//                        ThumbnailUrl = mediaDTO.ThumbnailUrl,
//                        Url = mediaDTO.Url
//                    }
//                };

//                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//                return true;
//            }

//            return false;
//        }

//        public bool ChargeUserProfileAppBackgroundImage(ProfileMediaDTO profileMediaDTO) {
//            if (profileMediaDTO != null) {
//                profileMediaDTO.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, profileMediaDTO.Url);
//                profileMediaDTO.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, profileMediaDTO.ThumbnailUrl);

//                GlobalSettings.Instance.UserProfile.AppBackgroundImage = new ProfileMediaDTO() {
//                    Id = profileMediaDTO.Id,
//                    Name = profileMediaDTO.Name,
//                    ThumbnailUrl = profileMediaDTO.ThumbnailUrl,
//                    Url = profileMediaDTO.Url
//                };

//                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//                return true;
//            }

//            return false;
//        }

//        public async void ChargeImpersonateUserProfile(string accessToken, GetProfileResponse getProfileResponse) {
//            UserProfile impersonateUserProfile = new UserProfile() {
//                About = GlobalSettings.Instance.UserProfile.About,
//                AccesToken = GlobalSettings.Instance.UserProfile.AccesToken,
//                Address = GlobalSettings.Instance.UserProfile.Address,
//                AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage,
//                Avatars = GlobalSettings.Instance.UserProfile.Avatars,
//                Children = GlobalSettings.Instance.UserProfile.Children,
//                Contact = GlobalSettings.Instance.UserProfile.Contact,
//                DateOfBirth = GlobalSettings.Instance.UserProfile.DateOfBirth,
//                DisplayName = GlobalSettings.Instance.UserProfile.DisplayName,
//                FirstName = GlobalSettings.Instance.UserProfile.FirstName,
//                Id = GlobalSettings.Instance.UserProfile.Id,
//                ParentId = GlobalSettings.Instance.UserProfile.ParentId,
//                ImpersonateProfile = GlobalSettings.Instance.UserProfile.ImpersonateProfile,
//                LastName = GlobalSettings.Instance.UserProfile.LastName,
//                MySports = GlobalSettings.Instance.UserProfile.MySports,
//                ProfileType = GlobalSettings.Instance.UserProfile.ProfileType,
//                ShortId = GlobalSettings.Instance.UserProfile.ShortId
//            };

//            ChargeUserProfile(getProfileResponse);
//            GlobalSettings.Instance.UserProfile.AccesToken = accessToken;

//            GlobalSettings.Instance.UserProfile.ImpersonateProfile = impersonateUserProfile;

//            Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//            await RestartSocketServicesAsync();
//        }

//        public void RefreshToken() {
//            Device.BeginInvokeOnMainThread(async () => {
//                GlobalSettings.Instance.UserProfile.ClearProfile();
//                Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);
//                _requestProvider.ClientTokenReset();

//                await _navigationService.NavigateToAsync<LoginViewModel>();
//                await _navigationService.RemoveBackStackAsync();

//                await StopSocketServicesAsync();
//            });
//        }

//        public async void LogOut() {
//            GlobalSettings.Instance.UserProfile.ClearProfile();
//            Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);
//            _requestProvider.ClientTokenReset();

//            await _navigationService.NavigateToAsync<LoginViewModel>();
//            await _navigationService.RemoveBackStackAsync();
//            await StopSocketServicesAsync();
//        }

//        public void ImpersonateLogOut() {
//            if (ChargeUserProfile(GlobalSettings.Instance.UserProfile.ImpersonateProfile)) {
//                Device.BeginInvokeOnMainThread(() => {
//                    _navigationService.Initialize(true);
//                });
//            }
//        }

//        private async Task RestartSocketServicesAsync() {
//            await StopSocketServicesAsync();

//            await _stateService.StartAsync(GlobalSettings.Instance.UserProfile.AccesToken);
//            await _messagesService.StartAsync(GlobalSettings.Instance.UserProfile.AccesToken);
//            await _groupMessagingService.StartAsync(GlobalSettings.Instance.UserProfile.AccesToken);
//        }

//        private async Task StopSocketServicesAsync() {
//            await _stateService.StopAsync();
//            await _messagesService.StopAsync();
//            await _groupMessagingService.StopAsync();
//        }

//        private async void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) {
//            try {
//                if (e.Profile.BackgroundImage != null) {
//                    e.Profile.BackgroundImage.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, e.Profile.BackgroundImage.Url);
//                    e.Profile.BackgroundImage.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, e.Profile.BackgroundImage.ThumbnailUrl);
//                }

//                if (e.Profile.Avatars != null && e.Profile.Avatars.Any()) {
//                    List<ProfileMediaDTO> avatars = e.Profile.Avatars.ToList();
//                    ProfileMediaDTO lastValidAvatar = avatars.First();
//                    avatars.Remove(lastValidAvatar);
//                    avatars.Add(lastValidAvatar);

//                    e.Profile.Avatars = avatars.ToArray();
//                }

//                bool fieldsDifference = GlobalSettings.Instance.UserProfile.FirstName == e.Profile.FirstName &&
//                GlobalSettings.Instance.UserProfile.LastName == e.Profile.LastName &&
//                GlobalSettings.Instance.UserProfile.Id == e.Profile.Id &&
//                GlobalSettings.Instance.UserProfile.ParentId == e.Profile.ParentId &&
//                GlobalSettings.Instance.UserProfile.Contact.Email == e.Profile.Contact.Email &&
//                GlobalSettings.Instance.UserProfile.Contact.Phone == e.Profile.Contact.Phone &&
//                GlobalSettings.Instance.UserProfile.DateOfBirth == e.Profile.DateOfBirth &&
//                GlobalSettings.Instance.UserProfile.Address.City == e.Profile.Address.City &&
//                GlobalSettings.Instance.UserProfile.Address.State == e.Profile.Address.State &&
//                GlobalSettings.Instance.UserProfile.Address.Street == e.Profile.Address.Street &&
//                GlobalSettings.Instance.UserProfile.Address.Zip == e.Profile.Address.Zip &&
//                GlobalSettings.Instance.UserProfile.DisplayName == e.Profile.DisplayName &&
//                GlobalSettings.Instance.UserProfile.ShortId == e.Profile.ShortId &&
//                GlobalSettings.Instance.UserProfile.About == e.Profile.About &&
//                GlobalSettings.Instance.UserProfile.MySports == e.Profile.MySports;

//                bool avatarsDifference = true;

//                if (GlobalSettings.Instance.UserProfile.Avatars != null && e.Profile.Avatars != null && GlobalSettings.Instance.UserProfile.Avatars.Any() && e.Profile.Avatars.Any()) {
//                    avatarsDifference = GlobalSettings.Instance.UserProfile.Avatars.Last().Url == e.Profile.Avatars.Last().Url;
//                }

//                bool backgroundDifference = true;

//                if (GlobalSettings.Instance.UserProfile.AppBackgroundImage != null && e.Profile.BackgroundImage != null) {
//                    backgroundDifference = GlobalSettings.Instance.UserProfile.AppBackgroundImage.Url == e.Profile.BackgroundImage.Url;
//                }

//                bool isNecessaryToUpdate = !fieldsDifference || !avatarsDifference || !backgroundDifference;

//                if (isNecessaryToUpdate) {
//                    GlobalSettings.Instance.UserProfile.FirstName = e.Profile.FirstName;
//                    GlobalSettings.Instance.UserProfile.LastName = e.Profile.LastName;
//                    GlobalSettings.Instance.UserProfile.Id = e.Profile.Id;
//                    GlobalSettings.Instance.UserProfile.ParentId = e.Profile.ParentId;
//                    GlobalSettings.Instance.UserProfile.Contact = e.Profile.Contact;
//                    GlobalSettings.Instance.UserProfile.DateOfBirth = e.Profile.DateOfBirth;
//                    GlobalSettings.Instance.UserProfile.Address = e.Profile.Address;
//                    GlobalSettings.Instance.UserProfile.DisplayName = e.Profile.DisplayName;
//                    GlobalSettings.Instance.UserProfile.ShortId = e.Profile.ShortId;
//                    GlobalSettings.Instance.UserProfile.Avatars = e.Profile.Avatars;
//                    GlobalSettings.Instance.UserProfile.About = e.Profile.About;
//                    GlobalSettings.Instance.UserProfile.Children = (e.Profile.Children != null && e.Profile.Children.Any()) ? e.Profile.Children : null;
//                    GlobalSettings.Instance.UserProfile.MySports = e.Profile.MySports;
//                    GlobalSettings.Instance.UserProfile.ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), e.Profile.Type);

//                    if (GlobalSettings.Instance.UserProfile.AppBackgroundImage.Url != e.Profile.BackgroundImage.Url) {
//                        GlobalSettings.Instance.UserProfile.AppBackgroundImage = e.Profile.BackgroundImage;

//                        GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChangedInvoke(this, new EventArgs());
//                    }

//                    Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);

//                    GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.ProfileUpdatedInvoke(this, new ProfileUpdatedArgs() { });
//                }
//            }
//            catch (Exception exc) {
//                Debugger.Break();

//                await _dialogService.ToastAsync(StateService.CHANGED_PROFILE_HANDLING_ERROR);
//            }
//        }
//    }
//}

