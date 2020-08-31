using System.Collections.ObjectModel;

namespace PeakMVP.Services.DataItems.Contracts {
    public interface IDataItems<T> {
        ObservableCollection<T> BuildDataItems();
    }
}
