using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Exceptions.MainContent;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Friends;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Friends;
using PeakMVP.Models.Rests.Responses.Friends;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Profile;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Friends {
    public class FriendService : IFriendService {

        private readonly IProfileService _profileService;
        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        public static readonly string DELELTE_FRIEND_ERROR = "Can't remove friend now";

        /// <summary>
        ///     ctor().
        /// </summary>
        public FriendService(
            IRequestProvider requestProvider,
            IProfileService profileService,
            IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _profileService = profileService;
            _identityUtilService = identityUtilService;
        }

        public async Task<AddFriendResponse> AddFriendAsync(string friendShortId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                // Get friend profile.
                GetProfileResponse friendProfile = await _profileService.GetProfileByShortIdAsync(friendShortId, cancellationToken);

                if (friendProfile == null) throw new Exception("Friend profile not found.");

                AddFriendRequest addFriendRequest = new AddFriendRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.FriendEndPoints.AddFriendRequestEndPoint,
                    Data = new AddFriendDataModel {
                        FriendId = friendProfile.Id,
                        ProfileId = GlobalSettings.Instance.UserProfile.Id,
                        ProfileType = GlobalSettings.Instance.UserProfile.ProfileType.ToString()
                    }
                };

                AddFriendResponse addFriendResponse = null;

                try {
                    addFriendResponse = await _requestProvider.PostAsync<AddFriendRequest, AddFriendResponse>(addFriendRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw new Exception((JsonConvert.DeserializeObject<AddFriendException>(ex.Message)).Empty.FirstOrDefault());
                }
                return addFriendResponse;
            }, cancellationToken);

        public async Task<List<FriendshipDTO>> GetAllFriendsAsync(CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<FriendshipDTO> friends = new List<FriendshipDTO>();

                GetAllFriendsRequest getAllFriendsRequest = new GetAllFriendsRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.FriendEndPoints.GetAllFriendsEndPoint,
                                        GlobalSettings.Instance.UserProfile.Id,
                                        GlobalSettings.Instance.UserProfile.ProfileType.ToString())
                };

                GetAllFriendsResponse getAllFriendsResponse = null;

                try {
                    getAllFriendsResponse = await _requestProvider.GetAsync<GetAllFriendsRequest, GetAllFriendsResponse>(getAllFriendsRequest);

                    if (getAllFriendsResponse != null && getAllFriendsResponse.Friends.Count() > 0) {
                        friends = getAllFriendsResponse?.Friends.ToList();
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (OperationCanceledException) { throw; }
                catch (ObjectDisposedException) { throw; }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw new Exception((JsonConvert.DeserializeObject<AddFriendException>(ex.Message)).Empty.FirstOrDefault());
                }
                return friends;
            }, cancellationToken);

        public async Task<GetFriendByIdResponse> GetFriendByIdAsync(long friendId, CancellationToken cancellationToken = default(CancellationToken)) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GetFriendByIdRequest getFriendByIdRequest = new GetFriendByIdRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.FriendEndPoints.GetFriendByIdEndPoint, friendId)
                };

                GetFriendByIdResponse getFriendByIdResponse = null;

                try {
                    getFriendByIdResponse = await _requestProvider.GetAsync<GetFriendByIdRequest, GetFriendByIdResponse>(getFriendByIdRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");
                }
                return getFriendByIdResponse;
            }, cancellationToken);

        public async Task<ConfirmFriendResponse> RequestConfirmAsync(long friendId, CancellationToken cancellationToken = default(CancellationToken), long? childProfileId = null) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ConfirmFriendRequest confirmFriendRequest = new ConfirmFriendRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.FriendEndPoints.ConfirmRequestEndPoint,
                    Data = new ConfirmRequestDataModel {
                        FriendId = friendId,
                        ChildId = childProfileId
                    }
                };

                ConfirmFriendResponse confirmFriendResponse = null;

                try {
                    confirmFriendResponse = await _requestProvider.PostAsync<ConfirmFriendRequest, ConfirmFriendResponse>(confirmFriendRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");

                    throw new Exception((JsonConvert.DeserializeObject<AddFriendException>(ex.Message)).Empty.FirstOrDefault());
                }

                return confirmFriendResponse;
            }, cancellationToken);

        public async Task<DeleteFriendResponse> RequestDeleteAsync(long friendId, CancellationToken cancellationToken = default(CancellationToken), long? childProfileId = null) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                DeleteFriendRequest deleteFriendRequest = new DeleteFriendRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.FriendEndPoints.DeleteRequestEndPoint,
                    Data = new ConfirmRequestDataModel {
                        FriendId = friendId,
                        ChildId = childProfileId
                    }
                };

                DeleteFriendResponse deleteFriendResponse = null;

                try {
                    deleteFriendResponse = await _requestProvider.PostAsync<DeleteFriendRequest, DeleteFriendResponse>(deleteFriendRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    DeleteFriendResponse deleteFriendBadResponse = JsonConvert.DeserializeObject<DeleteFriendResponse>(exc.Message);

                    string output = string.Format("{0} {1}",
                        deleteFriendBadResponse.Errors?.FirstOrDefault(),
                        deleteFriendBadResponse.Profile?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? DELELTE_FRIEND_ERROR : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw new InvalidOperationException((JsonConvert.DeserializeObject<AddFriendException>(ex.Message)).FriendId.FirstOrDefault(), ex);
                }
                return deleteFriendResponse;
            }, cancellationToken);
    }
}
