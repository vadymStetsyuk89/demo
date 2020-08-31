using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.ProfileMedia;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Rests.Responses.ProfileMedia;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;

namespace PeakMVP.Services.ProfileMedia {
    public class ProfileMediaService : IProfileMediaService {

        public static readonly string IMAGE_MEDIA_CATEGORY = "Picture";
        public static readonly string VIDEO_MEDIA_CATEGORY = "Video";
        public static readonly string IMAGE_MEDIA_TYPE = "image/";
        public static readonly string VIDEO_MEDIA_TYPE = "video/";
        public static readonly string PNG_IMAGE_FORMAT = "png";
        public static readonly string MP4_VIDEO_FORMAT = "mp4";
        public static readonly string JPG_INAGE_FORMAT = "jpg";

        public static readonly string MIME_IMAGE_TYPE = "image/jpeg";
        public static readonly string MIME_VIDEO_TYPE = "video/mp4";

        public static readonly string ADD_IMAGE_SUCCESSFUL_COMPLETION_MESSAGE = "The picture was added to albums";
        public static readonly string ADD_VIDEO_SUCCESSFUL_COMPLETION_MESSAGE = "The video was added to albums";
        public static readonly string ADD_MEDIA_SUCCESSFUL_COMPLETION_MESSAGE = "The media was added to albums";

        public static readonly string CANT_UPLOAD_MEDIA_COMMON_ERROR = "Can't upload media now";
        public static readonly string CANT_DELETE_MEDIA_COMMON_ERROR = "Can't delete media now";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly IMediaFactory _mediaFactory;

        /// <summary>
        ///     ctor().
        /// </summary>
        public ProfileMediaService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService,
            IMediaFactory mediaFactory) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
            _mediaFactory = mediaFactory;
        }

        public async Task<MediaDTO> UploadMediaToTrayAsync(FileDTO file, CancellationTokenSource cancellationTokenSource) =>
            await Task<MediaDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                MediaDTO media = null;

                try {
                    UploadMediaRequest uploadMediaRequest = new UploadMediaRequest {
                        AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                        Url = GlobalSettings.Instance.Endpoints.ProfileMediaEndPoints.UploadMedia,
                        Data = file
                    };

                    UploadMediaResponse uploadMediaResponse = await _requestProvider.PostAsync<UploadMediaRequest, UploadMediaResponse>(uploadMediaRequest);

                    if (uploadMediaResponse != null) {
                        media = _mediaFactory.BuildMediaDTO(uploadMediaResponse);
                    } else {
                        throw new InvalidOperationException(CANT_UPLOAD_MEDIA_COMMON_ERROR);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    media = null;

                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw;
                }

                return media;
            }, cancellationTokenSource.Token);

        public Task<bool> DeleteMediaFromTrayAsync(long mediaId, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                bool completion = false;

                try {
                    DeleteMediaRequest deleteMediaRequest = new DeleteMediaRequest {
                        AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                        Url = string.Format(GlobalSettings.Instance.Endpoints.ProfileMediaEndPoints.DeleteMedia, mediaId)
                    };

                    DeleteMediaResponse deleteMediaResponse = await _requestProvider.DeleteAsync<DeleteMediaRequest, DeleteMediaResponse>(deleteMediaRequest);

                    if (deleteMediaResponse != null) {
                        completion = true;
                    } else {
                        throw new InvalidOperationException(CANT_DELETE_MEDIA_COMMON_ERROR);
                    }
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    completion = false;

                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    throw;
                }

                return completion;
            }, cancellationTokenSource.Token);

        public async Task<List<ProfileMediaDTO>> GetProfileMedia(CancellationTokenSource cancellationTokenSource) =>
            await Task<List<ProfileMediaDTO>>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<ProfileMediaDTO> media = new List<ProfileMediaDTO>();

                GetProfileMediaRequest getProfileMediaRequest = new GetProfileMediaRequest {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ProfileMediaEndPoints.GetProfileMediaEndPoints,
                                    GlobalSettings.Instance.UserProfile.Id)
                };

                GetProfileMediaResponse getProfileMediaResponse = null;

                try {
                    getProfileMediaResponse = await _requestProvider.GetAsync<GetProfileMediaRequest, GetProfileMediaResponse>(getProfileMediaRequest);

                    if (getProfileMediaResponse != null) {
                        media = getProfileMediaResponse?.Media
                            .Select<ProfileMediaDTO, ProfileMediaDTO>(m => {
                                m.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, m.ThumbnailUrl);
                                m.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, m.Url);

                                return m;
                            })
                            .ToList<ProfileMediaDTO>();
                    }
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

                return media;
            }, cancellationTokenSource.Token);

        public Task<bool> AddProfileMedia(AddMediaDataModel addProfileMediaRequestDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                AddProfileMediaResponse mediaResponse = null;
                //ProfileMediaDTO mediaResult = null;

                MediaDTO mediaDTO = await UploadMediaToTrayAsync(addProfileMediaRequestDataModel.File, cancellationTokenSource);
                if (mediaDTO != null) {
                    addProfileMediaRequestDataModel.MediaId = mediaDTO.Id;

                    AddProfileMediaRequest addProfileMediaRequest = new AddProfileMediaRequest() {
                        Url = GlobalSettings.Instance.Endpoints.ProfileMediaEndPoints.AddProfileMediaEndPoint,
                        AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                        Data = addProfileMediaRequestDataModel
                    };


                    try {
                        mediaResponse = await _requestProvider.PostAsync<AddProfileMediaRequest, AddProfileMediaResponse>(addProfileMediaRequest);

                        if (mediaResponse != null) {
                            return true;
                            //mediaResult = new ProfileMediaDTO() {
                            //    Id = mediaDTO.Id,
                            //    Name = mediaDTO.Name,
                            //    ThumbnailUrl = mediaDTO.ThumbnailUrl,
                            //    Url = mediaDTO.Url,
                            //    Mime = mediaDTO.Mime
                            //};
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
                }
                return false;
            }, cancellationTokenSource.Token);

        public Task<bool> RemoveProfileMediaById(long id, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                RemoveMediaByIdRequest removeMediaByIdRequest = new RemoveMediaByIdRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = string.Format(GlobalSettings.Instance.Endpoints.ProfileMediaEndPoints.RemoveProfileMediaByIdEndPoint, id)
                };

                RemoveMediaByIdResponse removeMediaByIdResponse = null;

                try {
                    removeMediaByIdResponse = await _requestProvider.PostAsync<RemoveMediaByIdRequest, RemoveMediaByIdResponse>(removeMediaByIdRequest);

                    if (removeMediaByIdResponse != null) {
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
    }
}
