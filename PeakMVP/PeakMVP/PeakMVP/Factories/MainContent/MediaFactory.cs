using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Identities.Medias;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Models.Rests.Responses.ProfileMedia;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Services.ProfileMedia;
using System;

namespace PeakMVP.Factories.MainContent {
    public class MediaFactory : IMediaFactory {

        private static readonly string UNHANDLED_MEDIA_TYPE_ERROR_MESSAGE = "MediaFactory unhandled media type";

        public MediaDTO BuildMediaDTO(SetAppBackgroundImageResponse setAppBackgroundImageResponse) {
            MediaDTO profileMediaDTO = null;

            if (setAppBackgroundImageResponse != null) {
                profileMediaDTO = new MediaDTO() {
                    Url = setAppBackgroundImageResponse.Url,
                    ThumbnailUrl = setAppBackgroundImageResponse.ThumbnailUrl,
                    Name = setAppBackgroundImageResponse.Name,
                    Id = setAppBackgroundImageResponse.Id
                };
            }

            return profileMediaDTO;
        }
        public AttachedFeedMedia BuidAttachedMedia(MediaType mediaType, string sourceBase64, MediaDTO mediaDTO, string thumbnailBase64 = "") {
            AttachedFeedMedia attachedFeedMedia = new AttachedFeedMedia() {
                MediaType = mediaType,
                SourceBase64 = sourceBase64,
                ThumbnailBase64 = thumbnailBase64,
                Id = mediaDTO.Id,
                UISource64 = (mediaType == MediaType.Picture) ? sourceBase64 : thumbnailBase64,
                SourceUrl = mediaDTO.Url,
                SourceThumbnailUrl = mediaDTO.ThumbnailUrl
            };
            return attachedFeedMedia;
        }

        public AttachedFeedMedia BuidAttachedMedia(MediaType mediaType, string sourceBase64, string thumbnailBase64 = "") {
            AttachedFeedMedia attachedFeedMedia = new AttachedFeedMedia() {
                MediaType = mediaType,
                SourceBase64 = sourceBase64,
                ThumbnailBase64 = thumbnailBase64,
                UISource64 = (mediaType == MediaType.Picture) ? sourceBase64 : thumbnailBase64
            };

            return attachedFeedMedia;
        }

        public AttachedFeedMedia BuidAttachedMedia(Media media, string sourceBase64 = "", string thumbnailBase64 = "") {
            AttachedFeedMedia attachedFeedMedia = new AttachedFeedMedia() {
                MediaType = (media.Data.Mime == ProfileMediaService.MIME_IMAGE_TYPE)
                    ? MediaType.Picture
                    : (media.Data.Mime == ProfileMediaService.MIME_VIDEO_TYPE)
                        ? MediaType.Video
                        : throw new InvalidOperationException(UNHANDLED_MEDIA_TYPE_ERROR_MESSAGE),
                SourceBase64 = sourceBase64,
                ThumbnailBase64 = thumbnailBase64,
                Id = media.Data.Id,
                SourceUrl = media.Url,
                SourceThumbnailUrl = media.ThumbnailUrl
            };

            attachedFeedMedia.UISource64 = (attachedFeedMedia.MediaType == MediaType.Picture) ? sourceBase64 : thumbnailBase64;

            return attachedFeedMedia;
        }


        public AttachedFeedMedia BuidAttachedMedia(AttachedFileDataModel attachedFileData, MediaDTO mediaDTO) {
            AttachedFeedMedia attachedFeedMedia = new AttachedFeedMedia() {
                MediaType = (attachedFileData.MimeType == ProfileMediaService.IMAGE_MEDIA_TYPE)
                    ? MediaType.Picture
                    : (attachedFileData.MimeType == ProfileMediaService.VIDEO_MEDIA_TYPE)
                        ? MediaType.Video
                        : throw new InvalidOperationException(UNHANDLED_MEDIA_TYPE_ERROR_MESSAGE),
                SourceBase64 = attachedFileData.File.Base64,
                ThumbnailBase64 = attachedFileData.Thumbnail.Base64,
                Id = mediaDTO.Id
            };

            attachedFeedMedia.UISource64 = (attachedFeedMedia.MediaType == MediaType.Picture) ? attachedFileData.File.Base64 : attachedFileData.Thumbnail.Base64;

            return attachedFeedMedia;
        }

        public void BuildValidUrlPath(ProfileMediaDTO media) {
            if (media != null) {
                media.ThumbnailUrl = Uri.IsWellFormedUriString(media.ThumbnailUrl, UriKind.Absolute) ? media.ThumbnailUrl : string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, media.ThumbnailUrl);
                media.Url = Uri.IsWellFormedUriString(media.Url, UriKind.Absolute) ? media.Url : string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, media.Url);
            }
        }

        public MediaDTO BuildMediaDTO(UploadMediaResponse uploadMediaResponse) {
            MediaDTO media = null;

            if (uploadMediaResponse != null) {
                media = new MediaDTO() {
                    Id = uploadMediaResponse.Id,
                    Mime = uploadMediaResponse.Mime,
                    Name = uploadMediaResponse.Name
                };

                media.ThumbnailUrl = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, uploadMediaResponse.ThumbnailUrl);
                media.Url = string.Format(GlobalSettings.Instance.Endpoints.MediaEndPoints.GetMediaEndPoints, uploadMediaResponse.Url);
            }

            return media;
        }
    }
}
