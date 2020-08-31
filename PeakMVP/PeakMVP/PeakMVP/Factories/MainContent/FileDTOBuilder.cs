using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Identities.Medias;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.ProfileMedia;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.Factories.MainContent {
    public class FileDTOBuilder : IFileDTOBuilder {

        private readonly IMediaPickerService _mediaPickerService;

        public FileDTOBuilder(IMediaPickerService mediaPickerService) {
            _mediaPickerService = mediaPickerService;
        }

        public FileDTO BuidFileDTO(MediaType mediaType, string base64Data) {
            return new FileDTO() {
                Base64 = base64Data,
                Name = string.Format("{0}.{1}",
                    Guid.NewGuid(),
                    (mediaType == MediaType.Picture)
                        ? ProfileMediaService.PNG_IMAGE_FORMAT
                        : ProfileMediaService.MP4_VIDEO_FORMAT)
            };
        }

        public FileDTO BuidFileDTO(PickedImage pickedImage) {
            FileDTO file = null;

            if (pickedImage != null) {
                file = new FileDTO() {
                    Base64 = pickedImage.DataBase64,
                    Name = string.Format("{0}.{1}", Guid.NewGuid(), ProfileMediaService.PNG_IMAGE_FORMAT)
                };
            }

            return file;
        }

        public Task<AttachedFileDataModel> BuildAttachedPictureAsync(Stream pictureStream) =>
            Task<AttachedFileDataModel>.Run(async () => {
                FileDTO pictureFile = new FileDTO();
                pictureFile.Base64 = await _mediaPickerService.ParseStreamToBase64(pictureStream);
                pictureFile.Name = string.Format("{0}.{1}", Guid.NewGuid(), ProfileMediaService.PNG_IMAGE_FORMAT);

                FileDTO pictureThumbnail = new FileDTO();
                pictureThumbnail.Base64 = pictureFile.Base64;
                pictureThumbnail.Name = string.Format("{0}_thumbnail.{1}", Guid.NewGuid(), ProfileMediaService.PNG_IMAGE_FORMAT);

                pictureStream.Dispose();
                pictureStream.Close();

                return new AttachedFileDataModel() {
                    File = pictureFile,
                    Thumbnail = pictureThumbnail,
                    MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE
                };
            });

        public Task<AttachedFileDataModel> BuildAttachedVideoAsync(Stream videoStream, string videoFilePath) =>
            Task<AttachedFileDataModel>.Run(async () => {
                FileDTO videoFile = new FileDTO();
                videoFile.Base64 = await _mediaPickerService.ParseStreamToBase64(videoStream);
                videoFile.Name = string.Format("{0}.{1}", Guid.NewGuid(), ProfileMediaService.MP4_VIDEO_FORMAT);

                videoStream.Dispose();
                videoStream.Close();

                Stream thumbnailStream = DependencyService.Get<IPickMediaDependencyService>().GetThumbnail(videoFilePath);
                string base64thumbnail = await _mediaPickerService.ParseStreamToBase64(thumbnailStream);

                FileDTO pictureThumbnail = new FileDTO();
                pictureThumbnail.Base64 = base64thumbnail;
                pictureThumbnail.Name = string.Format("{0}_thumbnail.{1}", Guid.NewGuid(), ProfileMediaService.PNG_IMAGE_FORMAT);

                thumbnailStream.Dispose();
                thumbnailStream.Close();

                return new AttachedFileDataModel() {
                    File = videoFile,
                    Thumbnail = pictureThumbnail,
                    MimeType = ProfileMediaService.VIDEO_MEDIA_TYPE
                };
            });

        public Task<AttachedFileDataModel> BuildAttachedMediaAsync(IPickedMediaFromGallery mediaFromGallery) =>
            Task<AttachedFileDataModel>.Run(() => {
                if (mediaFromGallery.MimeType != ProfileMediaService.IMAGE_MEDIA_TYPE && mediaFromGallery.MimeType != ProfileMediaService.VIDEO_MEDIA_TYPE) {
                    Debugger.Break();
                    throw new InvalidOperationException("Unresolved app media type.");
                }

                FileDTO mainSourceFile = new FileDTO();
                mainSourceFile.Base64 = mediaFromGallery.DataBase64;
                mainSourceFile.Name = string.Format("{0}.{1}", Guid.NewGuid(), mediaFromGallery.MimeType == ProfileMediaService.IMAGE_MEDIA_TYPE ? ProfileMediaService.PNG_IMAGE_FORMAT : ProfileMediaService.MP4_VIDEO_FORMAT);

                FileDTO thumbnail = new FileDTO();
                thumbnail.Base64 = mediaFromGallery.DataThumbnailBase64;
                thumbnail.Name = string.Format("{0}_thumbnail.{1}", Guid.NewGuid(), ProfileMediaService.PNG_IMAGE_FORMAT);

                return new AttachedFileDataModel() {
                    File = mainSourceFile,
                    Thumbnail = thumbnail,
                    MimeType = mediaFromGallery.MimeType
                };
            });
    }
}
