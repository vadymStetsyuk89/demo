using PeakMVP.Models.AppNavigation;
using PeakMVP.Models.DataItems.Autorization;
using System.Collections.Generic;

namespace PeakMVP.Services.Navigation {
    public interface INavigationContext {

        List<NavigationModeBase> BuildModes(ProfileType profileType);
    }
}
