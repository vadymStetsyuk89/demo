using PeakMVP.Models.Identities.Medias;
using Plugin.Media.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace PeakMVP.Services.MediaPicker {
    public interface IMediaPickerService {
        Task<MediaFile> TakePhotoAsync();

        Task<MediaFile> PickPhotoAsync();

        Task<MediaFile> TakeVideoAsync();

        Task<MediaFile> PickVideoAsync();

        Task<string> ParseStreamToBase64(Stream stream);

        Task<Stream> ExtractStreamFromMediaUrlAsync(string urlPath);

        Task<PickedImage> BuildPickedImageAsync(MediaFile mediaFile);
    }
}
