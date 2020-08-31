using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Switcher {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectionRunner : ContentView {

        private ContentSwitcher _contentSwitcher;

        public SelectionRunner() {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            if (propertyName == WidthProperty.PropertyName && Width > 0 && _contentSwitcher != null) {
                _contentSwitcher.TasselVisualSelection();
            }
        }

        public void RegisterRelativeSwitcher(ContentSwitcher contentSwitcher) => _contentSwitcher = contentSwitcher;
    }
}