using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Models.Rests.Responses.Teams;

namespace PeakMVP.Factories.MainContent {
    public interface IProfileFactory {

        ProfileDTO BuildProfileDTO(NixTeamAppointmentResponse data);

        ProfileDTO BuildProfileDTO(RegistrationResponse data);

        ProfileDTO BuildProfileDTO(GetProfileResponse data);
    }
}
