using PeakMVP.Controls.Popovers.Base;
using System;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.Events {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameEventActionsPopover : PopoverBase {
        public GameEventActionsPopover() {
            InitializeComponent();
        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs e) {
            IsPopoverVisible = false;
        }
    }
}