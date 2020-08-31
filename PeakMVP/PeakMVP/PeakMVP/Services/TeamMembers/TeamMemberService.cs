using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Requests;
using PeakMVP.Models.Rests.Requests.TeamMembers;
using PeakMVP.Models.Rests.Responses;
using PeakMVP.Models.Rests.Responses.TeamMembers;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.TeamMembers {
    public sealed class TeamMemberService : ITeamMemberService {

        public static readonly string CANT_GET_TEAM_MEMBERS_ERROR_MESSAGE = "Can't get team members now";
        public static readonly string CANT_GET_TEAM_MEMBER_BY_MEMBER_ID_ERROR_MESSAGE = "Can't get team member by member id now";
        public static readonly string CANT_ADD_CONTACT_INFO_COMMON_WARNING = "Can't add contact info";
        public static readonly string CANT_EDIT_CONTACT_INFO_COMMON_WARNING = "Can't edit contact info";
        public static readonly string CANT_DELETE_CONTACT_INFO_COMMON_WARNING = "Can't delete contact info";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly ITeamMemberFactory _teamMemberFactory;

        public TeamMemberService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService,
            ITeamMemberFactory teamMemberFactory) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
            _teamMemberFactory = teamMemberFactory;
        }

        public async Task<List<TeamMember>> GetTeamMembersAsync(CancellationToken cancellationToken = default(CancellationToken), bool noRepeatings = false) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<TeamMember> teamMembers = new List<TeamMember>();

                GetTeamMembersRequest getTeamMembersRequest = new GetTeamMembersRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.TeamMemberEndPoints.GetTeamMembersEndPoint
                };

                GetTeamMembersResponse getTeamMembersResponse = null;

                try {
                    getTeamMembersResponse = await _requestProvider.GetAsync<GetTeamMembersRequest, GetTeamMembersResponse>(getTeamMembersRequest);

                    if (getTeamMembersResponse != null) {
                        if (noRepeatings) {
                            teamMembers = getTeamMembersResponse.TeamMembers.GroupBy<TeamMember, long>(teamMember => teamMember.Team.Id).Select<IGrouping<long, TeamMember>, TeamMember>(group => group.First()).ToList();
                        }
                        else {
                            teamMembers = getTeamMembersResponse.TeamMembers.ToList();
                        }
                    }
                    else {
                        throw new InvalidOperationException(CANT_GET_TEAM_MEMBER_BY_MEMBER_ID_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw;
                }

                return teamMembers;
            }, cancellationToken);

        public async Task<TeamMember> GetTeamMemberByMemberIdAsync(long profileId, CancellationToken cancellationToken = default(CancellationToken)) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamMember teamMember = null;

                AuthorisedRequest getTeamMembersByMemberIdRequest = new AuthorisedRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamMemberEndPoints.GetTeamMembersByMemberIdEndPoint, profileId)
                };

                GetTeamMembersResponse getTeamMembersByMemberIdResponse = null;

                try {
                    getTeamMembersByMemberIdResponse = await _requestProvider.GetAsync<AuthorisedRequest, GetTeamMembersResponse>(getTeamMembersByMemberIdRequest);

                    if (getTeamMembersByMemberIdResponse != null) {
                        teamMember = getTeamMembersByMemberIdResponse.TeamMembers.FirstOrDefault<TeamMember>(foundTeamMember => foundTeamMember.Member.Id == profileId);
                    }
                    else {
                        throw new InvalidOperationException(CANT_GET_TEAM_MEMBERS_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw;
                }

                return teamMember;
            }, cancellationToken);

        public Task<bool> AddContactInfoAsync(long teamMemberId, TeamMemberContactInfo memberContactInfo, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool completion = false;

                AuthorisedRequest addTeamMemberContactInfoRequest = new AuthorisedRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamMemberEndPoints.AddContactInfoEndpoint, teamMemberId),
                    Data = new object[] { memberContactInfo }
                };

                CommonResponse addTeamMemberContactInfoResponse = null;

                try {
                    addTeamMemberContactInfoResponse = await _requestProvider.PostAsync<AuthorisedRequest, CommonResponse>(addTeamMemberContactInfoRequest);

                    if (addTeamMemberContactInfoResponse != null) {
                        completion = true;
                    }
                    else {
                        completion = false;
                        throw new InvalidOperationException(CANT_ADD_CONTACT_INFO_COMMON_WARNING);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw;
                }

                return completion;
            }, cancellationTokenSource.Token);

        public Task<bool> EditContactInfoAsync(long teamMemberId, IEnumerable<TeamMemberContactInfo> editedContactInfos, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool completion = false;

                AuthorisedRequest addTeamMemberContactInfoRequest = new AuthorisedRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamMemberEndPoints.EditContactInfoEndpoint, teamMemberId),
                    Data = editedContactInfos
                };

                CommonResponse editTeamMemberContactInfoResponse = null;

                try {
                    editTeamMemberContactInfoResponse = await _requestProvider.PatchAsync<AuthorisedRequest, CommonResponse>(addTeamMemberContactInfoRequest);

                    if (editTeamMemberContactInfoResponse != null) {
                        completion = true;
                    }
                    else {
                        completion = false;
                        throw new InvalidOperationException(CANT_EDIT_CONTACT_INFO_COMMON_WARNING);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw;
                }

                return completion;
            }, cancellationTokenSource.Token);

        public Task<bool> DeleteContactInfoAsync(long teamMemberId, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool completion = false;

                AuthorisedRequest deleteTeamMemberContactInfoRequest = new AuthorisedRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamMemberEndPoints.DeleteContactInfoEndpoint, teamMemberId)
                };

                CommonResponse deleteTeamMemberContactInfoResponse = null;

                try {
                    deleteTeamMemberContactInfoResponse = await _requestProvider.DeleteAsync<AuthorisedRequest, CommonResponse>(deleteTeamMemberContactInfoRequest);

                    if (deleteTeamMemberContactInfoResponse != null) {
                        completion = true;
                    }
                    else {
                        completion = false;
                        throw new InvalidOperationException(CANT_DELETE_CONTACT_INFO_COMMON_WARNING);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw;
                }

                return completion;
            }, cancellationTokenSource.Token);
    }
}
