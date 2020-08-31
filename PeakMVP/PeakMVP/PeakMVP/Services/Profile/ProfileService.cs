using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.IdentityRequests;
using PeakMVP.Models.Rests.Requests.ProfileRequests;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Identity;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Models.Rests.Responses.ProfileResponses.ErrorResponses;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.Services.Profile {
    public class ProfileService : IProfileService {

        public static readonly string CANT_RESOLVE_PROFILE_COMMON_ERROR_MESSAGE = "Can't resolve profile info";
        public static readonly string CANT_UPDATE_OUTER_PROFILE_COMMON_ERROR_MESSAGE = "Cant update outer profile info now";
        public static readonly string SET_PROFILE_SETTINGS_COMMON_ERROR_MESSAGE = "Can't set profile settings";
        private static readonly string _SET_AVATAR_COMMON_ERROR_MESSAGE = "Can't set avatar";
        private static readonly string _SET_APP_BACKGROUND_IMAGE_COMMON_ERROR_MESSAGE = "Can't set app background image";
        private static readonly string _IMPERSONATE_LOGIN_COMMON_ERROR_MESSAGE = "Impersonate login error";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly IMediaFactory _mediaFactory;
        private readonly INavigationService _navigationService;
        private readonly IProfileFactory _profileFactory;

        /// <summary>
        ///     ctor().
        /// </summary>
        public ProfileService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService,
            IProfileMediaService profileMediaService,
            IMediaFactory mediaFactory,
            IProfileFactory profileFactory,
            INavigationService navigationService) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
            _mediaFactory = mediaFactory;
            _navigationService = navigationService;
            _profileFactory = profileFactory;
        }

        public Task<bool> UpdateOuterProfileInfoAsync(OuterProfileEditDataModel profileEdit, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                UpdateOuterProfileRequest updateOuterProfileRequest = new UpdateOuterProfileRequest {
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.OuterProfileEditEndPoint,
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = profileEdit
                };

                UpdateOuterProfileResponse updateOuterProfileResponse = null;
                bool completion = false;

                try {
                    updateOuterProfileResponse =
                        await _requestProvider.PostAsync<UpdateOuterProfileRequest, UpdateOuterProfileResponse>(updateOuterProfileRequest);

                    completion = true;
                }
                catch (ServiceAuthenticationException exc) {
                    completion = false;
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    completion = false;
                    Crashes.TrackError(exc);

                    throw exc;
                }

                return completion;
            });

        public async Task<GetProfileResponse> GetProfileAsync() =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GetProfileRequest getProfileRequest = new GetProfileRequest {
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.GetProfileEndPoints,
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                GetProfileResponse getProfileResponse = null;

                try {
                    getProfileResponse =
                        await _requestProvider.GetAsync<GetProfileRequest, GetProfileResponse>(getProfileRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    Debugger.Break();
                    throw new Exception(ex.Message);
                }

                return getProfileResponse;
            });

        public async Task<GetProfileResponse> GetProfileAsync(string accessToken, CancellationTokenSource cancellationTokenSource) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GetProfileRequest getProfileRequest = new GetProfileRequest {
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.GetProfileEndPoints,
                    AccessToken = accessToken
                };

                GetProfileResponse getProfileResponse = null;

                try {
                    getProfileResponse =
                        await _requestProvider.GetAsync<GetProfileRequest, GetProfileResponse>(getProfileRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    Debugger.Break();
                    throw new Exception(ex.Message);
                }

                return getProfileResponse;
            }, cancellationTokenSource.Token);

        public async Task<GetProfileResponse> GetProfileByShortIdAsync(string shortId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GetProfileRequest getProfileRequest = new GetProfileRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ProfileEndpoints.GetProfileByShortIdEndPoints, shortId),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                GetProfileResponse getProfileResponse = null;

                try {
                    getProfileResponse =
                        await _requestProvider.GetAsync<GetProfileRequest, GetProfileResponse>(getProfileRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    Debugger.Break();
                    throw new Exception(ex.Message);
                }

                return getProfileResponse;
            }, cancellationToken);

        public async Task<ProfileDTO> GetProfileByIdAsync(long id, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ProfileDTO profile = null;

                GetProfileRequest getProfileRequest = new GetProfileRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ProfileEndpoints.GetProfileByIdEndPoints, id),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                GetProfileResponse getProfileResponse = null;

                try {
                    getProfileResponse =
                        await _requestProvider.GetAsync<GetProfileRequest, GetProfileResponse>(getProfileRequest);

                    if (getProfileResponse != null) {
                        profile = _profileFactory.BuildProfileDTO(getProfileResponse);
                    }
                    else {
                        throw new InvalidOperationException(CANT_RESOLVE_PROFILE_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    Debugger.Break();
                    throw new Exception(ex.Message);
                }

                return profile;
            }, cancellationToken);

        public Task<bool> SetAvatarAsync(long mediaId, CancellationTokenSource cancellationTokenSource, long profileId = default(long)) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }
                bool completion = false;

                SetProfileAvatarRequest setProfileAvatarRequest = new SetProfileAvatarRequest() {
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.SetProfileAvatarEndPoint,
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = new SetProfileAvatarDataModel() {
                        MediaId = mediaId,
                        ProfileId = profileId != default(long) ? profileId : default(long)
                    }
                };

                try {
                    SetProfileAvatarResponse setProfileAvatarResponse = await _requestProvider.PostAsync<SetProfileAvatarRequest, SetProfileAvatarResponse>(setProfileAvatarRequest);

                    if (setProfileAvatarResponse != null) {
                        completion = true;
                    }
                    else {
                        completion = false;
                        throw new InvalidOperationException(ProfileService._SET_AVATAR_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    completion = false;
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    completion = false;
                    Crashes.TrackError(exc);

                    throw exc;
                }

                return completion;
            }, cancellationTokenSource.Token);

        //public Task<ProfileMediaDTO> SetAvatarAsync(string base64PickedAvatar, CancellationTokenSource cancellationTokenSource) =>
        //    Task<ProfileMediaDTO>.Run(async () => {
        //        if (!CrossConnectivity.Current.IsConnected) {
        //            throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
        //        }

        //        ProfileMediaDTO addedAvatar = null;

        //        SetProfileAvatarRequest setProfileAvatarRequest = new SetProfileAvatarRequest() {
        //            Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.SetProfileAvatarEndPoint,
        //            AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
        //            Data = new SetProfileAvatarDataModel() {
        //                ImageDataBase64 = base64PickedAvatar,
        //                ImageFileName = string.Format("{0}.{1}", Guid.NewGuid(), ProfileMediaService.PNG_IMAGE_FORMAT)
        //            }
        //        };

        //        try {
        //            SetProfileAvatarResponse setProfileAvatarResponse = await _requestProvider.PostAsync<SetProfileAvatarRequest, SetProfileAvatarResponse>(setProfileAvatarRequest);

        //            if (setProfileAvatarResponse != null) {
        //                addedAvatar = new ProfileMediaDTO() {
        //                    Id = setProfileAvatarResponse.Id,
        //                    Name = setProfileAvatarResponse.Name,
        //                    ThumbnailUrl = setProfileAvatarResponse.ThumbnailUrl,
        //                    Url = setProfileAvatarResponse.Url
        //                };
        //            }
        //            else {
        //                throw new InvalidOperationException(ProfileService._SET_AVATAR_COMMON_ERROR_MESSAGE);
        //            }
        //        }
        //        catch (ServiceAuthenticationException exc) {
        //            _identityUtilService.RefreshToken();

        //            throw exc;
        //        }
        //        catch (Exception exc) {
        //            Crashes.TrackError(exc);

        //            throw exc;
        //        }

        //        return addedAvatar;
        //    }, cancellationTokenSource.Token);

        public Task<SetProfileSettingsResponse> SetProfileAsync(SetProfileDataModel setProfileDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task<SetProfileSettingsResponse>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                SetProfileSettingsResponse setProfileSettingsResponse = null;

                SetProfileSettingsRequest setProfileSettingsRequest = new SetProfileSettingsRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = setProfileDataModel,
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.SetProfileSettingsEndPoint
                };

                try {
                    setProfileSettingsResponse = await _requestProvider.PostAsync<SetProfileSettingsRequest, SetProfileSettingsResponse>(setProfileSettingsRequest);

                    if (setProfileSettingsResponse == null)
                        throw new InvalidOperationException(SET_PROFILE_SETTINGS_COMMON_ERROR_MESSAGE);

                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    SetProfileSettingsErrorResponse crutch = JsonConvert.DeserializeObject<SetProfileSettingsErrorResponse>(exc.Message);

                    throw new InvalidOperationException(string.Format("{0}{1}{2}{3}{4}{5}",
                        crutch.Empty?.FirstOrDefault(),
                        crutch.ProvidedPassword?.FirstOrDefault(),
                        crutch.CurrentPassword?.FirstOrDefault(),
                        crutch.Email?.FirstOrDefault(),
                        crutch.FirstName?.FirstOrDefault(),
                        crutch.LastName?.FirstOrDefault()));
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw exc;
                }

                return setProfileSettingsResponse;
            }, cancellationTokenSource.Token);

        //public Task<MediaDTO> SetAppBackgroundImage(BackgroundImageDataModel backgroundImageDataModel, CancellationTokenSource cancellationTokenSource) =>
        //    Task<MediaDTO>.Run(async () => {
        //        if (!CrossConnectivity.Current.IsConnected) {
        //            throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
        //        }

        //        SetAppBackgroundImageResponse setAppBackgroundImageResponse = null;
        //        MediaDTO profileMediaDTO = null;

        //        SetAppBackgroundImageRequest setAppBackgroundImageRequest = new SetAppBackgroundImageRequest() {
        //            AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
        //            Data = new SetAppBackgroundImageDataModel() {
        //                BackgroundImage = backgroundImageDataModel
        //            },
        //            Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.SetAppBackgroundImageEndPoint
        //        };

        //        try {
        //            setAppBackgroundImageResponse = await _requestProvider.PostAsync<SetAppBackgroundImageRequest, SetAppBackgroundImageResponse>(setAppBackgroundImageRequest);

        //            if (setAppBackgroundImageResponse != null) {
        //                profileMediaDTO = _mediaFactory.BuildMediaDTO(setAppBackgroundImageResponse);
        //            }
        //            else {
        //                throw new InvalidOperationException(ProfileService._SET_APP_BACKGROUND_IMAGE_COMMON_ERROR_MESSAGE);
        //            }
        //        }
        //        catch (ServiceAuthenticationException exc) {
        //            _identityUtilService.RefreshToken();

        //            throw exc;
        //        }
        //        catch (HttpRequestExceptionEx exc) {
        //            setAppBackgroundImageResponse = JsonConvert.DeserializeObject<SetAppBackgroundImageResponse>(exc.Message);

        //            string output = setAppBackgroundImageResponse?.Errors?.FirstOrDefault();

        //            output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? _SET_APP_BACKGROUND_IMAGE_COMMON_ERROR_MESSAGE : output;

        //            throw new InvalidOperationException(output.Trim());
        //        }
        //        catch (Exception exc) {
        //            Crashes.TrackError(exc);

        //            throw exc;
        //        }

        //        return profileMediaDTO;
        //    });

        public Task<bool> SetAppBackgroundImage(long mediaId, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                SetAppBackgroundImageResponse setAppBackgroundImageResponse = null;
                bool completion = false;

                SetAppBackgroundImageRequest setAppBackgroundImageRequest = new SetAppBackgroundImageRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = new SetAppBackgroundImageDataModel() {
                        MediaId = mediaId
                    },
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.SetAppBackgroundImageEndPoint
                };

                try {
                    setAppBackgroundImageResponse = await _requestProvider.PostAsync<SetAppBackgroundImageRequest, SetAppBackgroundImageResponse>(setAppBackgroundImageRequest);

                    if (setAppBackgroundImageResponse != null) {
                        completion = true;
                    }
                    else {
                        completion = false;
                        throw new InvalidOperationException(ProfileService._SET_APP_BACKGROUND_IMAGE_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    completion = false;
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    completion = false;
                    setAppBackgroundImageResponse = JsonConvert.DeserializeObject<SetAppBackgroundImageResponse>(exc.Message);

                    string output = setAppBackgroundImageResponse?.Errors?.FirstOrDefault();

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? _SET_APP_BACKGROUND_IMAGE_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    completion = false;
                    Crashes.TrackError(exc);

                    throw exc;
                }

                return completion;
            });

        public Task ImpersonateLoginAsync(long targetId, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ImpersonateLogInRequest impersonateLogInRequest = new ImpersonateLogInRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.AuthenticationEndpoints.ImpersonateLogIn,
                    Data = new ImpersonateLogInDataModel() {
                        ChildProfileId = targetId
                    }
                };

                try {
                    ImpersonateLogInResponse impersonateLogInResponse = await _requestProvider.PostAsync<ImpersonateLogInRequest, ImpersonateLogInResponse>(impersonateLogInRequest);

                    if (impersonateLogInResponse == null) {
                        throw new InvalidOperationException(_IMPERSONATE_LOGIN_COMMON_ERROR_MESSAGE);
                    }
                    else {
                        GetProfileResponse getProfileResponse = await GetProfileAsync(impersonateLogInResponse.AccessToken, cancellationTokenSource);

                        if (getProfileResponse == null) {
                            throw new InvalidOperationException(_IMPERSONATE_LOGIN_COMMON_ERROR_MESSAGE);
                        }
                        else {
                            _navigationService.DisposeStack();

                            await _identityUtilService.ChargeImpersonateUserProfileAsync(impersonateLogInResponse.AccessToken, getProfileResponse);

                            Device.BeginInvokeOnMainThread(() => {
                                _navigationService.Initialize(true);
                            });
                        }
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw exc;
                }
            }, cancellationTokenSource.Token);

        public Task TryToRefreshLocalUserProfile(CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GetProfileRequest getProfileRequest = new GetProfileRequest {
                    Url = GlobalSettings.Instance.Endpoints.ProfileEndpoints.GetProfileEndPoints,
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                try {
                    await _identityUtilService.ChargeUserProfileAsync(await _requestProvider.GetAsync<GetProfileRequest, GetProfileResponse>(getProfileRequest), true);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }

            }, cancellationTokenSource.Token);
    }
}
