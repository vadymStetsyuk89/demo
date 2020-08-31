using PeakMVP.Controls.Popovers.Arguments;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Controls.Popovers {
    public interface IPopoverKeeper {

        void ShowPopover(IPopover popiver, ShowPopoverArgs showPopoverArgs);

        void HidePopover(IPopover popover);

        void HideAllPopovers();
    }
}