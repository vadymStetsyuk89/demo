using System;

namespace PeakMVP.Controls.Popups {
    public interface IPopupContext {

        Type RelativeViewType { get; }

        string Title { get; }
    }
}
