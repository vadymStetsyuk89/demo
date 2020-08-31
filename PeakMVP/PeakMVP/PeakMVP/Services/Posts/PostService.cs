using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Posts;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Models.Rests.Responses.Posts;
using PeakMVP.Services.Family;
using PeakMVP.Services.Groups;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using PeakMVP.Services.TeamMembers;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace PeakMVP.Services.Posts {
    public class PostService : IPostService {

        public static readonly string POST_DELETE_ERROR_MESSAGE = "Can't remove that post at the moment";
        public static readonly string POST_NEW_POST_COMMON_ERROR_MESSAGE = "Can't make new post";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly IGroupsService _groupsService;
        private readonly IPostPublicityScopeFactory _postPublicityScopeFactory;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IFamilyService _familyService;

        public PostService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService,
            IGroupsService groupsService,
            IPostPublicityScopeFactory postPublicityScopeFactory,
            ITeamMemberService teamMemberService,
            IFamilyService familyService) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
            _groupsService = groupsService;
            _postPublicityScopeFactory = postPublicityScopeFactory;
            _teamMemberService = teamMemberService;
            _familyService = familyService;
        }

        public async Task<List<PostDTO>> GetPostsAsync(string authorshortId = "", string pageId = "", CancellationToken cancellationToken = default(CancellationToken)) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<PostDTO> posts = new List<PostDTO>();

                GetPostsRequest getPostsRequest = new GetPostsRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.PostEndpoints.GetPostsPoint,
                                        authorshortId,
                                        pageId,
                                        GlobalSettings.Instance.UserProfile.Id,
                                        GlobalSettings.Instance.UserProfile.ProfileType.ToString()),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                GetPostsResponse getPostsResponse = null;

                try {
                    getPostsResponse =
                        await _requestProvider.GetAsync<GetPostsRequest, GetPostsResponse>(getPostsRequest);

                    if (getPostsResponse.Posts != null) {
                        posts = getPostsResponse.Posts.Select<PostDTO, PostDTO>(pDTO => {
                            pDTO.PublishTime = pDTO.PublishTime.ToLocalTime();
                            pDTO.Comments.ForEach<CommentDTO>(cDTO => cDTO.CreationTime = cDTO.CreationTime.ToLocalTime());
                            pDTO.Text = NormalizeTextMessage(pDTO.Text);

                            return pDTO;
                        }).ToList();
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw new Exception(ex.Message);
                }

                return posts;
            }, cancellationToken);

        public Task<bool> PublishPostAsync(PublishPostDataModel publishPostDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                PublishNewPostRequest publishNewPostRequest = new PublishNewPostRequest() {
                    Url = GlobalSettings.Instance.Endpoints.PostEndpoints.PublishNewPostEndPoint,
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = publishPostDataModel
                };

                try {
                    PublishNewPostResponse publishNewPostResponse = await _requestProvider.PostAsync<PublishNewPostRequest, PublishNewPostResponse>(publishNewPostRequest);

                    if (publishNewPostResponse != null) {
                        return true;
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    string output = "";

                    try {
                        PublishNewPostBadResponse publishNewPostBadResponse = JsonConvert.DeserializeObject<PublishNewPostBadResponse>(exc.Message);

                        output = string.Format("{0} {1}",
                            publishNewPostBadResponse.Errors?.FirstOrDefault(),
                            publishNewPostBadResponse.Text?.FirstOrDefault().Split('\r').FirstOrDefault());
                    }
                    catch {
                        Debugger.Break();
                        output = POST_NEW_POST_COMMON_ERROR_MESSAGE;
                    }

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? POST_NEW_POST_COMMON_ERROR_MESSAGE : output;

                    throw new InvalidOperationException(output, exc);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debugger.Break();
                    return false;
                }

                return false;
            }, cancellationTokenSource.Token);

        public Task<bool> DeletePostByPostId(long postId, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool completionResult = false;

                DeletePostRequest deletePostRequest = new DeletePostRequest() {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.PostEndpoints.DeletePostByPostIdEndPoint, postId),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                try {
                    DeletePostResponse deletePostResponse = await _requestProvider.PostAsync<DeletePostRequest, DeletePostResponse>(deletePostRequest);

                    if (deletePostResponse != null) {
                        completionResult = true;
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debug.WriteLine($"ERROR:{exc.Message}");
                    throw;
                }

                return completionResult;
            }, cancellationTokenSource.Token);

        public async Task<CommentDTO> PublishCommentAsync(PublishCommentDataModel publishCommentDataModel, CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                PublishCommentRequest publishCommentRequest = new PublishCommentRequest {
                    Url = GlobalSettings.Instance.Endpoints.PostEndpoints.PublishCommentEndPoint,
                    Data = publishCommentDataModel,
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                PublishCommentResponse publishCommentResponse = null;
                CommentDTO commentResult = null;

                try {
                    publishCommentResponse =
                        await _requestProvider.PostAsync<PublishCommentRequest, PublishCommentResponse>(publishCommentRequest);

                    if (publishCommentResponse != null) {
                        commentResult = new CommentDTO() {
                            Author = publishCommentResponse.Author,
                            CreationTime = publishCommentResponse.CreationTime,
                            Id = publishCommentResponse.Id,
                            Text = publishCommentResponse.Text
                        };
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw new Exception(ex.Message);
                }

                return commentResult;
            }, cancellationToken);

        public Task<List<PostPublicityScope>> GetPossiblePostPublicityScopesAsync(CancellationTokenSource cancellationTokenSource) =>
            Task<List<PostPublicityScope>>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<PostPublicityScope> possiblePublicityScopes = null;

                try {
                    List<PostPublicityScope> groupsScope = _postPublicityScopeFactory.BuildRawPublicityScope(await _groupsService.GetGroupsAsync(cancellationTokenSource));
                    List<PostPublicityScope> teamMembersScope = _postPublicityScopeFactory.BuildRawPublicityScope(await _teamMemberService.GetTeamMembersAsync(cancellationTokenSource.Token, GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent));
                    PostPublicityScope familyPostPublicity = _postPublicityScopeFactory.BuildFamilyPublicityScope(await _familyService.GetFamilyAsync(cancellationTokenSource));

                    possiblePublicityScopes = _postPublicityScopeFactory.BuildCompletedPublicityScopeList(groupsScope.Concat(teamMembersScope).ToArray(), familyPostPublicity);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    possiblePublicityScopes = _postPublicityScopeFactory.BuildCompletedPublicityScopeList(null);
                }

                return possiblePublicityScopes;
            }, cancellationTokenSource.Token);

        public Task<bool> EditPostAsync(long postId, PublishPostDataModel publishPostDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task<PostDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                EditPostRequest editPostRequest = new EditPostRequest() {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.PostEndpoints.EditPostEndPoint, postId),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Data = publishPostDataModel
                };

                try {
                    EditPostResponse editPostResponse = await _requestProvider.PostAsync<EditPostRequest, EditPostResponse>(editPostRequest);

                    if (editPostResponse != null) {
                        return true;
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debug.WriteLine($"ERROR:{exc.Message}");
                    throw;
                }

                return false;
            }, cancellationTokenSource.Token);

        //public Task<PostDTO> EditPostAsync(EditPostDataModel editPostDataModel, CancellationTokenSource cancellationTokenSource) =>
        //    Task<PostDTO>.Run(async () => {
        //        if (!CrossConnectivity.Current.IsConnected) {
        //            throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
        //        }

        //        PostDTO editedPostDTO = null;

        //        EditPostRequest editPostRequest = new EditPostRequest() {
        //            Url = GlobalSettings.Instance.Endpoints.PostEndpoints.EditPostEndPoint,
        //            AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
        //            Data = editPostDataModel
        //        };

        //        try {
        //            EditPostResponse editPostResponse = await _requestProvider.PostAsync<EditPostRequest, EditPostResponse>(editPostRequest);

        //            if (editPostResponse != null) {
        //                editedPostDTO = new PostDTO() {
        //                    Author = editPostResponse.Author,
        //                    Comments = editPostResponse.Comments,
        //                    PostPolicyName = editPostResponse.PostPolicyName,
        //                    PostPolicyType = editPostResponse.PostPolicyType,
        //                    Id = editPostResponse.Id,
        //                    Media = editPostResponse.Media,
        //                    PublishTime = editPostResponse.PublishTime,
        //                    Text = editPostResponse.Text
        //                };
        //            }
        //        }
        //        catch (ServiceAuthenticationException exc) {
        //            _identityUtilService.RefreshToken();

        //            throw exc;
        //        }
        //        catch (Exception exc) {
        //            Crashes.TrackError(exc);

        //            Debug.WriteLine($"ERROR:{exc.Message}");
        //            throw;
        //        }

        //        return editedPostDTO;
        //    }, cancellationTokenSource.Token);

        private string NormalizeTextMessage(string rawText) {
            if (rawText != null) {
                return rawText.Replace("&nbsp;", "").Replace("<div>", "").Replace("</div>", Environment.NewLine).Replace("<br>", Environment.NewLine).Replace("&lt;", "<").Replace("&gt;", ">").TrimEnd();
            }

            return rawText;
        }
    }
}
