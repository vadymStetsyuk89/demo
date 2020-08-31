using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.ProfileMedia {
    public interface IProfileMediaService {
        Task<List<ProfileMediaDTO>> GetProfileMedia(CancellationTokenSource cancellationTokenSource);

        Task<bool> AddProfileMedia(AddMediaDataModel addProfileMediaRequestDataModel, CancellationTokenSource cancellationTokenSource);

        Task<bool> RemoveProfileMediaById(long id, CancellationTokenSource cancellationTokenSource);

        Task<MediaDTO> UploadMediaToTrayAsync(FileDTO file, CancellationTokenSource cancellationTokenSource);

        Task<bool> DeleteMediaFromTrayAsync(long mediaId, CancellationTokenSource cancellationTokenSource);
    }
}
