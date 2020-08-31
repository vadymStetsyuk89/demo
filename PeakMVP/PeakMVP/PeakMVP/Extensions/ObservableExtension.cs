using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PeakMVP.Extensions {
    public static class ObservableExtension {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection) {
            return new ObservableCollection<T>(collection);
        } 
    }
}
