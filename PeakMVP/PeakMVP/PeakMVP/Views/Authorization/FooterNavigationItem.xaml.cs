using PeakMVP.Controls.Stacklist;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.Authorization {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FooterNavigationItem : CommonStackListItem {

        /// <summary>
        /// Public ctor
        /// </summary>
        public FooterNavigationItem() {
            InitializeComponent();

            _underline_BoxView.BackgroundColor = SelectedColor;
            _underline_BoxView.IsVisible = false;
        }

        public override void Deselected() {
            _header_Label.TextColor = DeselectedColor;
            _underline_BoxView.BackgroundColor = DeselectedColor;
            _underline_BoxView.IsVisible = false;
        }

        public override void Selected() {
            _header_Label.TextColor = SelectedColor;
            _underline_BoxView.BackgroundColor = SelectedColor;
            _underline_BoxView.IsVisible = true;
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();

            object selfBC = BindingContext;
        }
    }
}