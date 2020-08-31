using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace PeakMVP.Controls {
    public class ListViewExtended : ListView {

        public ListViewExtended() {
            TODO();
        }

        public ListViewExtended(ListViewCachingStrategy strategy)
            : base(strategy) {
            TODO();
        }

        private void TODO() {
            ItemAppearing += TODO_HANDLER;
            ItemDisappearing += TODO_HANDLER;
        }

        private void TODO_HANDLER(object sender, ItemVisibilityEventArgs e) {
            Console.WriteLine("===> TODO_HANDLER");
        }

        public double YScrollOutput { get; set; }

        public bool AutoScrollDownWhenContentChanged { get; set; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemsSourceProperty.PropertyName) {
                if (ItemsSource is INotifyCollectionChanged notifyCollection) {
                    notifyCollection.CollectionChanged += OnNotifyCollectionCollectionChanged;
                }

                ScrollToTheLast();
            }
        }

        private void OnNotifyCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => ScrollToTheLast();

        private void ScrollToTheLast() {
            if (AutoScrollDownWhenContentChanged) {
                if (ItemsSource is IEnumerable<object> iEnumerable && iEnumerable.Any()) {
                    ScrollTo(iEnumerable.Last(), ScrollToPosition.MakeVisible, false);
                }
            }
        }
    }
}
