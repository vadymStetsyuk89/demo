using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.Events {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsView : ContentView {

        private Color _selectedViewModeBorderColor = Color.FromHex("#BCC1C8");
        private Color _unSelectedViewModeBorderColor = Color.FromHex("#798897");

        public EventsView() {
            InitializeComponent();

            ///
            /// TODO: temporary implementation. Necessary to define appropriate switch control
            /// 
            _listViewButton_ExtendedContentView.BorderColor = _selectedViewModeBorderColor;
            _calendarViewButton_ExtendedContentView.BorderColor = _unSelectedViewModeBorderColor;
        }

        ///
        /// TODO: temporary implementation. Necessary to define appropriate switch control
        /// 
        private void TODO_TemporaryImplementation_OnTapGestureRecognizerTapped(object sender, EventArgs e) {
            ///
            /// TODO: temporary implementation. Necessary to define appropriate switch control
            /// 
            if (sender == _calendarViewButton_ExtendedContentView) {
                _listViewButton_ExtendedContentView.BorderColor = _unSelectedViewModeBorderColor;
                _calendarViewButton_ExtendedContentView.BorderColor = _selectedViewModeBorderColor;
            }
            else if (sender == _listViewButton_ExtendedContentView) {
                _listViewButton_ExtendedContentView.BorderColor = _selectedViewModeBorderColor;
                _calendarViewButton_ExtendedContentView.BorderColor = _unSelectedViewModeBorderColor;
            }
        }
    }
}