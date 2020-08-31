using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.ViewModels.Base {
    public interface INestedInContentPageBase {
        ContentPageBaseViewModel RelativeContentPageBaseViewModel { get; }
    }
}
