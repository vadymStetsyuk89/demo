using PeakMVP.ViewModels.Base;
using System.Threading.Tasks;

namespace PeakMVP.Models.DataItems.MainContent.Messenger {
    public interface IMessengerDataItem : IAskToRefresh {

        string Title { get; set; }

        void Dispose();

        Task InitializeAsync(object navigationData);

        bool IsSelected { get; set; }
    }
}
