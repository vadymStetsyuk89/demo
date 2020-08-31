using PeakMVP.Views.Base;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.MainContent.Events {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class GameManagingViewBase : ContentPageBase {
        public GameManagingViewBase() {
            InitializeComponent();
        }
    }
}