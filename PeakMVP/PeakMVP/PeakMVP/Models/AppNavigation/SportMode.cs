using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace PeakMVP.Models.AppNavigation {
    public class SportMode : NavigationModeBase {

        public SportMode(IEnumerable<IBottomBarTab> barItems) {
            BarItems = barItems.ToList().AsReadOnly();
        }

        public override string ModeIconPath => NavigationContext.SPORT_MODE_ICON_PATH;

        public override BarMode BarModeType => BarMode.Sport;

        public override void Dispose() {
            base.Dispose();

            BarItems?.ForEach(bBT => bBT.Dispose());
        }
    }
}
