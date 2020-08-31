using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Invites;
using PeakMVP.Models.Rests.Responses.Invites;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Invites {
    public class InviteService : IInviteService {

        private static readonly string INVITE_MEMBERS_TO_THE_TEAM_COMMON_ERROR_MESSAGE = "Exception. Can't invite member to the team";
        private static readonly string MEMBER_SUCCESSFULLY_TO_THE_TEAM_MESSAGE = "{0} successfully invited to the team";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        public InviteService(IRequestProvider requestProvider, IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
        }

        public async Task<InvitesResponse> GetInvitesAsync(CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                InvitesRequest invitesRequest = new InvitesRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.InviteEndPoints.GetInvitesEndPoint
                };

                InvitesResponse invitesResponse = null;

                try {
                    invitesResponse = await _requestProvider.GetAsync<InvitesRequest, InvitesResponse>(invitesRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    Debug.WriteLine($"ERROR:{ex.Message}");
                }
                return invitesResponse;
            }, cancellationToken);

        public Task<Tuple<List<ProfileDTO>, string>> AddMembersToTheTeamByTeamIdAsync(long targetTeamId, IEnumerable<long> targetMemberIds, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<ProfileDTO> addedMembers = new List<ProfileDTO>();
                string messageOutput = "";

                for (int i = 0; i < targetMemberIds.Count(); i++) {
                    try {
                        ProfileDTO addedMember = await AddSingleMemberToTheTeamByTeamIdAsync(targetTeamId, targetMemberIds.ElementAt(i));

                        if (addedMember != null) {
                            addedMembers.Add(addedMember);
                        }
                        else {
                            throw new InvalidOperationException(INVITE_MEMBERS_TO_THE_TEAM_COMMON_ERROR_MESSAGE);
                        }
                    }
                    catch (Exception exc) {
                        messageOutput = string.Format("{0}{1} ", messageOutput, exc.Message);
                    }
                }

                return new Tuple<List<ProfileDTO>, string>(addedMembers, messageOutput);
            }, cancellationTokenSource.Token);

        private Task<ProfileDTO> AddSingleMemberToTheTeamByTeamIdAsync(long targetTeamId, long targetMemberId) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ProfileDTO addedMember = null;

                InviteMemberToTeamByIdRequest inviteMemberToTeamByIdRequest = new InviteMemberToTeamByIdRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.InviteEndPoints.InviteMemberToTeamByIdEndPoint, targetTeamId),
                    Data = targetMemberId
                };

                InviteMemberToTeamByIdResponse inviteMemberToTeamByIdResponse = null;

                try {
                    inviteMemberToTeamByIdResponse = await _requestProvider.PostAsync<InviteMemberToTeamByIdRequest, InviteMemberToTeamByIdResponse>(inviteMemberToTeamByIdRequest);

                    if (inviteMemberToTeamByIdResponse != null) {
                        addedMember = inviteMemberToTeamByIdResponse.Member;
                    }
                    else {
                        throw new InvalidOperationException(INVITE_MEMBERS_TO_THE_TEAM_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    inviteMemberToTeamByIdResponse = JsonConvert.DeserializeObject<InviteMemberToTeamByIdResponse>(exc.Message);

                    string output = string.Format("{0} {1}",
                                            inviteMemberToTeamByIdResponse.Errors?.FirstOrDefault(),
                                            inviteMemberToTeamByIdResponse.UserToAdd?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? INVITE_MEMBERS_TO_THE_TEAM_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return addedMember;
            });

        public async Task<TeamInviteConfirmResponse> TeamInviteConfirmAsync(long teamId, CancellationToken cancellationToken, long? profileId = null) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamInviteConfirmRequest teamInviteConfirmRequest = new TeamInviteConfirmRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.InviteEndPoints.TeamInviteConfirmEndPoint, teamId),
                    Data = (profileId.HasValue) ? profileId.Value : GlobalSettings.Instance.UserProfile.Id
                };

                TeamInviteConfirmResponse teamInviteConfirmResponse = null;
                try {
                    teamInviteConfirmResponse = await _requestProvider.PutAsync<TeamInviteConfirmRequest, TeamInviteConfirmResponse>(teamInviteConfirmRequest);
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
                return teamInviteConfirmResponse;
            }, cancellationToken);

        public async Task<TeamInviteRejectResponse> TeamInviteRejectAsync(long teamId, CancellationToken cancellationToken, long? profileId = null) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamInviteRejectRequest teamInviteRejectRequest = new TeamInviteRejectRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.InviteEndPoints.TeamInviteRejectEndPoint, teamId),
                    Data = (profileId.HasValue)?profileId.Value: GlobalSettings.Instance.UserProfile.Id
                };

                TeamInviteRejectResponse teamInviteRejectResponse = null;

                try {
                    teamInviteRejectResponse = await _requestProvider.PostAsync<TeamInviteRejectRequest, TeamInviteRejectResponse>(teamInviteRejectRequest);
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
                return teamInviteRejectResponse;
            }, cancellationToken);
    }
}
