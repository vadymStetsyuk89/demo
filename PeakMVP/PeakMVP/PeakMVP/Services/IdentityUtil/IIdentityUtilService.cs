using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using System.Threading.Tasks;

namespace PeakMVP.Services.IdentityUtil {
    public interface IIdentityUtilService {

        void RefreshToken();

        Task<bool> ChargeUserProfileAsync(GetProfileResponse getProfileResponse, bool restartSignalHubs);

        Task<bool> ChargeUserProfileAsync(UserProfile userProfile, bool restartSignalHubs);

        Task<bool> ChargeUserProfileAsync(SetProfileSettingsResponse setProfileSettingsResponse, bool restartSignalHubs);

        bool ChargeUserProfileAvatar(MediaDTO mediaDTO);

        bool ChargeUserProfileAvatar(ProfileMediaDTO mediaDTO);

        bool ChargeUserProfileAppBackgroundImage(MediaDTO profileMediaDTO);

        Task ChargeImpersonateUserProfileAsync(string accessToken, GetProfileResponse getProfileResponse);

        void LogOut();

        void ImpersonateLogOut();
    }
}
