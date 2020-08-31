using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Groups;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Groups;
using PeakMVP.Models.Rests.Responses.Groups;
using PeakMVP.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using PeakMVP.Models.Exceptions;
using PeakMVP.Services.IdentityUtil;
using Newtonsoft.Json;
using System.Diagnostics;
using PeakMVP.Helpers;
using Plugin.Connectivity;

namespace PeakMVP.Services.Groups {
    public class GroupsService : IGroupsService {

        private static readonly string _CREATE_GROUP_COMMON_ERROR_MESSAGE = "Can't create group";
        private static readonly string _GET_GROUPS_COMMON_ERROR_MESSAGE = "Can't get groups";
        private static readonly string _INVITE_MEMBERS_TO_THE_GROUP_COMMON_ERROR_MESSAGE = "Can't invite new members to the group";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        public GroupsService(IRequestProvider requestProvider, IIdentityUtilService identityUtilService) {
            _requestProvider = requestProvider;

            _identityUtilService = identityUtilService;
        }

        public Task<GroupDTO> CreateNewGroupAsync(CreateGroupDataModel createGroupDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GroupDTO createdGroup = null;

                CreateGroupRequest createGroupRequest = new CreateGroupRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = createGroupDataModel,
                    Url = GlobalSettings.Instance.Endpoints.GroupsEndpoints.CreateGroup
                };

                try {
                    CreateGroupResponse createGroupResponse = await _requestProvider.PostAsync<CreateGroupRequest, CreateGroupResponse>(createGroupRequest);

                    if (createGroupResponse != null) {
                        createdGroup = BuildGroup(createGroupResponse);
                    }
                    else {
                        throw new InvalidOperationException(GroupsService._CREATE_GROUP_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return createdGroup;
            }, cancellationTokenSource.Token);

        public Task<List<GroupDTO>> GetGroupsAsync(CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<GroupDTO> foundGroups = null;

                GetGroupsRequest getGroupsRequest = new GetGroupsRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.GroupsEndpoints.GetGroups
                };

                try {
                    GetGroupsResponse getGroupsResponse = await _requestProvider.GetAsync<GetGroupsRequest, GetGroupsResponse>(getGroupsRequest);

                    if (getGroupsResponse != null) {
                        foundGroups = getGroupsResponse.Data.ToList();
                    }
                    else {
                        throw new InvalidOperationException(GroupsService._GET_GROUPS_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return foundGroups;
            }, cancellationTokenSource.Token);

        public Task<bool> DeleteGroupByIdAsync(long groupId, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool deleteCompletion = false;

                DeleteGroupRequest deleteGroupRequest = new DeleteGroupRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.GroupsEndpoints.DeletePostById, groupId)
                };

                try {
                    DeleteGroupResponse deleteGroupResponse = await _requestProvider.PostAsync<DeleteGroupRequest, DeleteGroupResponse>(deleteGroupRequest);

                    if (deleteGroupResponse != null) {
                        deleteCompletion = true;
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return deleteCompletion;
            }, cancellationTokenSource.Token);

        public Task<GroupDTO> GetGroupByIdAsync(long groupId, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GroupDTO foundGroup = null;

                GetGroupByIdRequest getGroupByIdRequest = new GetGroupByIdRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.GroupsEndpoints.GetGroupById, groupId)
                };

                try {
                    GetGroupByIdResponse getGroupByIdResponse = await _requestProvider.GetAsync<GetGroupByIdRequest, GetGroupByIdResponse>(getGroupByIdRequest);

                    if (getGroupByIdResponse != null) {
                        foundGroup = BuildGroup(getGroupByIdResponse);
                    }
                    else {
                        throw new InvalidOperationException(_GET_GROUPS_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return foundGroup;
            }, cancellationTokenSource.Token);

        public Task<List<MemberDTO>> InviteMemberToTheGroupAsync(InviteMembersToTheGroupDataModel data, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<MemberDTO> successfullyInvitedMembers = null;

                InviteMembersToTheGroupRequest inviteMembersToTheGroupRequest = new InviteMembersToTheGroupRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.GroupsEndpoints.InviteMembersToTheGroup, data.GroupId),
                    Data = data
                };

                InviteMembersToTheGroupResponse inviteMembersToTheGroupResponse;

                try {
                    inviteMembersToTheGroupResponse = await _requestProvider.PostAsync<InviteMembersToTheGroupRequest, InviteMembersToTheGroupResponse>(inviteMembersToTheGroupRequest);

                    if (inviteMembersToTheGroupResponse != null) {
                        successfullyInvitedMembers = new List<MemberDTO>((inviteMembersToTheGroupResponse.Data != null)
                            ? inviteMembersToTheGroupResponse.Data : new MemberDTO[] { });
                    }
                    else {
                        throw new InvalidOperationException(GroupsService._INVITE_MEMBERS_TO_THE_GROUP_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    inviteMembersToTheGroupResponse = JsonConvert.DeserializeObject<InviteMembersToTheGroupResponse>(exc.Message);

                    string output = string.Format("{0} {1}",
                        inviteMembersToTheGroupResponse.Errors?.FirstOrDefault(),
                        inviteMembersToTheGroupResponse.Id?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? GroupsService._INVITE_MEMBERS_TO_THE_GROUP_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return successfullyInvitedMembers;
            }, cancellationTokenSource.Token);

        public async Task<GroupRequestConfirmResponse> GroupRequestConfirmAsync(GroupRequestConfirmDataModel dataModel, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GroupRequestConfirmRequest groupRequestConfirmRequest = new GroupRequestConfirmRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = dataModel,
                    Url = GlobalSettings.Instance.Endpoints.GroupsEndpoints.GroupRequestConfirmEndPoint
                };

                GroupRequestConfirmResponse groupRequestConfirmResponse = null;

                try {
                    groupRequestConfirmResponse = await _requestProvider.PostAsync<GroupRequestConfirmRequest, GroupRequestConfirmResponse>(groupRequestConfirmRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw;
                }

                return groupRequestConfirmResponse;
            });

        public async Task<bool> GroupRequestDeclineAsync(GroupRequestDeclineDataModel dataModel, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool isDecline = default(bool);

                GroupRequestDeclineRequest groupRequestDeclineRequest = new GroupRequestDeclineRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = dataModel,
                    Url = GlobalSettings.Instance.Endpoints.GroupsEndpoints.GroupRequestDeclineEndPoint
                };

                GroupRequestDeclineResponse groupRequestDeclineResponse = null;

                try {
                    groupRequestDeclineResponse =
                                                            await _requestProvider.PostAsync<GroupRequestDeclineRequest, GroupRequestDeclineResponse>(groupRequestDeclineRequest);

                    if (groupRequestDeclineResponse != null) {
                        isDecline = true;
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");
                    isDecline = false;
                }

                return isDecline;
            });

        private GroupDTO BuildGroup(GroupResponseBase groupResponseBase) =>
            new GroupDTO() {
                Id = groupResponseBase.Id,
                Owner = groupResponseBase.Owner,
                Members = groupResponseBase.Members,
                Name = groupResponseBase.Name,
                OwnerId = groupResponseBase.OwnerId
            };
    }
}
