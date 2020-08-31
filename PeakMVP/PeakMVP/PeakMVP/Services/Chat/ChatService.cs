using Microsoft.AppCenter.Crashes;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Chat;
using PeakMVP.Models.Rests.Responses.Chat;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Chat {
    public class ChatService : IChatService {

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        public ChatService(IRequestProvider requestProvider, IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
        }

        public async Task<List<MessageDTO>> GetFriendChatAsync(long friendId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<MessageDTO> messages = new List<MessageDTO>();

                GetMessagesRequest getMessagesRequest = new GetMessagesRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ChatEndPoints.GetMessagesEndPoint,
                        string.Empty,
                        friendId,
                        GlobalSettings.Instance.UserProfile.Id,
                        GlobalSettings.Instance.UserProfile.ProfileType.ToString())
                };

                GetMessagesResponse getAllFriendsResponse = null;

                try {
                    getAllFriendsResponse = await _requestProvider.GetAsync<GetMessagesRequest, GetMessagesResponse>(getMessagesRequest);
                    if (getAllFriendsResponse != null) {
                        messages = getAllFriendsResponse?.Messages.ToList();
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
                return messages;
            }, cancellationToken);

        public async Task<List<MessageDTO>> GetGroupChatAsync(long groupId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<MessageDTO> messages = new List<MessageDTO>();

                GetGroupChatRequest getGroupChatRequest = new GetGroupChatRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ChatEndPoints.GetGroupChatEndPoint, groupId)
                };

                GetGroupChatResponse getGroupChatResponse = null;

                try {
                    getGroupChatResponse = await _requestProvider.GetAsync<GetGroupChatRequest, GetGroupChatResponse>(getGroupChatRequest);

                    if (getGroupChatResponse != null) {
                        messages = getGroupChatResponse?.GroupMessages.ToList();
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

                return messages;
            }, cancellationToken);

        public async Task<List<MessageDTO>> GetTeamChatAsync(long teamId, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<MessageDTO> messages = new List<MessageDTO>();

                GetTeamChatRequest getTeamChatRequest = new GetTeamChatRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ChatEndPoints.GetTeamChatEndPoint, teamId)
                };

                GetTeamChatResponse getTeamChatResponse = null;

                try {
                    getTeamChatResponse = await _requestProvider.GetAsync<GetTeamChatRequest, GetTeamChatResponse>(getTeamChatRequest);
                    if (getTeamChatResponse != null) {
                        messages = getTeamChatResponse?.TeamMessages.ToList();
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

                return messages;
            }, cancellationToken);

        public async Task<List<MessageDTO>> GetFamilyChatAsync(long profileId, CancellationToken cancellationToken = default(CancellationToken)) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<MessageDTO> messages = new List<MessageDTO>();

                GetFamilyChatRequest getFamilyChatRequest = new GetFamilyChatRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ChatEndPoints.GetFamilyChatEndPoint, profileId)
                };

                GetFamilyChatResponse getFamilyChatResponse = null;

                try {
                    getFamilyChatResponse = await _requestProvider.GetAsync<GetFamilyChatRequest, GetFamilyChatResponse>(getFamilyChatRequest);
                    if (getFamilyChatResponse != null) {
                        messages = getFamilyChatResponse?.FamilyMessages.ToList();
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

                return messages;
            }, cancellationToken);

        public Task<int> GetUnreadMessagesCountAsync(long companionId, MessagingCompanionType companionType) =>
            Task<int>.Run(async () => {
                int result = 0;

                try {
                    switch (companionType) {
                        case MessagingCompanionType.Family:
                            result = (await GetFamilyChatAsync(companionId, new CancellationToken())).Where<MessageDTO>(message => !message.Seen).Count();
                            break;
                        case MessagingCompanionType.Friend:
                            result = (await GetFriendChatAsync(companionId, new CancellationToken())).Where<MessageDTO>(message => !message.Seen).Count();
                            break;
                        case MessagingCompanionType.Team:
                            result = (await GetTeamChatAsync(companionId, new CancellationToken())).Where<MessageDTO>(message => !message.Seen).Count();
                            break;
                        case MessagingCompanionType.Group:
                            result = (await GetGroupChatAsync(companionId, new CancellationToken())).Where<MessageDTO>(message => !message.Seen).Count();
                            break;
                        default:
                            Debugger.Break();
                            throw new InvalidOperationException("Unrezolved messaging companion type");
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);
                }

                return result;
            });
    }
}
