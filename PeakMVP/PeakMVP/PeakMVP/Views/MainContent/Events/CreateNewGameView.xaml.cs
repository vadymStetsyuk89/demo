using PeakMVP.Views.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.MainContent.Events {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateNewGameView : GameManagingViewBase {
        public CreateNewGameView() {
            InitializeComponent();
        }
    }
}