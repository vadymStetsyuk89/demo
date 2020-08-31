using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent.Teams;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Teams;
using PeakMVP.Models.Rests.Requests.Teams;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Teams {
    public class TeamService : ITeamService {

        public static readonly string GET_MEMBERS_BY_TEAM_ID_COMMON_ERROR_MESSAGE = "Can't get members for this team";
        public static readonly string REMOVE_TEAM_BY_ID_COMMON_ERROR_MESSAGE = "Can't remove this team";
        public static readonly string CREATE_NEW_TEAM_COMMON_ERROR_MESSAGE = "Can't create new tem now";
        public static readonly string GET_TEAMS_COMMON_ERROR_MESSAGE = "Can't get teams";
        public static readonly string APPOINTMENT_TO_THE_TEAM_COMMON_ERROR_MESSAGE = "Can't make appointment to the team";
        public static readonly string NIX_APPOINTMENT_TO_THE_TEAM_COMMON_ERROR_MESSAGE = "Can't nix appointment to the team";
        public static readonly string CHECK_APPOINTMENT_STATUS_COMMON_ERROR_MESSAGE = "Can't check appointment status";
        public static readonly string TEAM_END_PARTNERSHIP_WITH_ORGANIZATION_COMMON_ERROR_MESSAGE = "Can't end tihs partnership now";
        public static readonly string CANT_GET_TEAM_ERROR = "Can't get team";
        public static readonly string INVITE_EXTERNAL_MEMBER_COMMON_ERROR_MESSAGE = "Can't invite external member";
        public static readonly string RESEND_INVITE_TO_EXTERNAL_COMMON_ERROR_MESSAGE = "Can't resend invite to external member";
        public static readonly string GET_TEAM_EXTERNAL_INVITES_ERROR_MESSAGE = "Can't resolve external invites";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly ITeamFactory _teamFactory;
        private readonly IProfileFactory _profileFactory;

        /// <summary>
        ///     ctor().
        /// </summary>
        public TeamService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService,
            ITeamFactory teamFactory,
            IProfileFactory profileFactory) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
            _teamFactory = teamFactory;
            _profileFactory = profileFactory;
        }

        public Task<CreateTeamResponse> CreateTeamAsync(CreateTeamDataModel createTeamDataModel, CancellationToken cancellationToken = default(CancellationToken)) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                CreateTeamRequest createTeamRequest = new CreateTeamRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.TeamEndPoints.CreateTeamEndPoint,
                    Data = createTeamDataModel
                };

                CreateTeamResponse createTeamResponse = null;

                try {
                    createTeamResponse = await _requestProvider.PostAsync<CreateTeamRequest, CreateTeamResponse>(createTeamRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    CreateTeamBadResponse createTeamBadResponse = JsonConvert.DeserializeObject<CreateTeamBadResponse>(exc.Message);

                    string output = string.Format("{0}",
                        createTeamBadResponse.Name?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? TeamService.CREATE_NEW_TEAM_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);
                    throw;
                }

                return createTeamResponse;
            }, cancellationToken);

        public Task<List<TeamMember>> GetMembersByTeamIdAsync(long teamId, CancellationTokenSource cancellationTokenSource) =>
            Task<List<TeamMember>>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<TeamMember> foundMembers = null;

                GetMembersByTeamIdRequest getMembersByTeamIdRequest = new GetMembersByTeamIdRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.GetMembersByTeamIdEndPoint, teamId)
                };

                try {
                    GetMembersByTeamIdResponse getMembersByTeamIdResponse = await _requestProvider.GetAsync<GetMembersByTeamIdRequest, GetMembersByTeamIdResponse>(getMembersByTeamIdRequest);

                    if (getMembersByTeamIdResponse != null) {
                        foundMembers = (getMembersByTeamIdResponse.Data != null) ? getMembersByTeamIdResponse.Data.ToList() : new List<TeamMember>();
                    }
                    else {
                        throw new InvalidOperationException(TeamService.GET_MEMBERS_BY_TEAM_ID_COMMON_ERROR_MESSAGE);
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

                return foundMembers;
            }, cancellationTokenSource.Token);

        public Task<bool> RemoveTeamByIdAsync(long teamId, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool deleteCompletion = false;

                RemoveTeamRequest removeTeamRequest = new RemoveTeamRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.RemoveTeamByIdEndPoint, teamId)
                };

                RemoveTeamResponse removeTeamResponse = null;

                try {
                    removeTeamResponse = await _requestProvider.PostAsync<RemoveTeamRequest, RemoveTeamResponse>(removeTeamRequest);

                    if (removeTeamResponse != null) {
                        deleteCompletion = true;
                    }
                    else {
                        throw new InvalidOperationException(TeamService.REMOVE_TEAM_BY_ID_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    removeTeamResponse = JsonConvert.DeserializeObject<RemoveTeamResponse>(exc.Message);

                    string output = string.Format("{0}",
                        removeTeamResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? TeamService.REMOVE_TEAM_BY_ID_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return deleteCompletion;
            }, cancellationTokenSource.Token);

        public Task<List<TeamDTO>> GetTeamsAsync(CancellationTokenSource cancellationTokenSource) =>
            Task<List<TeamDTO>>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<TeamDTO> foundTeams = null;

                GetTeamsRequest getTeamsRequest = new GetTeamsRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.TeamEndPoints.GetAllTeams
                };

                try {
                    GetTeamsResponse getTeamsResponse = await _requestProvider.GetAsync<GetTeamsRequest, GetTeamsResponse>(getTeamsRequest);

                    if (getTeamsResponse != null) {
                        foundTeams = (getTeamsResponse.Data != null) ? getTeamsResponse.Data.ToList() : new List<TeamDTO>();
                    }
                    else {
                        throw new InvalidOperationException(TeamService.GET_TEAMS_COMMON_ERROR_MESSAGE);
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

                return foundTeams;
            }, cancellationTokenSource.Token);

        /// <summary>
        /// API doesn't provide get team by id method
        /// </summary>
        public Task<TeamDTO> ResolveFullTeamInfoByTeamIdAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task<TeamDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamDTO resolvedTeam = null;

                try {
                    List<TeamDTO> foundTeams = await GetTeamsAsync(cancellationTokenSource);

                    resolvedTeam = foundTeams.FirstOrDefault((tDTO) => tDTO.Id == targetTeamId);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    Debugger.Break();
                }

                return resolvedTeam;
            }, cancellationTokenSource.Token);

        public Task<bool> ResolveIsRequestToJoinTeamWasSentAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task<TeamDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool isRequestWasSent = false;

                IsRequestToJoinTeamWasSentRequest isRequestToJoinTeamWasSentRequest = new IsRequestToJoinTeamWasSentRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.StatusRequestToJoinToTeam, targetTeamId)
                };

                IsRequestToJoinTeamWasSentResponse isRequestToJoinTeamWasSentResponse = null;

                try {
                    isRequestToJoinTeamWasSentResponse = await _requestProvider.GetAsync<IsRequestToJoinTeamWasSentRequest, IsRequestToJoinTeamWasSentResponse>(isRequestToJoinTeamWasSentRequest);

                    isRequestWasSent = isRequestToJoinTeamWasSentResponse != null;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return isRequestWasSent;
            }, cancellationTokenSource.Token);

        public Task<TeamDTO> GetTeamByIdAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task<TeamDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamDTO team = null;

                GetTeamByIdRequest getTeamByIdRequest = new GetTeamByIdRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.GetTeamById, targetTeamId)
                };

                GetTeamByIdResponse getTeamByIdResponse = null;

                try {
                    getTeamByIdResponse = await _requestProvider.GetAsync<GetTeamByIdRequest, GetTeamByIdResponse>(getTeamByIdRequest);

                    if (getTeamByIdResponse == null) {
                        throw new InvalidOperationException(CANT_GET_TEAM_ERROR);
                    }

                    team = _teamFactory.BuildTeam(getTeamByIdResponse);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return team;
            }, cancellationTokenSource.Token);

        public Task<bool> SendTeamAppointmentRequestAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool completioin = false;

                SendTeamAppointmentRequest sendTeamAppointmentRequest = new SendTeamAppointmentRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.SendTeamAppointmentRequest, targetTeamId)
                };

                SendTeamAppointmentResponse sendTeamAppointmentResponse = null;

                try {
                    sendTeamAppointmentResponse = await _requestProvider.PostAsync<SendTeamAppointmentRequest, SendTeamAppointmentResponse>(sendTeamAppointmentRequest);

                    if (sendTeamAppointmentResponse != null) {
                        completioin = true;
                    }
                    else {
                        throw new InvalidOperationException(TeamService.APPOINTMENT_TO_THE_TEAM_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    SendTeamAppointmentBadResponse sendTeamAppointmentBadResponse = JsonConvert.DeserializeObject<SendTeamAppointmentBadResponse>(exc.Message);

                    string output = string.Format("{0}",
                        sendTeamAppointmentBadResponse.Team?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? TeamService.APPOINTMENT_TO_THE_TEAM_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return completioin;
            }, cancellationTokenSource.Token);

        public Task<ProfileDTO> NixTeamAppointmentAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task<ProfileDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ProfileDTO resultProfile = null;

                NixTeamAppointmentRequest nixTeamAppointmentRequest = new NixTeamAppointmentRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.NixTeamAppointmentRequest, targetTeamId)
                };

                NixTeamAppointmentResponse nixTeamAppointmentResponse = null;

                try {
                    nixTeamAppointmentResponse = await _requestProvider.DeleteAsync<NixTeamAppointmentRequest, NixTeamAppointmentResponse>(nixTeamAppointmentRequest);

                    if (nixTeamAppointmentResponse != null) {
                        resultProfile = _profileFactory.BuildProfileDTO(nixTeamAppointmentResponse);
                    }
                    else {
                        throw new InvalidOperationException(TeamService.NIX_APPOINTMENT_TO_THE_TEAM_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    nixTeamAppointmentResponse = JsonConvert.DeserializeObject<NixTeamAppointmentResponse>(exc.Message);

                    string output = string.Format("{0}",
                        nixTeamAppointmentResponse.Relation?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? TeamService.NIX_APPOINTMENT_TO_THE_TEAM_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return resultProfile;
            }, cancellationTokenSource.Token);

        public Task<TeamAppointmentStatusDTO> CheckAppointmentStatusAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource) =>
            Task<TeamAppointmentStatusDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamAppointmentStatusDTO statusResult = null;

                CheckTeamAppointmentStatusRequest checkTeamAppointmentStatusRequest = new CheckTeamAppointmentStatusRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.CheckTeamAppointmentStatusApiKey, targetTeamId)
                };

                CheckTeamAppointmentStatusResponse checkTeamAppointmentStatusResponse = null;

                try {
                    checkTeamAppointmentStatusResponse = await _requestProvider.GetAsync<CheckTeamAppointmentStatusRequest, CheckTeamAppointmentStatusResponse>(checkTeamAppointmentStatusRequest);

                    statusResult = BuildTeamAppointmentStatusDTO(checkTeamAppointmentStatusResponse);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    Debugger.Break();
                    throw;
                }

                return statusResult;
            });

        public Task<TeamDTO> EndPartnershipWithOrganization(long teamId, CancellationTokenSource cancellationTokenSource) =>
            Task<TeamDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamDTO resultTeam = null;

                TeamEndsPartnershipWithOrganizationRequest teamEndsPartnershipWithOrganizationRequest = new TeamEndsPartnershipWithOrganizationRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.TeamEndPartnershipWithOrganizationApiKey, teamId)
                };

                try {
                    TeamEndsPartnershipWithOrganizationResponse teamEndsPartnershipWithOrganizationResponse =
                        await _requestProvider.PostAsync<TeamEndsPartnershipWithOrganizationRequest, TeamEndsPartnershipWithOrganizationResponse>(teamEndsPartnershipWithOrganizationRequest);

                    if (teamEndsPartnershipWithOrganizationResponse == null) {
                        throw new InvalidOperationException(TEAM_END_PARTNERSHIP_WITH_ORGANIZATION_COMMON_ERROR_MESSAGE);
                    }

                    if (teamEndsPartnershipWithOrganizationResponse.Errors != null || teamEndsPartnershipWithOrganizationResponse.OrgError != null ||
                        teamEndsPartnershipWithOrganizationResponse.ProfileError != null || teamEndsPartnershipWithOrganizationResponse.TeamError != null) {
                        throw new InvalidOperationException(string.Format("{0} {1} {2} {3}",
                            teamEndsPartnershipWithOrganizationResponse.Errors?.FirstOrDefault(), teamEndsPartnershipWithOrganizationResponse.OrgError?.FirstOrDefault(),
                            teamEndsPartnershipWithOrganizationResponse.ProfileError?.FirstOrDefault(), teamEndsPartnershipWithOrganizationResponse.TeamError?.FirstOrDefault()).Trim());
                    }
                    else {
                        resultTeam = _teamFactory.BuildTeam(teamEndsPartnershipWithOrganizationResponse);
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

                return resultTeam;
            }, cancellationTokenSource.Token);

        public async Task<GetTeamRequestsResponse> GetTeamRequestsAsync(long id, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                GetTeamRequestsRequest getTeamRequestsRequest = new GetTeamRequestsRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.GetTeamRequestsEndPoint, id)
                };

                GetTeamRequestsResponse getTeamRequestsResponse = null;

                try {
                    getTeamRequestsResponse = await _requestProvider.GetAsync<GetTeamRequestsRequest, GetTeamRequestsResponse>(getTeamRequestsRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return getTeamRequestsResponse;
            });

        public async Task<ApproveTeamRequestsResponse> ApproveTeamRequestsAsync(long requestId, long teamId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ApproveTeamRequestsRequest approveTeamRequestsRequest = new ApproveTeamRequestsRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.ApproveTeamRequestRndpoint, teamId, requestId)
                };
                ApproveTeamRequestsResponse approveTeamRequestsResponse = null;
                try {
                    approveTeamRequestsResponse =
                        await _requestProvider.PutAsync<ApproveTeamRequestsRequest, ApproveTeamRequestsResponse>(approveTeamRequestsRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }
                return approveTeamRequestsResponse;
            });

        public async Task<RejectTeamRequestsResponse> RejectTeamRequestsAsync(long requestId, long teamId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                RejectTeamRequestsRequest rejectTeamRequestsRequest = new RejectTeamRequestsRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.RejectTeamRequestRndpoint, teamId, requestId)
                };

                RejectTeamRequestsResponse rejectTeamRequestsResponse = null;

                try {
                    rejectTeamRequestsResponse =
                        await _requestProvider.DeleteAsync<RejectTeamRequestsRequest, RejectTeamRequestsResponse>(rejectTeamRequestsRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return rejectTeamRequestsResponse;
            });

        public Task<TeamMember[]> GetFilteredMembersAsync(long teamId, TeamMemberFilters filter, CancellationTokenSource cancellationTokenSource) =>
            Task<TeamMember[]>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamMember[] foundMembers = null;

                try {
                    foundMembers = (await GetMembersByTeamIdAsync(teamId, cancellationTokenSource)).ToArray();

                    if (foundMembers != null) {
                        switch (filter) {
                            case TeamMemberFilters.All:
                                break;
                            case TeamMemberFilters.Players:
                                foundMembers = foundMembers.Where<TeamMember>(teamMember => teamMember.Member.Type == ProfileType.Player.ToString()).ToArray();
                                break;
                            case TeamMemberFilters.Staff:
                                foundMembers = foundMembers.Where<TeamMember>(teamMember => (teamMember.Member.Type == ProfileType.Organization.ToString()) || (teamMember.Member.Type == ProfileType.Coach.ToString())).ToArray();
                                break;
                            default:
                                Debugger.Break();
                                break;
                        }
                    }
                    else {
                        throw new InvalidOperationException(string.Format(GET_MEMBERS_BY_TEAM_ID_COMMON_ERROR_MESSAGE));
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw;
                }

                return foundMembers;
            }, cancellationTokenSource.Token);

        public Task<InviteExternalMemberToTeamResponse> InviteExternalMemberToTeamAsync(ExternalMemberTeamIntive externalInvite, CancellationTokenSource cancellationTokenSource) =>
            Task<InviteExternalMemberToTeamResponse>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                InviteExternalMemberToTeamRequest inviteExternalMemberToTeamRequest = new InviteExternalMemberToTeamRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.TeamEndPoints.InviteExternalMemberToTheTeam,
                    Data = externalInvite
                };

                InviteExternalMemberToTeamResponse inviteExternalResponse = null;

                try {
                    inviteExternalResponse = await _requestProvider.PostAsync<InviteExternalMemberToTeamRequest, InviteExternalMemberToTeamResponse>(inviteExternalMemberToTeamRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    try {
                        inviteExternalResponse = JsonConvert.DeserializeObject<InviteExternalMemberToTeamResponse>(exc.Message);
                    }
                    catch {
                        Debugger.Break();
                        throw new InvalidOperationException(INVITE_EXTERNAL_MEMBER_COMMON_ERROR_MESSAGE);
                    }

                    string output = string.Format("{0} {1} {2}",
                        inviteExternalResponse.Errors?.FirstOrDefault(),
                        inviteExternalResponse.Model?.FirstOrDefault(),
                        inviteExternalResponse.Email?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? INVITE_EXTERNAL_MEMBER_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return inviteExternalResponse;
            }, cancellationTokenSource.Token);

        public Task<List<ExternalInvite>> GetExternalIvitesByTeamIdAsync(long teamId, CancellationTokenSource cancellationTokenSource) =>
            Task<List<ExternalInvite>>.Run(async () => {

                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                TeamExternalInvitesRequest teamExternalInvitesRequest = new TeamExternalInvitesRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.ExternalInvitesByTeamId, teamId)
                };

                TeamExternalInvitesResponse teamExternalInvitesResponse = null;
                List<ExternalInvite> externalInvites = null;

                try {
                    teamExternalInvitesResponse = await _requestProvider.GetAsync<TeamExternalInvitesRequest, TeamExternalInvitesResponse>(teamExternalInvitesRequest);

                    if (teamExternalInvitesResponse != null) {
                        externalInvites = new List<ExternalInvite>(teamExternalInvitesResponse);
                    }
                    else {
                        throw new InvalidOperationException(GET_TEAM_EXTERNAL_INVITES_ERROR_MESSAGE);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    try {
                        teamExternalInvitesResponse = JsonConvert.DeserializeObject<TeamExternalInvitesResponse>(exc.Message);
                    }
                    catch {
                        Debugger.Break();
                        throw new InvalidOperationException(GET_TEAM_EXTERNAL_INVITES_ERROR_MESSAGE);
                    }

                    string output = string.Format("{0}",
                        teamExternalInvitesResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? GET_TEAM_EXTERNAL_INVITES_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return externalInvites;
            }, cancellationTokenSource.Token);

        public Task<ResendExternalMemberInviteResponse> ResendExternalMemberInviteAsync(long teamId, string externalMemberEmail, CancellationTokenSource cancellationTokenSource) =>
            Task<ResendExternalMemberInviteResponse>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ResendExternalMemberInviteRequest resendExternalMemberInviteRequest = new ResendExternalMemberInviteRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.TeamEndPoints.ResendExternalMemberInvite, teamId),
                    Data = new ExternalMemberTeamIntive() {
                        Email = externalMemberEmail
                    }
                };

                ResendExternalMemberInviteResponse resendExternalMemberInviteResponse = null;

                try {
                    resendExternalMemberInviteResponse = await _requestProvider.PutAsync<ResendExternalMemberInviteRequest, ResendExternalMemberInviteResponse>(resendExternalMemberInviteRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    try {
                        resendExternalMemberInviteResponse = JsonConvert.DeserializeObject<ResendExternalMemberInviteResponse>(exc.Message);
                    }
                    catch {
                        Debugger.Break();
                        throw new InvalidOperationException(RESEND_INVITE_TO_EXTERNAL_COMMON_ERROR_MESSAGE);
                    }

                    string output = string.Format("{0}",
                        resendExternalMemberInviteResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? RESEND_INVITE_TO_EXTERNAL_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output.Trim());
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                    throw;
                }

                return resendExternalMemberInviteResponse;
            }, cancellationTokenSource.Token);

        private TeamAppointmentStatusDTO BuildTeamAppointmentStatusDTO(CheckTeamAppointmentStatusResponse data) {
            if (!CrossConnectivity.Current.IsConnected) {
                throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
            }

            TeamAppointmentStatusDTO result = null;

            if (data != null) {
                result = new TeamAppointmentStatusDTO() {
                    Id = data.Id,
                    Profile = data.Profile,
                    Team = data.Team
                };

            }

            return result;
        }
    }
}
