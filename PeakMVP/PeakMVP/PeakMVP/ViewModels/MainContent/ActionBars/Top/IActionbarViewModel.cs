using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.ActionBars.Top {
    public interface IActionbarViewModel {

        ContentPageBaseViewModel RelativeContentPageBaseViewModel { get; }

        Task InitializeAsync(object navigationData);

        void Dispose();
    }
}
