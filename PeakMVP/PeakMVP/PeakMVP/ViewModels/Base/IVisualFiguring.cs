using System;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.Base {
    public interface IVisualFiguring {

        Type RelativeViewType { get; }

        string TabHeader { get; }

        void Dispose();

        Task InitializeAsync(object navigationData);
    }
}
