using PeakMVP.Controls.Stacklist;
using PeakMVP.Controls.Stacklist.Base;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendItemView : CommonStackListItem {
        public FriendItemView() {
            InitializeComponent();
        }

        public override void Deselected() {

        }

        public override void Selected() {

        }
    }
}