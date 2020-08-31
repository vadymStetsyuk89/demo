using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Controls.Popovers {
    public interface IPopover {

        bool IsPopoverVisible { get; set; }

        IList ItemContext { get; set; }

        object SelectedItem { get; set; }

        string HintText { get; set; }

        bool IsHaveSameWidth { get; set; }
    }
}
