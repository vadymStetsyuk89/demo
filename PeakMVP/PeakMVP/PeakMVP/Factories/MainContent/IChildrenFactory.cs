using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface IChildrenFactory {
        List<ChildItemViewModel> MakeChildrenItems(IEnumerable<ProfileDTO> profiles);

        ChildItemViewModel MakeChild(RegistrationResponse registrationResponse);
    }
}
