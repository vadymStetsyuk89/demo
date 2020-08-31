using PeakMVP.Controls.Stacklist.Base;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileTileView : SourceItemBase {

        /// <summary>
        ///     ctor().
        /// </summary>
        public ProfileTileView() {
            InitializeComponent();

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;

            GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs e) {
            SelectionAction(this);
        }

        public override void Deselected() {
            _frame_ExtendedContentView.BackgroundColor = Color.Transparent;
        }

        public override void Selected() {
            _frame_ExtendedContentView.BackgroundColor = Color.White;
        }
    }
}