using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Profile {
    public interface IProfileService {
        Task<GetProfileResponse> GetProfileAsync();

        Task<GetProfileResponse> GetProfileByShortIdAsync(string shortId, CancellationToken cancellationToken);

        Task<ProfileDTO> GetProfileByIdAsync(long id, CancellationToken cancellationToken);

        Task<bool> SetAvatarAsync(long mediaId, CancellationTokenSource cancellationTokenSource, long profileId = default(long));

        Task<SetProfileSettingsResponse> SetProfileAsync(SetProfileDataModel setProfileDataModel, CancellationTokenSource cancellationTokenSource);

        Task<bool> SetAppBackgroundImage(long mediaId, CancellationTokenSource cancellationTokenSource);

        Task<GetProfileResponse> GetProfileAsync(string accessToken, CancellationTokenSource cancellationTokenSource);

        Task ImpersonateLoginAsync(long targetId, CancellationTokenSource cancellationTokenSource);

        Task TryToRefreshLocalUserProfile(CancellationTokenSource cancellationTokenSource);

        Task<bool> UpdateOuterProfileInfoAsync(OuterProfileEditDataModel profileEdit, CancellationTokenSource cancellationTokenSource);
    }
}
