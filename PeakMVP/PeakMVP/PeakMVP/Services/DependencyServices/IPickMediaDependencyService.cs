using PeakMVP.Services.DependencyServices.Arguments;
using System.IO;
using System.Threading.Tasks;

namespace PeakMVP.Services.DependencyServices {
    public interface IPickMediaDependencyService {

        Stream GetThumbnail(string pathMediaPath);

        Task<IPickedMediaFromGallery> PickVideoAsync();

        Task<IPickedMediaFromGallery> PickMediaAsync();

        Task<IPickedMediaFromGallery> TakePhotoOrVideoAsync();
    }
}
