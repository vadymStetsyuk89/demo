using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Identities.Medias;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Services.DependencyServices.Arguments;
using System.IO;
using System.Threading.Tasks;

namespace PeakMVP.Factories.MainContent {
    public interface IFileDTOBuilder {
        FileDTO BuidFileDTO(MediaType mediaType, string base64Data);

        FileDTO BuidFileDTO(PickedImage pickedImage);

        Task<AttachedFileDataModel> BuildAttachedPictureAsync(Stream pictureStream);

        Task<AttachedFileDataModel> BuildAttachedVideoAsync(Stream videoStream, string videoFilePath);

        Task<AttachedFileDataModel> BuildAttachedMediaAsync(IPickedMediaFromGallery mediaFromGallery);
    }
}
