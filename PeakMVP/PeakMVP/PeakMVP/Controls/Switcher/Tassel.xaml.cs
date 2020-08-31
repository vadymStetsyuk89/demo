using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Switcher {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tassel : ContentView {

        public Tassel() {
            InitializeComponent();

            ApplyVisualSelection();
        }

        private Color _selectedMainColor;
        public Color SelectedMainColor {
            get => _selectedMainColor;
            set {
                _selectedMainColor = value;

                ApplyVisualSelection();
            }
        }

        private Color _unSelectedMainColor;
        public Color UnSelectedMainColor {
            get => _unSelectedMainColor;
            set {
                _unSelectedMainColor = value;

                ApplyVisualSelection();
            }
        }

        private bool _isTasselSelected;
        public bool IsTasselSelected {
            get => _isTasselSelected;
            set {
                _isTasselSelected = value;

                ApplyVisualSelection();
            }
        }

        private void ApplyVisualSelection() {
            if (IsTasselSelected) {
                _mainTitle_Label.TextColor = SelectedMainColor;
            }
            else {
                _mainTitle_Label.TextColor = UnSelectedMainColor;
            }
        }
    }
}