using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace PeakMVP.Models.AppNavigation {
    public class SocialMode : NavigationModeBase {

        public SocialMode(IEnumerable<IBottomBarTab> barItems) {
            BarItems = barItems.ToList().AsReadOnly();
        }

        public override BarMode BarModeType => BarMode.Social;

        public override string ModeIconPath => NavigationContext.SOCIAL_MODE_ICON_PATH;
        
        public override void Dispose() {
            base.Dispose();

            BarItems?.ForEach(bBT => bBT.Dispose());
        }
    }
}
