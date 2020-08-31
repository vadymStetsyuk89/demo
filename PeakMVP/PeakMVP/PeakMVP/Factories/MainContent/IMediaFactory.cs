using PeakMVP.Models.Identities;
using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Models.Rests.Responses.ProfileMedia;
using PeakMVP.Models.Rests.Responses.ProfileResponses;

namespace PeakMVP.Factories.MainContent {
    public interface IMediaFactory {

        MediaDTO BuildMediaDTO(SetAppBackgroundImageResponse setAppBackgroundImageResponse);

        MediaDTO BuildMediaDTO(UploadMediaResponse uploadMediaResponse);

        AttachedFeedMedia BuidAttachedMedia(MediaType mediaType, string sourceBase64, string thumbnailBase64 = "");

        AttachedFeedMedia BuidAttachedMedia(MediaType mediaType, string sourceBase64, MediaDTO mediaDTO, string thumbnailBase64 = "");

        AttachedFeedMedia BuidAttachedMedia(Media media, string sourceBase64 = "", string thumbnailBase64 = "");

        AttachedFeedMedia BuidAttachedMedia(AttachedFileDataModel attachedFileData, MediaDTO mediaDTO);

        void BuildValidUrlPath(ProfileMediaDTO media);
    }
}
